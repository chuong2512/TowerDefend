using UnityEngine;

public class MenuScript : MonoBehaviour
{
	public GameObject ExitCanvas;

	public GameObject BtnPlay;

	public GameObject BtnEndless;

	public GameObject DimScreen;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void GotoScript(string ScreenName)
	{
		LoadingControl._LevelName = ScreenName;
		Application.LoadLevelAdditiveAsync("Loading2");
	}

	public void Load34(string ScreenName)
	{
		LoadingControl._LevelName = ScreenName;
		Application.LoadLevelAdditiveAsync("Loading2");
	}

	public void EnableExit()
	{
		ExitCanvas.SetActive(value: true);
	}

	public void ExitBtn()
	{
		Application.Quit();
	}

	public void GetGeat()
	{
		ES2.Save(500000, "Gear");
	}

	public void CheckNewBie()
	{
	}

	public void RateApp()
	{
		Application.OpenURL("https://play.google.com");
	}
}
