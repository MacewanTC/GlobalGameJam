using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TriggerRegenNeed : MonoBehaviour
{
    private PlayerController player;
    public float regenAmount = 0.5f;
    public float regenTime = 2.0f;
	public PlayerController.Need need = PlayerController.Need.hunger; // See player controller :: ChangeNeed for enum


    public Text text; 

    void Start()
    {
		player = FindObjectOfType<PlayerController>();
        text.enabled = false;
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
        if (Input.GetButton("Fire1"))
        {
            player.Freeze(regenTime);
            player.ChangeNeed(need, regenAmount);

			if (need == PlayerController.Need.sleep) {
				player.animator.SetTrigger("Collapse");
			} else if (need == PlayerController.Need.hunger) {
				player.animator.SetTrigger("Eat");
			} else {
				player.animator.SetTrigger("Interact");
			}
        }

    }
}
