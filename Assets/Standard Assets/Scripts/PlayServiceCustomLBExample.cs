using SA.Common.Pattern;
using System.Collections.Generic;
using UnityEngine;

public class PlayServiceCustomLBExample : MonoBehaviour
{
	private const string LEADERBOARD_ID = "CgkIipfs2qcGEAIQAA";

	public GameObject avatar;

	private Texture defaulttexture;

	public DefaultPreviewButton connectButton;

	public SA_Label playerLabel;

	public DefaultPreviewButton GlobalButton;

	public DefaultPreviewButton LocalButton;

	public DefaultPreviewButton AllTimeButton;

	public DefaultPreviewButton WeekButton;

	public DefaultPreviewButton TodayButton;

	public DefaultPreviewButton SubmitScoreButton;

	public DefaultPreviewButton[] ConnectionDependedntButtons;

	public CustomLeaderboardFiledsHolder[] lines;

	private GPLeaderBoard loadedLeaderBoard;

	private GPCollectionType displayCollection = GPCollectionType.FRIENDS;

	private GPBoardTimeSpan displayTime = GPBoardTimeSpan.ALL_TIME;

	private int score = 100;

	private void Start()
	{
		playerLabel.text = "Player Disconnected";
		defaulttexture = avatar.GetComponent<Renderer>().material.mainTexture;
		SA_StatusBar.text = "Custom Leader-board example scene loaded";
		CustomLeaderboardFiledsHolder[] array = lines;
		foreach (CustomLeaderboardFiledsHolder customLeaderboardFiledsHolder in array)
		{
			customLeaderboardFiledsHolder.Disable();
		}
		GooglePlayConnection.ActionPlayerConnected += OnPlayerConnected;
		GooglePlayConnection.ActionPlayerDisconnected += OnPlayerDisconnected;
		GooglePlayConnection.ActionConnectionResultReceived += OnConnectionResult;
		GooglePlayManager.ActionScoreSubmited += OnScoreSbumitted;
		GooglePlayManager.ActionScoresListLoaded += ActionScoreRequestReceived;
		if (GooglePlayConnection.State == GPConnectionState.STATE_CONNECTED)
		{
			OnPlayerConnected();
		}
	}

	public void LoadScore()
	{
		Singleton<GooglePlayManager>.Instance.LoadPlayerCenteredScores("CgkIipfs2qcGEAIQAA", displayTime, displayCollection, 10);
	}

	public void OpenUI()
	{
		Singleton<GooglePlayManager>.Instance.ShowLeaderBoardById("CgkIipfs2qcGEAIQAA");
	}

	public void ShowGlobal()
	{
		displayCollection = GPCollectionType.GLOBAL;
		UpdateScoresDisaplay();
	}

	public void ShowLocal()
	{
		displayCollection = GPCollectionType.FRIENDS;
		UpdateScoresDisaplay();
	}

	public void ShowAllTime()
	{
		displayTime = GPBoardTimeSpan.ALL_TIME;
		UpdateScoresDisaplay();
	}

	public void ShowWeek()
	{
		displayTime = GPBoardTimeSpan.WEEK;
		UpdateScoresDisaplay();
	}

	public void ShowDay()
	{
		displayTime = GPBoardTimeSpan.TODAY;
		UpdateScoresDisaplay();
	}

	private void ConncetButtonPress()
	{
		UnityEngine.Debug.Log("GooglePlayManager State  -> " + GooglePlayConnection.State.ToString());
		if (GooglePlayConnection.State == GPConnectionState.STATE_CONNECTED)
		{
			SA_StatusBar.text = "Disconnecting from Play Service...";
			Singleton<GooglePlayConnection>.Instance.Disconnect();
		}
		else
		{
			SA_StatusBar.text = "Connecting to Play Service...";
			Singleton<GooglePlayConnection>.Instance.Connect();
		}
	}

	private void UpdateScoresDisaplay()
	{
		if (loadedLeaderBoard != null)
		{
			GPScore currentPlayerScore = loadedLeaderBoard.GetCurrentPlayerScore(displayTime, displayCollection);
			int i;
			if (currentPlayerScore == null)
			{
				i = 1;
			}
			else
			{
				for (i = Mathf.Clamp(currentPlayerScore.Rank - 5, 1, currentPlayerScore.Rank); loadedLeaderBoard.GetScore(i, displayTime, displayCollection) == null; i++)
				{
				}
			}
			UnityEngine.Debug.Log("Start Display at rank: " + i);
			int num = i;
			CustomLeaderboardFiledsHolder[] array = lines;
			foreach (CustomLeaderboardFiledsHolder customLeaderboardFiledsHolder in array)
			{
				customLeaderboardFiledsHolder.Disable();
				GPScore gPScore = loadedLeaderBoard.GetScore(num, displayTime, displayCollection);
				if (gPScore != null)
				{
					customLeaderboardFiledsHolder.rank.text = num.ToString();
					customLeaderboardFiledsHolder.score.text = gPScore.LongScore.ToString();
					customLeaderboardFiledsHolder.playerId.text = gPScore.PlayerId;
					GooglePlayerTemplate playerById = Singleton<GooglePlayManager>.Instance.GetPlayerById(gPScore.PlayerId);
					if (playerById != null)
					{
						customLeaderboardFiledsHolder.playerName.text = playerById.name;
						if (playerById.hasIconImage)
						{
							customLeaderboardFiledsHolder.avatar.GetComponent<Renderer>().material.mainTexture = playerById.icon;
						}
						else
						{
							customLeaderboardFiledsHolder.avatar.GetComponent<Renderer>().material.mainTexture = defaulttexture;
						}
					}
					else
					{
						customLeaderboardFiledsHolder.playerName.text = "--";
						customLeaderboardFiledsHolder.avatar.GetComponent<Renderer>().material.mainTexture = defaulttexture;
					}
					customLeaderboardFiledsHolder.avatar.GetComponent<Renderer>().enabled = true;
				}
				else
				{
					customLeaderboardFiledsHolder.Disable();
				}
				num++;
			}
		}
		else
		{
			CustomLeaderboardFiledsHolder[] array2 = lines;
			foreach (CustomLeaderboardFiledsHolder customLeaderboardFiledsHolder2 in array2)
			{
				customLeaderboardFiledsHolder2.Disable();
			}
		}
	}

	private void FixedUpdate()
	{
		SubmitScoreButton.text = "Submit Score: " + score;
		if (GooglePlayConnection.State == GPConnectionState.STATE_CONNECTED)
		{
			if (Singleton<GooglePlayManager>.Instance.player.icon != null)
			{
				avatar.GetComponent<Renderer>().material.mainTexture = Singleton<GooglePlayManager>.Instance.player.icon;
			}
		}
		else
		{
			avatar.GetComponent<Renderer>().material.mainTexture = defaulttexture;
		}
		string text = "Connect";
		if (GooglePlayConnection.State == GPConnectionState.STATE_CONNECTED)
		{
			text = "Disconnect";
			DefaultPreviewButton[] connectionDependedntButtons = ConnectionDependedntButtons;
			foreach (DefaultPreviewButton defaultPreviewButton in connectionDependedntButtons)
			{
				defaultPreviewButton.EnabledButton();
			}
			AllTimeButton.Unselect();
			WeekButton.Unselect();
			TodayButton.Unselect();
			switch (displayTime)
			{
			case GPBoardTimeSpan.ALL_TIME:
				AllTimeButton.Select();
				break;
			case GPBoardTimeSpan.WEEK:
				WeekButton.Select();
				break;
			case GPBoardTimeSpan.TODAY:
				TodayButton.Select();
				break;
			}
			GlobalButton.Unselect();
			LocalButton.Unselect();
			switch (displayCollection)
			{
			case GPCollectionType.GLOBAL:
				GlobalButton.Select();
				break;
			case GPCollectionType.FRIENDS:
				LocalButton.Select();
				break;
			}
		}
		else
		{
			DefaultPreviewButton[] connectionDependedntButtons2 = ConnectionDependedntButtons;
			foreach (DefaultPreviewButton defaultPreviewButton2 in connectionDependedntButtons2)
			{
				defaultPreviewButton2.DisabledButton();
			}
			text = ((GooglePlayConnection.State != GPConnectionState.STATE_DISCONNECTED && GooglePlayConnection.State != 0) ? "Connecting.." : "Connect");
		}
		connectButton.text = text;
	}

	private void SubmitScore()
	{
		Singleton<GooglePlayManager>.Instance.SubmitScoreById("CgkIipfs2qcGEAIQAA", score);
		SA_StatusBar.text = "Submitiong score: " + (score + 1).ToString();
		score++;
	}

	private void OnPlayerDisconnected()
	{
		SA_StatusBar.text = "Player Disconnected";
		playerLabel.text = "Player Disconnected";
	}

	private void OnPlayerConnected()
	{
		SA_StatusBar.text = "Player Connected";
		playerLabel.text = Singleton<GooglePlayManager>.Instance.player.name;
	}

	private void OnConnectionResult(GooglePlayConnectionResult result)
	{
		SA_StatusBar.text = "Connection Resul:  " + result.code.ToString();
		UnityEngine.Debug.Log(result.code.ToString());
	}

	private void ActionScoreRequestReceived(GooglePlayResult obj)
	{
		SA_StatusBar.text = "Scores Load Finished";
		loadedLeaderBoard = Singleton<GooglePlayManager>.Instance.GetLeaderBoard("CgkIipfs2qcGEAIQAA");
		if (loadedLeaderBoard == null)
		{
			UnityEngine.Debug.Log("No Leaderboard found");
			return;
		}
		List<GPScore> scoresList = loadedLeaderBoard.GetScoresList(GPBoardTimeSpan.ALL_TIME, GPCollectionType.GLOBAL);
		foreach (GPScore item in scoresList)
		{
			UnityEngine.Debug.Log("OnScoreUpdated " + item.Rank + " " + item.PlayerId + " " + item.LongScore);
		}
		GPScore currentPlayerScore = loadedLeaderBoard.GetCurrentPlayerScore(displayTime, displayCollection);
		UnityEngine.Debug.Log("currentPlayerScore: " + currentPlayerScore.LongScore + " rank:" + currentPlayerScore.Rank);
		UpdateScoresDisaplay();
	}

	private void OnScoreSbumitted(GP_LeaderboardResult result)
	{
		SA_StatusBar.text = "Score Submit Resul:  " + result.Message;
		LoadScore();
	}

	private void OnDestroy()
	{
		GooglePlayConnection.ActionPlayerConnected += OnPlayerConnected;
		GooglePlayConnection.ActionPlayerDisconnected += OnPlayerDisconnected;
		GooglePlayConnection.ActionConnectionResultReceived += OnConnectionResult;
		GooglePlayManager.ActionScoreSubmited -= OnScoreSbumitted;
		GooglePlayManager.ActionScoresListLoaded -= ActionScoreRequestReceived;
	}
}
