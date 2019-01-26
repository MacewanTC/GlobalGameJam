using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TriggerRegenNeed : MonoBehaviour
{
    public PlayerController player;
    public float regenAmount = 0.5f;
    public float regenTime = 2.0f;
    public int need = 1; // See player controller :: ChangeNeed for enum


    public Text text; 

    void Start()
    {
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
        }

    }
}
