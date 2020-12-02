using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//看板に接触した時に表示されるメッセージのスクリプト
public class SignBoardScript : MonoBehaviour
{

	public int triggerId;
	private CanvasGroup cg;
	private float fadeSpeed = 0.04f;
	void Start()
	{
		cg = GetComponent<CanvasGroup>();
		cg.alpha = 0;
	}

	void Update()
	{
		if (Core.Instance.GetTrigger(triggerId))
		{
			if (cg.alpha + fadeSpeed < 1f) cg.alpha += fadeSpeed;
			else cg.alpha = 1f;
		}
		else
		{
			if (cg.alpha - fadeSpeed > 0f) cg.alpha -= fadeSpeed;
			else cg.alpha = 0f;
		}
	}
}
