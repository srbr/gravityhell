using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Core : MonoBehaviour
{
	private static Core _core;
	public static Core Instance { get { return _core; } }
	public CanvasGroup fadeScreen;
	private Action fadedOutCallback, fadedInCallback;
	private bool fadingIn, fadingOut;
	private float fadeSpeed = 0.01f;
	private bool[] flags = new bool[65536]; //グローバルなフラグ(看板やギミック等で使用)
	private PlayerScript player;
	public void SetPlayer(PlayerScript a)
	{
		player = a;
	}
	public PlayerScript GetPlayer()
	{
		return player;
	}

	void Start()
	{
		//本体と関連オブジェクトをシーン遷移で消えないようにする
		DontDestroyOnLoad(gameObject);
		DontDestroyOnLoad(fadeScreen.gameObject);
	}
	void Update()
	{
		if (fadingOut) //フェードアウト処理
		{
			if (fadeScreen.alpha + fadeSpeed < 1f)
			{
				fadeScreen.alpha += fadeSpeed;
			}
			else
			{
				fadeScreen.alpha = 1f;
				fadingOut = false;
				if (fadedOutCallback != null)
				{
					fadedOutCallback();
					fadedOutCallback = null;
				}
			}
		}
		else if (fadingIn) //フェードイン処理
		{
			if (fadeScreen.alpha - fadeSpeed > 0f)
			{
				fadeScreen.alpha -= fadeSpeed;
			}
			else
			{
				fadeScreen.alpha = 0f;
				fadingIn = false;
				if (fadedInCallback != null)
				{
					fadedInCallback();
					fadedInCallback = null;
				}
			}
		}
	}
	public void SetTrigger(int id, bool flag)
	{
		flags[id] = flag;
	}
	public bool GetTrigger(int id)
	{
		return flags[id];
	}
	public bool ToggleTrigger(int id)
	{
		flags[id] = !flags[id];
		return flags[id];
	}
	public void StartFadeOut(Action e)
	{
		fadedOutCallback = e; //フェードアウト完了時のコールバック
		fadingOut = true;
	}
	public void StartFadeIn(Action e)
	{
		fadedInCallback = e; //フェードイン完了時のコールバック
		fadingIn = true;
	}
	public bool IsFading()
	{
		return fadingIn || fadingOut;
	}

	public bool IsFadingIn()
	{
		return fadingIn;
	}
	public bool IsFadingOut()
	{
		return fadingOut;
	}
	void Awake()
	{
		if (_core != null && _core != this)
		{
			Destroy(gameObject);
		}
		else
		{
			_core = this;
		}
	}

	void OnDestroy()
	{
		if (this == _core)
		{
			_core = null;
		}
	}
}
