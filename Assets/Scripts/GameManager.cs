using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool finished = false;
    public string nextScene;

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

    public void EndGame()
    {
        finished = true;
        SceneManager.LoadScene(nextScene);
    }
}
