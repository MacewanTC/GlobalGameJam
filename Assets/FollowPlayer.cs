using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
	private PlayerController player;
	private float distanceFromPlayerLayer = 10, startZoomTime, zoomPeriod = 1f;
	private bool zoom = false;
    // Start is called before the first frame update
    void Start()
    {
		player = FindObjectOfType<PlayerController>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
		float z = -1*distanceFromPlayerLayer;

		if (zoom) Camera.main.orthographicSize = Mathf.Lerp(5, 1, (Time.time - startZoomTime)/zoomPeriod);

		transform.position = new Vector3(player.transform.position.x, player.transform.position.y, z);
    }

	public void StartZoom() {
		zoom = true;
		startZoomTime = Time.time;

	}
}
