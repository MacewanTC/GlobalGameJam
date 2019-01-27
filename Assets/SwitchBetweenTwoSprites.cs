using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchBetweenTwoSprites : MonoBehaviour
{
	public Sprite spr1, spr2;
	public float period = 1f;

	private SpriteRenderer sprRend;
    // Start is called before the first frame update
    void Start()
    {
		sprRend = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
		if ( Time.time % period > period/2) {
			sprRend.sprite = spr1;
		} else {
			sprRend.sprite = spr2;
		}
    }
}
