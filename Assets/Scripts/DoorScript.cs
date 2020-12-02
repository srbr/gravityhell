using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//ドアのスクリプト
public class DoorScript : MonoBehaviour
{
	public string nextScene; //移動先のシーン
	public float nextX, nextY; //移動先の座標
	private bool isTriggered = false; //接触中かどうか
	void Update()
	{
		if (isTriggered && Core.Instance.GetPlayer().controllable && !Core.Instance.IsFading() && Core.Instance.GetPlayer().isLanded && Input.GetKeyDown(KeyCode.UpArrow) && Mathf.Abs(Core.Instance.GetPlayer().way * 90f - transform.rotation.eulerAngles.z) < 45f)
		{
			Core.Instance.GetPlayer().controllable = false;
			Core.Instance.StartFadeOut(() =>
			{
				if (SceneManager.GetActiveScene().name != nextScene)
				{
					SceneManager.LoadScene(nextScene);
				}
				Core.Instance.GetPlayer().transform.position = new Vector3(nextX, nextY, Core.Instance.GetPlayer().transform.position.z);
				Core.Instance.StartFadeIn(() =>
				{
					Core.Instance.GetPlayer().controllable = true;
					Core.Instance.GetPlayer().jumpControllable = true;
				});
			});
		}
	}

	private void OnTriggerEnter2D(Collider2D c)
	{
		if (c.tag == "Player")
		{
			isTriggered = true;
			Core.Instance.GetPlayer().jumpControllable = false;
		}
	}

	private void OnTriggerStay2D(Collider2D c)
	{
		if (c.tag == "Player")
		{
			isTriggered = true;
			Core.Instance.GetPlayer().jumpControllable = false;
		}
	}
	private void OnTriggerExit2D(Collider2D c)
	{
		if (c.tag == "Player")
		{
			isTriggered = false;
			Core.Instance.GetPlayer().jumpControllable = true;
		}
	}
}
