using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public float hunger = 1.0f;
    public float sleep = 1.0f;
    public float hungerDecay =  0.01f;
    public float sleepDecay = 0.01f;
    public Slider hungerSlider;
    public Slider sleepSlider;


    public float speed = 3.0f;
    public float altSpeed = 6.0f;

    private Rigidbody2D body;

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        ChangeNeeds(1, hungerDecay * Time.deltaTime);
        ChangeNeeds(2, sleepDecay * Time.deltaTime);
    }

    // enum need { hunger = 1, sleep = 2}
    void ChangeNeeds(int need, float delta) 
    {
        if (need == 1)
        {
            hunger -= delta;
            hungerSlider.value = hunger;
        }
        else if (need == 2)
        {
            sleep -= delta;
            sleepSlider.value = sleep;
        }
    }

    void FixedUpdate()
    {
        Vector2 moveDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if (true) // TODO: if not altspeed
        {
            body.MovePosition((Vector2)transform.position + (moveDirection.normalized * speed * Time.deltaTime));
        }
        else
        {
            body.MovePosition((Vector2)transform.position + (moveDirection.normalized * altSpeed * Time.deltaTime));
        }
    }
}
