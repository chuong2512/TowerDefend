using MadLevelManager;
using TDTK;
using UnityEngine;

public class Menucontrol : MonoBehaviour
{
	public GameObject PauseCanvas;

	private bool _Pausegame;

	private void Start()
	{
	}

	private void Update()
	{
		if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
		{
			if (!GameControl.IsGamePaused())
			{
				PauseFc();
			}
			else if (GameControl.IsGamePaused())
			{
				ResumeFc();
			}
		}
	}

	public void OnPauseButton()
	{
	}

	public void PauseFc()
	{
		_GameState gameState = GameControl.GetGameState();
		if (gameState != _GameState.Over)
		{
			GameControl.PauseGame();
			PauseCanvas.SetActive(value: true);
		}
	}

	public void GoToFc(string LevelName)
	{
		MadLevel.LoadLevelByNameAsync(LevelName);
	}

	public void GiftPauseFc()
	{
		_GameState gameState = GameControl.GetGameState();
		if (gameState != _GameState.Over)
		{
			GameControl.PauseGame();
		}
	}

	public void ResumeFc()
	{
		GameControl.ResumeGame();
		PauseCanvas.SetActive(value: false);
	}

	public void RestartFc()
	{
		MadLevel.ReloadCurrentAsync();
	}

	public void NextLvFc()
	{
		if (MadLevel.HasNext())
		{
			string text = LoadingControl._LevelName = MadLevel.GetNextLevelName();
			Application.LoadLevelAdditiveAsync("Loading2");
		}
		else
		{
			LoadingControl._LevelName = "LevelSelect";
			Application.LoadLevelAdditiveAsync("Loading2");
		}
	}

	public void menuFc()
	{
		Time.timeScale = 1f;
		LoadingControl._LevelName = "LevelSelect";
		Application.LoadLevelAdditiveAsync("Loading2");
	}

	public void ShopBtn()
	{
		Time.timeScale = 1f;
		LoadingControl._LevelName = "ShopScreen";
		Application.LoadLevelAdditiveAsync("Loading2");
	}
}
