using SA.Common.Pattern;
using System;
using UnityEngine;

public class QuestAndEventsExample : MonoBehaviour
{
	public GameObject avatar;

	private Texture defaulttexture;

	public Texture2D pieIcon;

	public DefaultPreviewButton connectButton;

	public SA_Label playerLabel;

	public DefaultPreviewButton[] ConnectionDependedntButtons;

	private const string EVENT_ID = "CgkIipfs2qcGEAIQDQ";

	private const string QUEST_ID = "CgkIipfs2qcGEAIQDg";

	private void Start()
	{
		playerLabel.text = "Player Disconnected";
		defaulttexture = avatar.GetComponent<Renderer>().material.mainTexture;
		GooglePlayConnection.ActionPlayerConnected += OnPlayerConnected;
		GooglePlayConnection.ActionPlayerDisconnected += OnPlayerDisconnected;
		GooglePlayConnection.ActionConnectionResultReceived += OnConnectionResult;
		GooglePlayEvents instance = Singleton<GooglePlayEvents>.Instance;
		instance.OnEventsLoaded = (Action<GooglePlayResult>)Delegate.Combine(instance.OnEventsLoaded, new Action<GooglePlayResult>(OnEventsLoaded));
		GooglePlayQuests instance2 = Singleton<GooglePlayQuests>.Instance;
		instance2.OnQuestsAccepted = (Action<GP_QuestResult>)Delegate.Combine(instance2.OnQuestsAccepted, new Action<GP_QuestResult>(OnQuestsAccepted));
		GooglePlayQuests instance3 = Singleton<GooglePlayQuests>.Instance;
		instance3.OnQuestsCompleted = (Action<GP_QuestResult>)Delegate.Combine(instance3.OnQuestsCompleted, new Action<GP_QuestResult>(OnQuestsCompleted));
		GooglePlayQuests instance4 = Singleton<GooglePlayQuests>.Instance;
		instance4.OnQuestsLoaded = (Action<GP_QuestResult>)Delegate.Combine(instance4.OnQuestsLoaded, new Action<GP_QuestResult>(OnQuestsLoaded));
		if (GooglePlayConnection.State == GPConnectionState.STATE_CONNECTED)
		{
			OnPlayerConnected();
		}
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

	private void FixedUpdate()
	{
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

	public void LoadEvents()
	{
		Singleton<GooglePlayEvents>.Instance.LoadEvents();
	}

	public void IncrementEvent()
	{
		Singleton<GooglePlayEvents>.Instance.SumbitEvent("CgkIipfs2qcGEAIQDQ");
	}

	public void ShowAllQuests()
	{
		Singleton<GooglePlayQuests>.Instance.ShowQuests();
	}

	public void ShowAcceptedQuests()
	{
		Singleton<GooglePlayQuests>.Instance.ShowQuests(GP_QuestsSelect.SELECT_ACCEPTED);
	}

	public void ShowCompletedQuests()
	{
		Singleton<GooglePlayQuests>.Instance.ShowQuests(GP_QuestsSelect.SELECT_COMPLETED);
	}

	public void ShowOpenQuests()
	{
		Singleton<GooglePlayQuests>.Instance.ShowQuests(GP_QuestsSelect.SELECT_OPEN);
	}

	public void AcceptQuest()
	{
		Singleton<GooglePlayQuests>.Instance.AcceptQuest("CgkIipfs2qcGEAIQDg");
	}

	public void LoadQuests()
	{
		Singleton<GooglePlayQuests>.Instance.LoadQuests(GP_QuestSortOrder.SORT_ORDER_ENDING_SOON_FIRST);
	}

	private void OnEventsLoaded(GooglePlayResult result)
	{
		UnityEngine.Debug.Log("Total Events: " + Singleton<GooglePlayEvents>.Instance.Events.Count);
		AN_PoupsProxy.showMessage("Events Loaded", "Total Events: " + Singleton<GooglePlayEvents>.Instance.Events.Count);
		SA_StatusBar.text = "OnEventsLoaded:  " + result.Response.ToString();
		foreach (GP_Event @event in Singleton<GooglePlayEvents>.Instance.Events)
		{
			UnityEngine.Debug.Log(@event.Id);
			UnityEngine.Debug.Log(@event.Description);
			UnityEngine.Debug.Log(@event.FormattedValue);
			UnityEngine.Debug.Log(@event.Value);
			UnityEngine.Debug.Log(@event.IconImageUrl);
			UnityEngine.Debug.Log(@event.icon);
		}
	}

	private void OnQuestsAccepted(GP_QuestResult result)
	{
		AN_PoupsProxy.showMessage("On Quests Accepted", "Quests Accepted, ID: " + result.GetQuest().Id);
		SA_StatusBar.text = "OnQuestsAccepted:  " + result.Response.ToString();
	}

	private void OnQuestsCompleted(GP_QuestResult result)
	{
		UnityEngine.Debug.Log("Quests Completed, Reward: " + result.GetQuest().RewardData);
		AN_PoupsProxy.showMessage("On Quests Completed", "Quests Completed, Reward: " + result.GetQuest().RewardData);
		SA_StatusBar.text = "OnQuestsCompleted:  " + result.Response.ToString();
	}

	private void OnQuestsLoaded(GooglePlayResult result)
	{
		UnityEngine.Debug.Log("Total Quests: " + Singleton<GooglePlayQuests>.Instance.GetQuests().Count);
		AN_PoupsProxy.showMessage("Quests Loaded", "Total Quests: " + Singleton<GooglePlayQuests>.Instance.GetQuests().Count);
		SA_StatusBar.text = "OnQuestsLoaded:  " + result.Response.ToString();
		foreach (GP_Quest quest in Singleton<GooglePlayQuests>.Instance.GetQuests())
		{
			UnityEngine.Debug.Log(quest.Id);
		}
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
		SA_StatusBar.text = "ConnectionResul:  " + result.code.ToString();
		UnityEngine.Debug.Log(result.code.ToString());
	}

	private void OnDestroy()
	{
		GooglePlayConnection.ActionPlayerConnected -= OnPlayerConnected;
		GooglePlayConnection.ActionPlayerDisconnected -= OnPlayerDisconnected;
		GooglePlayConnection.ActionConnectionResultReceived -= OnConnectionResult;
	}
}
