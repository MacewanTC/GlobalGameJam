using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool finished = false;
    public string nextScene;

    public float timeOfDay = 0.3f;

    void Start()
    {
        Time.timeScale = 1;
    }

    void Update()
    {
        timeOfDay += Time.deltaTime;
        if (timeOfDay >= 1.0f) timeOfDay = 0.0f;

        if (finished && Input.GetButton("Fire1")) SceneManager.LoadScene(nextScene);
    }

    public void EndGame()
    {
        if (finished != true)
		{
            //Time.timeScale = 0;

			FindObjectOfType<PlayerController>().animator.SetTrigger("Death");
			FindObjectOfType<FollowPlayer>().StartZoom();
            AudioController.instance.OnDeath(nextScene);//SceneManager.LoadScene(nextScene);
        }

		finished = true;
    }
}
