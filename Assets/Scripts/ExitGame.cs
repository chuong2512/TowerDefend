using MadLevelManager;
using UnityEngine;

public class ExitGame : MonoBehaviour
{
	public GameObject ExitPanel;

	public GameObject[] ListPanel;

	private bool popup;

	public bool _Back;

	public string _BackLvName;

	private void Start()
	{
		if (_BackLvName == null)
		{
			_BackLvName = "Start";
		}
	}

	private void Update()
	{
		if (!Input.GetKeyDown(KeyCode.Escape))
		{
			return;
		}
		if (CheckActive())
		{
			for (int i = 0; i < ListPanel.Length; i++)
			{
				ListPanel[i].SetActive(value: false);
			}
		}
		else if (_Back)
		{
			if (MadLevel.currentLevelName == "Shop")
			{
				MadLevel.LoadLevelByNameAsync("Start");
				return;
			}
			LoadingControl._LevelName = _BackLvName;
			Application.LoadLevelAdditiveAsync("Loading2");
		}
		else if ((bool)ExitPanel)
		{
			ExitPanel.SetActive(value: true);
		}
	}

	public void ExitFc()
	{
		Application.Quit();
	}

	private bool CheckActive()
	{
		for (int i = 0; i < ListPanel.Length; i++)
		{
			if (ListPanel[i].activeInHierarchy)
			{
				return true;
			}
		}
		return false;
	}
}
