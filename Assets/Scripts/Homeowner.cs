using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Homeowner : MonoBehaviour
{
    public enum HomeownerState
    {
        Moving = 0, Waiting, Focusing, Looking
    }

    public HomeownerState initalState = HomeownerState.Moving;
        public HomeownerState currentState
    {
        get { return m_currentState; }
        set
        {
            m_currentState = value;
            timeInState = 0f;
            transform.GetChild(0).GetComponent<Animator>().SetInteger("currentState", (int)value);
        }
    }
    public HomeownerState m_currentState;
    public float currentAlarm,
        maxAlarm,
        movementSpeed,
        fov,
        losRadius,
        waitingTime,
        timeInState,
        lookingTime,
        turnSpeed;
    public Waypoint destination;
    public LayerMask losMask;
    public List<GameObject> targets;
    public AnimationCurve losFalloff;
    public bool sawAnyTargetThisFrame;
    public Vector2 lastSeen;
    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
		gameManager = FindObjectOfType<GameManager>();

        currentState = initalState;
        selectDestination();
    }

    // Update is called once per frame
    void Update()
    {
        timeInState += Time.deltaTime;
        updateLos();
        updateAlarm();
        updateState();
    }

    private void updateState()
    {
        switch (currentState)
        {
            case HomeownerState.Moving:
                if (sawAnyTargetThisFrame)
                {//Transition to focusing
                    transform.GetChild(0).GetComponent<Animator>().SetTrigger("Point");
                    currentState = HomeownerState.Focusing;
                }
                else if (!destination)
                    break;

                var direction = directionToDestination();
                var distance = direction.magnitude;
                if (distance < destination.arrivalRadius) //Transition to waiting
                {
                    waitingTime = Random.Range(destination.minDuration, destination.maxDuration);
                    currentState = HomeownerState.Waiting;
                }
                else //Continue moving to and facing destination
                {
                    var velocity = direction.normalized * movementSpeed * Time.deltaTime;
                    transform.Translate(velocity, Space.World);
                    var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;
                    var q = Quaternion.AngleAxis(angle, Vector3.forward);
                    transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * turnSpeed);
                }
                break;
            case HomeownerState.Waiting:
                waitingTime -= Time.deltaTime;
                if (sawAnyTargetThisFrame)
                {
                    transform.GetChild(0).GetComponent<Animator>().SetTrigger("Point");
                    currentState = HomeownerState.Focusing;
                }
                else if (waitingTime <= 0)
                {
                    selectDestination();
                    currentState = HomeownerState.Moving;
                }
                break;
            case HomeownerState.Focusing:
                if (currentAlarm >= maxAlarm)
                {
                    gameManager.EndGame();
                    return;
                }
                if (!sawAnyTargetThisFrame)
                {
                    currentState = HomeownerState.Looking;
                }
                else //Face the last seen target
                {
                    direction.x = lastSeen.x - transform.position.x;
                    direction.y = lastSeen.y - transform.position.y;
                    var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;
                    var q = Quaternion.AngleAxis(angle, Vector3.forward);
                    transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * turnSpeed);
                }
                break;
            case HomeownerState.Looking:
                if (sawAnyTargetThisFrame)
                {
                    transform.GetChild(0).GetComponent<Animator>().SetTrigger("Point");
                    currentState = HomeownerState.Focusing;
                }
                else if (timeInState >= lookingTime)
                {
                    currentState = HomeownerState.Moving;
                }
                break;
            default:
                break;
        }
    }

    private void updateAlarm()
    {
        if (!sawAnyTargetThisFrame)
        {
            currentAlarm = Mathf.Lerp(currentAlarm, 0, Time.deltaTime);
            if (currentAlarm < maxAlarm / 4)
                AudioController.instance.ReturnToDefaultMusic();
        }
        else if (!(AudioController.instance.currentLocation == AudioController.PlayerState.CAUTIOUS ||
          AudioController.instance.currentLocation == AudioController.PlayerState.SEEN) && !gameManager.finished)
        {
            AudioController.instance.ActivateSeenMusic();
        }
    }

    private void updateLos()
    {
        sawAnyTargetThisFrame = false;
        foreach (var target in targets)
        {
            if (!inFOV(target.transform) || !inLOS(target.transform))
                continue; //Target is outside our fov or los radius, dont bother with raycasting

            Debug.Log("FOV & LOS tests passed, raycasting to target...");
            var direction = target.transform.position - transform.position;
			var raycast = Physics2D.Raycast(transform.position, direction, losRadius * FindObjectOfType<PlayerController>().GetVisibility(), losMask);
            if (raycast.collider && raycast.collider.tag == "Player")
            {
                sawAnyTargetThisFrame = true;
                var alarm = losFalloff.Evaluate(1 - raycast.fraction) * Time.deltaTime;
                currentAlarm += alarm;
				Debug.Log(string.Format("{0} can see {1}.", name, target.name));
                lastSeen = raycast.point;
            }
            else if (raycast.collider)
            {
                Debug.Log(string.Format("Raycast from {0} to {1} hit {2} instead.", name, target.name, raycast.collider.name));
            }
        }
    }

    void selectDestination()
    {
        if (!destination)
        {
            Debug.Log("Homeowner cannot get next destination because it does not have starting destination.");
            return;
        }
        var index = Random.Range(0, destination.adjacents.Count);
        destination = destination.adjacents[index];
    }

    void OnDrawGizmosSelected()
    {
        //Draw direction to up and destination
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, transform.up.normalized);
        Gizmos.DrawRay(transform.position, directionToDestination().normalized);

        //Draw raycasts to targets
        Gizmos.color = Color.red;
        foreach (var target in targets)
            Gizmos.DrawLine(transform.position, target.transform.position);

        //Draw fov cone
        Gizmos.color = Color.magenta;
        for (int i = -1; i <= 1; i += 2)
        {
            var angle = Quaternion.AngleAxis(fov / 2 * i, Vector3.forward);
            var ray = angle * transform.GetChild(0).up;
            Gizmos.DrawRay(transform.position, ray * losRadius);
        }

        //Reset colour
        Gizmos.color = Color.white;
    }

    Vector3 directionToDestination()
    {
        return destination.transform.position - transform.position;
    }

    bool inFOV(Transform target)
    {
        var direction = target.position - transform.position;
        var facing = transform.GetChild(0).up;
        var angle = Vector2.Angle(facing, direction);
        Debug.Log(string.Format("Angle from homeowner to target is {0}.", angle));
        var result = angle < fov / 2;
        if (!result)
            Debug.Log("Target outside fov angle.");
        return result;
    }

    bool inLOS(Transform target)
    {
		bool result = Vector2.Distance(target.position, transform.position) <= FindObjectOfType<PlayerController>().GetVisibility() * losRadius;
        if (!result)
            Debug.Log("Target outside los radius.");
        return result;
    }
}
