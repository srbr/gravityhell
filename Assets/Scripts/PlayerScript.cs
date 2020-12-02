using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//プレイヤーのスクリプト
public class PlayerScript : MonoBehaviour
{
	public float walkSpeed = 0.04f; //歩行速度
	public float jumpSpeed = 0.1f; //ジャンプ速度
	public int way = 0; //重力の方向(0=マイナスy方向,1=プラスx方向,2=プラスy方向,3=マイナスx方向)
	public bool controllable = true; //歩行操作を受け付けるかどうか
	public bool jumpControllable = true; //ジャンプ操作を受け付けるかどうか
	public bool justGravity = true; //重力が90°の倍数ならtrue、ローテーターを移動中はfalse
	public bool isLanded = false; //地面に立っているかどうか
	private float rotatingPosition; //重力回転中のポジション、ローテーターの左端を0.0、右端を1.0とする
	private float rotatingSize; //ローテーターの原点とプレイヤーの原点の距離を格納する
	private SpriteRenderer sr;
	private Animator a;
	private BoxCollider2D bc;
	private int jumpingCount = 0; //ジャンプ操作の残りフレーム数
	private static int jumpingCountMax = 10; //ジャンプ操作が可能な最大フレーム数
	private float vSpeed = 0; //上方向(重力の反対方向)の速度
	private RotatorScript rotator; //接触中のローテーター
	private bool jumpStart = true; //ジャンプキーが押された瞬間かどうか
	public static Vector2[] gravities = { Vector2.down, Vector2.right, Vector2.up, Vector2.left }; //wayからVector2定数への変換用
	void Start()
	{
		if (Core.Instance.GetPlayer() == null)
		{
			sr = GetComponent<SpriteRenderer>();
			a = GetComponent<Animator>();
			bc = GetComponent<BoxCollider2D>();
			Core.Instance.SetPlayer(this);
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}
	}

	void FixedUpdate()
	{
		Vector2 rotatedOrigin = bc.transform.position;
		switch (way) //判定用に重力方向を加味した原点を計算
		{
			case 0:
				rotatedOrigin += new Vector2(bc.offset.x, bc.offset.y);
				break;
			case 1:
				rotatedOrigin += new Vector2(-bc.offset.y, bc.offset.x);
				break;
			case 2:
				rotatedOrigin += new Vector2(-bc.offset.x, -bc.offset.y);
				break;
			case 3:
				rotatedOrigin += new Vector2(bc.offset.y, -bc.offset.x);
				break;
		}
		//地面に触れているかどうかの判定
		RaycastHit2D hitBottom = Physics2D.BoxCast(rotatedOrigin, new Vector2(bc.size.x - 0.05f, bc.size.y - 0.01f), 90f * way, gravities[way], 0.02f, 1 << 8);
		isLanded = hitBottom;
		if ((isLanded && hitBottom.distance + vSpeed < 0.04f) || !justGravity)
		{
			vSpeed = 0;
		}
		else
		{
			vSpeed -= 0.008f;
			if (vSpeed < -0.2f)
			{
				vSpeed = -0.2f;
			}
		}
		//天井に触れているかどうかの判定
		RaycastHit2D hitTop = Physics2D.BoxCast(rotatedOrigin, new Vector2(bc.size.x - 0.05f, bc.size.y - 0.01f), 90f * way, gravities[(way + 2) % 4], 0.015f, 1 << 8);
		if (hitTop && vSpeed > 0)
		{
			vSpeed = 0;
			jumpingCount = 0;
		}
		//プレイヤー操作
		if (controllable)
		{
			if (Input.anyKey)
			{
				if (Input.GetKey(KeyCode.LeftArrow))
				{
					sr.flipX = true;
					Control(false);
				}
				else if (Input.GetKey(KeyCode.RightArrow))
				{
					sr.flipX = false;
					Control(true);
				}
				if (Input.GetKey(KeyCode.UpArrow))
				{
					if (jumpControllable)
					{
						ControlJump(jumpStart);
					}
					jumpStart = false;
				}
			}
		}
		if (!Input.GetKey(KeyCode.UpArrow))
		{
			jumpingCount = 0;
			jumpStart = true;
		}
		if (justGravity)
		{
			transform.Translate(0, vSpeed, 0);
		}
		//アニメーション用の変数を反映
		a.SetFloat("VSpeed", vSpeed);
		a.SetBool("IsLanded", isLanded);
	}

	private void Control(bool right) //歩行操作
	{
		if (!justGravity)
		{
			rotatingPosition += right ? walkSpeed : -walkSpeed;
			float theta = (rotator.way + rotatingPosition) / 2 * Mathf.PI;
			transform.position = new Vector3(rotator.GetOriginX() + Mathf.Cos(theta) * rotatingSize, rotator.GetOriginY() + Mathf.Sin(theta) * rotatingSize);
			transform.rotation = Quaternion.Euler(0f, 0f, (rotatingPosition + rotator.way + 1f) * 90f);
			if (rotatingPosition >= 1f)
			{
				way = rotator.GetWayR();
				transform.rotation = Quaternion.Euler(0f, 0f, 90f * way);
				UnsetRotator();
			}
			if (rotatingPosition <= 0f)
			{
				way = rotator.GetWayL();
				transform.rotation = Quaternion.Euler(0f, 0f, 90f * way);
				UnsetRotator();
			}
		}
		if (justGravity)
		{
			transform.Translate(right ? walkSpeed : -walkSpeed, 0, 0);
		}
	}
	private void ControlJump(bool start) //ジャンプ操作
	{
		if (justGravity)
		{
			if ((start && isLanded) || (!start && jumpingCount > 0))
			{
				if (start) jumpingCount = jumpingCountMax;
				vSpeed = jumpSpeed;
				--jumpingCount;
			}
		}
		else
		{
			if (start)
			{
				jumpingCount = jumpingCountMax;
				vSpeed = jumpSpeed;
				way = rotatingPosition < 0.5f ? rotator.GetWayL() : rotator.GetWayR();
				transform.rotation = Quaternion.Euler(0f, 0f, 90f * way);
				UnsetRotator();
			}
		}
	}

	public void SetRotator(RotatorScript r) //ローテーターをセット
	{
		rotator = r;
		justGravity = false;
		rotatingSize = way % 2 == 0 ? Mathf.Abs(rotator.GetOriginY() - transform.position.y) : Mathf.Abs(rotator.GetOriginX() - transform.position.x);
		rotatingPosition = rotator.GetWayL() == way ? 0f : 1f;
		way = rotator.GetWayL();
	}

	public void UnsetRotator() //ローテーターをアンセット
	{
		rotator = null;
		justGravity = true;
	}
}
