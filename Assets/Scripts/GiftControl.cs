using SIS;
using System;
using UnityEngine;
using UnityEngine.UI;

public class GiftControl : MonoBehaviour
{
	public Text TitleGift;

	public Text NumgearTxt;

	public GameObject ErrorCanvas;

	private int NumGearGift;

	private int StartGearGift = 400;

	private DateTime _Lasttime;

	private DateTime _NowTime;

	private void Start()
	{
		CheckGift();
		CheckHack();
		UnityEngine.Debug.Log(NumGearGift);
		if (ES2.Load<bool>("KeyFirstPlay"))
		{
			if (NumGearGift > 0)
			{
				TitleGift.text = "Login Gift";
				NumgearTxt.text = NumGearGift.ToString();
			}
		}
		else
		{
			TitleGift.text = "Fist Gift";
			NumgearTxt.text = StartGearGift.ToString();
		}
	}

	public void CollectGift()
	{
		if (ES2.Load<bool>("KeyFirstPlay"))
		{
			if (NumGearGift >= 1)
			{
				DBManager.IncreaseFunds("coins", NumGearGift);
				ES2.Save(DateTime.Now, "KeyLastDay");
				TitleGift.text = "Collected!";
				CheckGift();
				NumGearGift = 0;
				NumgearTxt.text = NumGearGift.ToString();
			}
		}
		else
		{
			DBManager.IncreaseFunds("coins", StartGearGift);
			ES2.Save(DateTime.Now, "KeyLastDay");
			ES2.Save(param: true, "KeyFirstPlay");
			TitleGift.text = "Collected!";
			NumgearTxt.text = NumGearGift.ToString();
		}
	}

	public void CheckGift()
	{
		_Lasttime = ES2.Load<DateTime>("KeyLastDay");
		_NowTime = DateTime.Now;
		double totalMinutes = (_NowTime - _Lasttime).TotalMinutes;
		if (totalMinutes > 0.0)
		{
			NumGearGift = (int)totalMinutes / 2;
			if (NumGearGift >= 2000)
			{
				NumGearGift = 2000;
			}
			UnityEngine.Debug.Log("time ok!");
		}
		else
		{
			UnityEngine.Debug.Log("time error!");
			TitleGift.text = "Time Error!";
			NumgearTxt.text = "xxxxxx";
			ErrorCanvas.SetActive(value: true);
			ES2.Save(param: true, "KeyHack");
		}
	}

	public void ExitGame()
	{
		Application.Quit();
	}

	private void CheckHack()
	{
		if (ES2.Load<bool>("KeyHack"))
		{
			ErrorCanvas.SetActive(value: true);
			UnityEngine.Debug.Log("vao day");
		}
	}
}
