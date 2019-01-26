﻿using System.Collections;
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

    public GameManager gameManager;

    private Rigidbody2D body;

    private float frozenTime;
    private bool isFrozen = false; 

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (!isFrozen)
        {
            ChangeNeed(1, -hungerDecay * Time.deltaTime);
            ChangeNeed(2, -sleepDecay * Time.deltaTime);
            ChangeNeed(3, -hygieneDecay * Time.deltaTime);
        }
        if (sleep == 0.0f)
        {
            Freeze(passOutTime);
            ChangeNeed(2, passOutRegen);
        }
        
        if (hunger == 0.0f)
        {
            gameManager.EndGame();
        }
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
    public void ChangeNeed(int need, float delta) 
    {
        if (need == 1)
        {
            hunger += delta;
            hunger = Mathf.Clamp(hunger, 0.0f, 1.0f);
            hungerSlider.value = hunger;
        }
        else if (need == 2)
        {
            sleep += delta;
            sleep = Mathf.Clamp(sleep, 0.0f, 1.0f);
            sleepSlider.value = sleep;
        }
        else if (need == 3)
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
}
