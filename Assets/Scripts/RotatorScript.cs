using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ローテーター(歩行すると重力方向が変わる坂)のスクリプト
public class RotatorScript : MonoBehaviour
{
	[Range(0, 3)]
	public int way;
	private int wayL, wayR;
	private float originX, originY;
    void Start()
    {
		originX = way == 0 || way == 3 ? -0.5f : 0.5f;
		originY = way == 0 || way == 1 ? -0.5f : 0.5f;
		wayL = (way + 1) % 4;
		wayR = (way + 2) % 4;
    }
	private void OnTriggerStay2D(Collider2D c)
	{
		if (c.tag == "Player")
		{
			PlayerScript player = Core.Instance.GetPlayer();
			if (player.justGravity && player.isLanded && (player.way == wayL || player.way == wayR) && transform.position.x - 0.5f < player.transform.position.x && player.transform.position.x < transform.position.x + 0.5f && transform.position.y - 0.5f < player.transform.position.y && player.transform.position.y < transform.position.y + 0.5f)
			{
				player.SetRotator(this);
			}
		}
	}
	private void OnTriggerExit2D(Collider2D c)
	{
		if (c.tag == "Player")
		{
			Core.Instance.GetPlayer().UnsetRotator();
		}
	}

	public float GetOriginX()
	{
		return transform.position.x + originX;
	}
	public float GetOriginY()
	{
		return transform.position.y + originY;
	}
	public int GetWayL()
	{
		return wayL;
	}
	public int GetWayR()
	{
		return wayR;
	}
}
