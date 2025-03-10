using UnityEngine;

public class RateScript : MonoBehaviour
{
	public GameObject Rate;

	public static bool InternetCheck;

	private void Awake()
	{
		if (ES2.Exists("Facebook"))
		{
			if (ES2.Load<int>("Rate") == 0)
			{
				Rate.SetActive(value: true);
			}
			else
			{
				Rate.SetActive(value: false);
			}
		}
		else
		{
			ES2.Save(0, "Facebook");
			ES2.Save(0, "Tweet");
			ES2.Save(0, "Rate");
			ES2.Save(0, "Download");
			Rate.SetActive(value: true);
		}
	}

	private void Start()
	{
	}

	public void FbBtn()
	{
		if (InternetCheck)
		{
			Application.OpenURL("https://www.facebook.com");
			ES2.Save(1, "Facebook");
			ES2.Save(ES2.Load<int>("Gear") + 100, "Gear");
		}
	}

	public void TwBtn()
	{
		if (InternetCheck)
		{
			Application.OpenURL("https://twitter.com");
			ES2.Save(1, "Tweet");
			ES2.Save(ES2.Load<int>("Gear") + 100, "Gear");
		}
	}

	public void RateBtn()
	{
		if (InternetCheck)
		{
			Application.OpenURL("https://play.google.com");
			ES2.Save(1, "Rate");
			Rate.SetActive(value: false);
		}
	}

	public void DownloadBtn()
	{
		if (InternetCheck)
		{
			Application.OpenURL("https://play.google.com");
			ES2.Save(1, "Download");
			ES2.Save(ES2.Load<int>("Gear") + 100, "Gear");
		}
	}

	private void Update()
	{
	}
}
