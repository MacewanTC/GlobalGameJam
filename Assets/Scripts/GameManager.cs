using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool finished = false;
    public string nextScene;

    private float startTime;

    void Start()
    {
        startTime = Time.time;
    }

    void Update()
    {
        if (finished)
        {
            if (Input.GetButton("Fire1"))
            {
				EndGame();
            }
        }
    }

    public float GetStartTime()
    {
        return startTime;
    }

    public void EndGame()
    {
        finished = true;
		AudioController.instance.OnDeath(nextScene);//SceneManager.LoadScene(nextScene);
    }
}
