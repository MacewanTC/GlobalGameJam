using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Homeowner : MonoBehaviour
{
    public enum HomeownerState
    {
        Moving, Waiting, Focusing, Looking
    }

    public HomeownerState initalState = HomeownerState.Moving;
        public HomeownerState currentState
    {
        get { return m_currentState; }
        set
        {
            m_currentState = value;
            timeInState = 0f;
        }
    }
    public HomeownerState m_currentState;
    public float currentAlarm,
        maxAlarm,
        alarmDecay,
        movementSpeed,
        fov, 
        losRadius,
        waitingTime,
        timeInState,
        lookingTime,
        turnSpeed,
        turnMagnitude;
    public Waypoint destination;
    public LayerMask losMask;
    public List<GameObject> targets;
    public AnimationCurve losFalloff;
    public bool sawAnyTargetThisFrame;
    public Vector2 lastSeen;

    // Start is called before the first frame update
    void Start()
    {
        currentState = initalState;
        selectDestination();
    }

    // Update is called once per frame
    void Update()
    {
        timeInState += Time.deltaTime;
        checkLOS();
        switch (currentState)
        {
            case HomeownerState.Moving:
                if (sawAnyTargetThisFrame) //Transition to focusing
                    currentState = HomeownerState.Focusing;
                else if (!destination)
                    break;

                var direction = destination.transform.position - transform.position;
                var distance = direction.magnitude;
                if (distance < destination.arrivalRadius) //Transition to waiting
                {
                    waitingTime = Random.Range(destination.minDuration, destination.maxDuration);
                    currentState = HomeownerState.Waiting; 
                }
                else //Continue moving to and facing destination
                {
                    var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                    transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                    var velocity = direction.normalized * movementSpeed;
                    transform.transform.Translate(velocity);
                }
                break;
            case HomeownerState.Waiting:
                waitingTime -= Time.deltaTime;
                if (sawAnyTargetThisFrame)
                {
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
                    //Game over
                }
                else if (!sawAnyTargetThisFrame)
                {
                    currentState = HomeownerState.Looking;
                }
                else //Face the last seen target
                {
                    direction.x = lastSeen.x - transform.position.x;
                    direction.y = lastSeen.y - transform.position.y;
                    var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                    transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                }
                break;
            case HomeownerState.Looking:
                if (sawAnyTargetThisFrame)
                {
                    currentState = HomeownerState.Focusing;
                }
                else if (timeInState >= lookingTime)
                {
                    currentState = HomeownerState.Moving;
                }
                else
                {
                    transform.Rotate(Vector3.forward, Mathf.Sin(Time.time) * turnSpeed);
                }
                break;
            default:
                break;
        }
    }

    private void checkLOS()
    {
        sawAnyTargetThisFrame = false;
        foreach (var target in targets)
        {
            var direction = target.transform.position - transform.position;
            var raycast = Physics2D.Raycast(transform.position, direction, losRadius, losMask);
            if (raycast.collider)
            {
                sawAnyTargetThisFrame = true;
                var alarm = losFalloff.Evaluate(1 - raycast.fraction);
                currentAlarm += alarm;
                lastSeen = raycast.point;
            }
        }
        if (!sawAnyTargetThisFrame)
        {
            currentAlarm = Mathf.Lerp(currentAlarm, 0, alarmDecay);
        }
    }

    void selectDestination()
    {
        if (!destination)
            return;
        var index = Random.Range(0, destination.adjacents.Count);
        destination = destination.adjacents[index];
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, losRadius);
    }
}
