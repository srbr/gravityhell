using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//汎用型のトリガースクリプト
//プレイヤー接触時に指定したフラグがtrueになる
public class TriggerScript : MonoBehaviour
{
	public int triggerId;
	private void OnTriggerEnter2D(Collider2D c)
	{
		if (c.tag == "Player")
		{
			Core.Instance.SetTrigger(triggerId, true);
		}
	}
	private void OnTriggerStay2D(Collider2D c)
	{
		if (c.tag == "Player")
		{
			Core.Instance.SetTrigger(triggerId, true);
		}
	}
	private void OnTriggerExit2D(Collider2D c)
	{
		if (c.tag == "Player")
		{
		Core.Instance.SetTrigger(triggerId, false);
		}
	}
}
