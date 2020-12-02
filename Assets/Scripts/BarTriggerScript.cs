using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarTriggerScript : MonoBehaviour
{
	public int flagId;
	public Sprite offSprite, onSprite;
	private SpriteRenderer sr;
	void Start()
	{
		sr = GetComponent<SpriteRenderer>();
	}

	void Update()
	{
		sr.sprite = Core.Instance.GetTrigger(flagId) ? onSprite : offSprite;
	}
	private void OnTriggerEnter2D(Collider2D c)
	{
		if (c.tag == "Player")
		{
			Core.Instance.ToggleTrigger(flagId);
		}
	}
}
