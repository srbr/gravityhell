using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleBlockScript : MonoBehaviour
{
	public int flagId;
	public Sprite offSprite, onSprite;
	public bool inverted;
	private SpriteRenderer sr;
	private BoxCollider2D bc;
	void Start()
	{
		sr = GetComponent<SpriteRenderer>();
		bc = GetComponent<BoxCollider2D>();
	}

	void Update()
	{
		sr.sprite = (bc.enabled = Core.Instance.GetTrigger(flagId) != inverted) ? onSprite : offSprite;
	}
}
