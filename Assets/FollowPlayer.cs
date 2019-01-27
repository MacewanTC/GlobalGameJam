using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
	private PlayerController player;
	private float distanceFromPlayerLayer = 10;
    // Start is called before the first frame update
    void Start()
    {
		player = FindObjectOfType<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
		transform.position = new Vector3(player.transform.position.x, player.transform.position.y, -1*distanceFromPlayerLayer);
    }
}
