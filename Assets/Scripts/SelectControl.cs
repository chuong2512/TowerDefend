using MadLevelManager;
using UnityEngine;

public class SelectControl : MonoBehaviour
{
	public GameObject _BtnStart;

	public GameObject _BtnEndless;

	private string EndlessLvName;

	public string NameLevel;

	public string NameLevel2;

	public void Awake()
	{
		_BtnStart.SetActive(value: false);
	}

	public void OnIconActivate(MadLevelIcon icon)
	{
		LoadingControl._LevelName = icon.level.name;
		Application.LoadLevelAdditiveAsync("Loading2");
	}

	public void GetMessage(MadLevelIcon iconName)
	{
		NameLevel = iconName.level.name;
		NameLevel2 = iconName.level.name;
		_BtnEndless.SetActive(value: false);
		switch (NameLevel)
		{
		case "Level 5":
			_BtnStart.SetActive(value: true);
			_BtnEndless.SetActive(value: true);
			EndlessLvName = "Level 05 EndLess";
			LoadingControl._LevelName = EndlessLvName;
			UnityEngine.Debug.Log("5");
			break;
		case "Level 10":
			_BtnStart.SetActive(value: true);
			_BtnEndless.SetActive(value: true);
			EndlessLvName = "Level 10 EndLess";
			LoadingControl._LevelName = EndlessLvName;
			UnityEngine.Debug.Log("10");
			break;
		case "Level 15":
			_BtnStart.SetActive(value: true);
			_BtnEndless.SetActive(value: true);
			EndlessLvName = "Level 15 EndLess";
			LoadingControl._LevelName = EndlessLvName;
			UnityEngine.Debug.Log("15");
			break;
		case "Level 20":
			_BtnStart.SetActive(value: true);
			_BtnEndless.SetActive(value: true);
			EndlessLvName = "Level 20 EndLess";
			LoadingControl._LevelName = EndlessLvName;
			UnityEngine.Debug.Log("20");
			break;
		case "Level 25":
			_BtnStart.SetActive(value: true);
			_BtnEndless.SetActive(value: true);
			EndlessLvName = "Level 25 EndLess";
			LoadingControl._LevelName = EndlessLvName;
			UnityEngine.Debug.Log("25");
			break;
		case "Level 30":
			_BtnStart.SetActive(value: true);
			_BtnEndless.SetActive(value: true);
			EndlessLvName = "Level 30 EndLess";
			LoadingControl._LevelName = EndlessLvName;
			UnityEngine.Debug.Log("30");
			break;
		case "Level 35":
			_BtnStart.SetActive(value: true);
			_BtnEndless.SetActive(value: true);
			EndlessLvName = "Level 35 EndLess";
			LoadingControl._LevelName = EndlessLvName;
			UnityEngine.Debug.Log("35");
			break;
		case "Level 40":
			_BtnStart.SetActive(value: true);
			_BtnEndless.SetActive(value: true);
			EndlessLvName = "Level 40 EndLess";
			LoadingControl._LevelName = EndlessLvName;
			UnityEngine.Debug.Log("40");
			break;
		}
		UnityEngine.Debug.Log(NameLevel);
		_BtnStart.SetActive(value: true);
	}

	public void StartBtn()
	{
		LoadingControl._LevelName = NameLevel2;
		Application.LoadLevelAdditiveAsync("Loading2");
	}

	public void Endlessbtn()
	{
		LoadingControl._LevelName = EndlessLvName;
		UnityEngine.Debug.Log(LoadingControl._LevelName);
		Application.LoadLevelAdditiveAsync("Loading2");
	}
}
