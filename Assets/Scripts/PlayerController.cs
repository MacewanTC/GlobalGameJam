using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public float hunger = 1.0f;
    public float sleep = 1.0f;
    public float hygiene = 1.0f;
    public float hungerDecay =  0.01f;
    public float sleepDecay = 0.01f;
    public float hygieneDecay = 0.01f;
    public Slider hungerSlider;
    public Slider sleepSlider;
    public Slider hygieneSlider;

    public float passOutTime = 3.0f;
    public float passOutRegen = 0.1f;

    public float speed = 3.0f;
    public float altSpeed = 6.0f;

    private GameManager gameManager;
	public Animator animator;

    private Rigidbody2D body;

    private float frozenTime;
    private bool isFrozen = false;

    private float visibility;

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
		gameManager = FindObjectOfType<GameManager>();

		animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (!isFrozen)
        {
            ChangeNeed(Need.hunger, -hungerDecay * Time.deltaTime);
            ChangeNeed(Need.sleep, -sleepDecay * Time.deltaTime);
            ChangeNeed(Need.hygiene, -hygieneDecay * Time.deltaTime);
        }
        if (sleep == 0.0f)
        {
            Freeze(passOutTime);
            ChangeNeed(Need.sleep, passOutRegen);
        }
        
        if (hunger == 0.0f)
        {
            gameManager.EndGame();
        }

        visibility = 1.0f - hygiene;
    }

    public float GetVisibility()
    {
        return visibility;
    }

    public void Freeze(float freezeTime)
    {
        if (freezeTime >= 0.0f)
        {
            isFrozen = true;
        }
        frozenTime = Mathf.Max(frozenTime, freezeTime);
    }

    // enum need { hunger = 1, sleep = 2}
	public enum Need { hunger, sleep, hygiene};
    public void ChangeNeed(Need need, float delta) 
    {
        if (need == Need.hunger)
        {
            hunger += delta;
            hunger = Mathf.Clamp(hunger, 0.0f, 1.0f);
            hungerSlider.value = hunger;
        }
        else if (need == Need.sleep)
        {
            sleep += delta;
            sleep = Mathf.Clamp(sleep, 0.0f, 1.0f);
            sleepSlider.value = sleep;
        }
        else if (need == Need.hygiene)
        {
            hygiene += delta;
            hygiene = Mathf.Clamp(hygiene, 0.0f, 1.0f);
            hygieneSlider.value = hygiene;
        }

    }

    void FixedUpdate()
    {
        if (isFrozen)
        {
            frozenTime -= Time.deltaTime;
            if (frozenTime <= 0.0f)
            {
                isFrozen = false;
                frozenTime = 0.0f;
            }
        }
        else
        {
            Vector2 moveDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

			if (moveDirection.magnitude > 0.1) StartStepSFX();
			else StopStepSFX();

			if (moveDirection.magnitude != 0) {
				//Rotate Towards Direction
				transform.rotation = Quaternion.Slerp(transform.rotation, 
					Quaternion.AngleAxis((Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg) + 270, Vector3.forward), 
					Time.deltaTime * speed * 3);
			}
				
            if (Input.GetButton("Fire2"))
            {
                body.MovePosition((Vector2)transform.position + (moveDirection.normalized * altSpeed * Time.deltaTime));
            }
            else
            {
                body.MovePosition((Vector2)transform.position + (moveDirection.normalized * speed * Time.deltaTime));
            }
        }
    }

	private void StartStepSFX() {
		animator.SetBool("Walking", true);
		AudioController.instance.StartPlayerStep();
	}

	private void StopStepSFX() {
		animator.SetBool("Walking", false);
		AudioController.instance.StopPlayerStep();
	}
}
