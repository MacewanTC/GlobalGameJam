using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreCounter : MonoBehaviour
{
    public Text text;

    private float score = 0.0f;

    void Start()
    {
        
    }

    void Update()
    {
        score += Time.deltaTime;
        text.text = ((int)score).ToString();
    }
}
