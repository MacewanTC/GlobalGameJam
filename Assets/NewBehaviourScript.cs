﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewBehaviourScript : MonoBehaviour
{
    public Slider slider;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(slider.value <= 0)
        {
            slider.value = 1.0f;
        }
        else
        {
            slider.value -= 0.01f;
        }
    }
}
