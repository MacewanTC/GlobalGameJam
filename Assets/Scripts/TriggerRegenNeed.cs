using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TriggerRegenNeed : MonoBehaviour
{
    private PlayerController player;
    public float regenAmount = 0.5f;
    public float regenTime = 2.0f;
	private float sleepPeriod = 5f;
	public PlayerController.Need need = PlayerController.Need.hunger; // See player controller :: ChangeNeed for enum


    public Text text; 

    void Start()
    {
		player = FindObjectOfType<PlayerController>();
        text.enabled = false;
    }

	private bool changingNeed = false;
	private float freezeTime, startTime;
	private float startAmount, endAmount;
	void Update() {
		if (changingNeed) {
			player.SetNeed(need, Mathf.Lerp(startAmount, endAmount, (Time.time-startTime) / freezeTime));
			if (((Time.time-startTime) / freezeTime) >= 1) changingNeed = false;
		}
	}

    void OnTriggerEnter2D()
    {
        text.enabled = true;
    }

    void OnTriggerExit2D()
    {
        text.enabled = false;
    }

    void OnTriggerStay2D()
    {
        if (Input.GetButton("Fire1") && !player.isFrozen)
        {
			if (need == PlayerController.Need.sleep)
	            freezeTime = regenTime*sleepPeriod;
			else 
				freezeTime = regenTime;
			
			player.Freeze(freezeTime);
			
            changingNeed = true;
			startTime = Time.time;
			startAmount = player.GetNeed(need);
			endAmount = regenAmount + startAmount;

			if (need == PlayerController.Need.sleep) {
				player.animator.SetTrigger("Collapse");
				StartCoroutine(WakeAfterTime(freezeTime));
			} else if (need == PlayerController.Need.hunger) {
				player.animator.SetTrigger("Eat");
			} else {
				player.animator.SetTrigger("Interact");
			}
        }

    }

	IEnumerator WakeAfterTime(float time) {
		yield return StartCoroutine(WaitForRealSeconds(time));

		player.animator.SetTrigger("GetUp");
	}

	IEnumerator WaitForRealSeconds(float time) {
		float start = Time.realtimeSinceStartup;
		while (Time.realtimeSinceStartup < start + time)
			yield return null;
	}
}
