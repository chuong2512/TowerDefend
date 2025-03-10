using MadLevelManager;
using UnityEngine;

public class PlaySeclect : MonoBehaviour
{
	private string LvNameIcon;

	public GameObject PlayBtnObj;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void OnIconActivate(MadLevelIcon Icon)
	{
		PlayBtnObj.SetActive(value: true);
		UnityEngine.Debug.Log(Icon.level.name);
		LvNameIcon = Icon.level.name;
	}

	public void PlayBtn()
	{
		LoadingControl._LevelName = LvNameIcon;
		Application.LoadLevelAdditiveAsync("Loading2");
	}
}
