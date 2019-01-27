using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeRoom : MonoBehaviour
{
	public AudioController.PlayerState locationMusic = AudioController.PlayerState.SAFE;

	void OnTriggerEnter2D(Collider2D collider) {
		AudioController.instance.defaultLocation = locationMusic;
	}

	void OnTriggerExit2D(Collider2D collider) {
		AudioController.instance.defaultLocation = AudioController.PlayerState.EXPLORE;
	}
}
