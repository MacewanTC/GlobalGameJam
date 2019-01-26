using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool finished = false;
    public string nextScene;

    public float timeOfDay = 0.3f;

    void Update()
    {
        timeOfDay += Time.deltaTime;
        if (timeOfDay >= 1.0f)
        {
            timeOfDay = 0.0f;
        }

        if (finished)
        {
            if (Input.GetButton("Fire1"))
            {
                SceneManager.LoadScene(nextScene);
            }
        }
    }

    public void EndGame()
    {
        finished = true;
		AudioController.instance.OnDeath(nextScene);//SceneManager.LoadScene(nextScene);
    }
}
