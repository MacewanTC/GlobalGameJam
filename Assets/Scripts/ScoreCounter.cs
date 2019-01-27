using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreCounter : MonoBehaviour
{
    private Text text;

    private float score = 0.0f;

    void Start()
    {
        text = gameObject.GetComponent(typeof(Text)) as Text;
    }

    void Update()
    {
        score += Time.deltaTime;
        text.text = ((int)(score)).ToString() + " Seconds";
    }
}
