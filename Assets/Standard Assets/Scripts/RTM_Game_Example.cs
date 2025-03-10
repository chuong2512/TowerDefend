using SA.Common.Pattern;
using System;
using System.Text;
using UnityEngine;

public class RTM_Game_Example : AndroidNativeExampleBase
{
	public GameObject avatar;

	public GameObject hi;

	public SA_Label playerLabel;

	public SA_Label gameState;

	public SA_Label parisipants;

	public DefaultPreviewButton connectButton;

	public DefaultPreviewButton helloButton;

	public DefaultPreviewButton leaveRoomButton;

	public DefaultPreviewButton showRoomButton;

	public DefaultPreviewButton[] ConnectionDependedntButtons;

	public SA_PartisipantUI[] patricipants;

	public SA_FriendUI[] friends;

	private Texture defaulttexture;

	private string inviteId;

	private void Start()
	{
		playerLabel.text = "Player Disconnected";
		defaulttexture = avatar.GetComponent<Renderer>().material.mainTexture;
		GooglePlayInvitationManager.ActionInvitationReceived += OnInvite;
		GooglePlayInvitationManager.ActionInvitationAccepted += ActionInvitationAccepted;
		GooglePlayRTM.ActionRoomCreated = (Action<GP_GamesStatusCodes>)Delegate.Combine(GooglePlayRTM.ActionRoomCreated, new Action<GP_GamesStatusCodes>(OnRoomCreated));
		GooglePlayConnection.ActionPlayerConnected += OnPlayerConnected;
		GooglePlayConnection.ActionPlayerDisconnected += OnPlayerDisconnected;
		GooglePlayConnection.ActionConnectionResultReceived += OnConnectionResult;
		if (GooglePlayConnection.State == GPConnectionState.STATE_CONNECTED)
		{
			OnPlayerConnected();
		}
		GooglePlayRTM.ActionDataRecieved = (Action<GP_RTM_Network_Package>)Delegate.Combine(GooglePlayRTM.ActionDataRecieved, new Action<GP_RTM_Network_Package>(OnGCDataReceived));
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

	private void ShowWatingRoom()
	{
		Singleton<GooglePlayRTM>.Instance.ShowWaitingRoomIntent();
	}

	private void findMatch()
	{
		int minPlayers = 1;
		int maxPlayers = 2;
		Singleton<GooglePlayRTM>.Instance.FindMatch(minPlayers, maxPlayers);
	}

	private void InviteFriends()
	{
		int minPlayers = 1;
		int maxPlayers = 2;
		Singleton<GooglePlayRTM>.Instance.OpenInvitationBoxUI(minPlayers, maxPlayers);
	}

	private void SendHello()
	{
		string s = "hello world";
		UTF8Encoding uTF8Encoding = new UTF8Encoding();
		byte[] bytes = uTF8Encoding.GetBytes(s);
		Singleton<GooglePlayRTM>.Instance.SendDataToAll(bytes, GP_RTM_PackageType.RELIABLE);
	}

	private void LeaveRoomInstance()
	{
		Singleton<GooglePlayRTM>.Instance.LeaveRoom();
	}

	private void DrawParticipants()
	{
		UpdateGameState("Room State: " + Singleton<GooglePlayRTM>.Instance.currentRoom.status.ToString());
		parisipants.text = "Total Room Participants: " + Singleton<GooglePlayRTM>.Instance.currentRoom.participants.Count;
		SA_PartisipantUI[] array = patricipants;
		foreach (SA_PartisipantUI sA_PartisipantUI in array)
		{
			sA_PartisipantUI.gameObject.SetActive(value: false);
		}
		int num = 0;
		foreach (GP_Participant participant in Singleton<GooglePlayRTM>.Instance.currentRoom.participants)
		{
			patricipants[num].gameObject.SetActive(value: true);
			patricipants[num].SetParticipant(participant);
			num++;
		}
	}

	private void UpdateGameState(string msg)
	{
		gameState.text = msg;
	}

	private void FixedUpdate()
	{
		DrawParticipants();
		if (Singleton<GooglePlayRTM>.Instance.currentRoom.status != GP_RTM_RoomStatus.ROOM_VARIANT_DEFAULT && Singleton<GooglePlayRTM>.Instance.currentRoom.status != GP_RTM_RoomStatus.ROOM_STATUS_ACTIVE)
		{
			showRoomButton.EnabledButton();
		}
		else
		{
			showRoomButton.DisabledButton();
		}
		if (Singleton<GooglePlayRTM>.Instance.currentRoom.status == GP_RTM_RoomStatus.ROOM_VARIANT_DEFAULT)
		{
			leaveRoomButton.DisabledButton();
		}
		else
		{
			leaveRoomButton.EnabledButton();
		}
		if (Singleton<GooglePlayRTM>.Instance.currentRoom.status == GP_RTM_RoomStatus.ROOM_STATUS_ACTIVE)
		{
			helloButton.EnabledButton();
			hi.SetActive(value: true);
		}
		else
		{
			helloButton.DisabledButton();
			hi.SetActive(value: false);
		}
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

	private void OnPlayerDisconnected()
	{
		SA_StatusBar.text = "Player Disconnected";
		playerLabel.text = "Player Disconnected";
	}

	private void OnPlayerConnected()
	{
		SA_StatusBar.text = "Player Connected";
		playerLabel.text = Singleton<GooglePlayManager>.Instance.player.name;
		Singleton<GooglePlayInvitationManager>.Instance.RegisterInvitationListener();
		GooglePlayManager.ActionFriendsListLoaded += OnFriendListLoaded;
		Singleton<GooglePlayManager>.Instance.LoadFriends();
	}

	private void OnFriendListLoaded(GooglePlayResult result)
	{
		UnityEngine.Debug.Log("OnFriendListLoaded: " + result.Message);
		GooglePlayManager.ActionFriendsListLoaded -= OnFriendListLoaded;
		if (result.IsSucceeded)
		{
			UnityEngine.Debug.Log("Friends Load Success");
			int num = 0;
			foreach (string friends2 in Singleton<GooglePlayManager>.Instance.friendsList)
			{
				if (num < 3)
				{
					friends[num].SetFriendId(friends2);
				}
				num++;
			}
		}
	}

	private void OnConnectionResult(GooglePlayConnectionResult result)
	{
		SA_StatusBar.text = "ConnectionResul:  " + result.code.ToString();
		UnityEngine.Debug.Log(result.code.ToString());
	}

	private void OnInvite(GP_Invite invitation)
	{
		if (invitation.InvitationType == GP_InvitationType.INVITATION_TYPE_REAL_TIME)
		{
			inviteId = invitation.Id;
			AndroidDialog androidDialog = AndroidDialog.Create("Invite", "You have new invite from: " + invitation.Participant.DisplayName, "Manage Manually", "Open Google Inbox");
			androidDialog.ActionComplete += OnInvDialogComplete;
		}
	}

	private void ActionInvitationAccepted(GP_Invite invitation)
	{
		UnityEngine.Debug.Log("ActionInvitationAccepted called");
		if (invitation.InvitationType == GP_InvitationType.INVITATION_TYPE_REAL_TIME)
		{
			UnityEngine.Debug.Log("Starting The Game");
			Singleton<GooglePlayRTM>.Instance.AcceptInvitation(invitation.Id);
		}
	}

	private void OnRoomCreated(GP_GamesStatusCodes code)
	{
		SA_StatusBar.text = "Room Create Result:  " + code.ToString();
	}

	private void OnInvDialogComplete(AndroidDialogResult result)
	{
		switch (result)
		{
		case AndroidDialogResult.YES:
		{
			AndroidDialog androidDialog = AndroidDialog.Create("Manage Invite", "Would you like to accept this invite?", "Accept", "Decline");
			androidDialog.ActionComplete += OnInvManageDialogComplete;
			break;
		}
		case AndroidDialogResult.NO:
			Singleton<GooglePlayRTM>.Instance.OpenInvitationInBoxUI();
			break;
		}
	}

	private void OnInvManageDialogComplete(AndroidDialogResult result)
	{
		switch (result)
		{
		case AndroidDialogResult.YES:
			Singleton<GooglePlayRTM>.Instance.AcceptInvitation(inviteId);
			break;
		case AndroidDialogResult.NO:
			Singleton<GooglePlayRTM>.Instance.DeclineInvitation(inviteId);
			break;
		}
	}

	private void OnGCDataReceived(GP_RTM_Network_Package package)
	{
		UTF8Encoding uTF8Encoding = new UTF8Encoding();
		string @string = uTF8Encoding.GetString(package.buffer);
		string str = package.participantId;
		GP_Participant participantById = Singleton<GooglePlayRTM>.Instance.currentRoom.GetParticipantById(package.participantId);
		if (participantById != null)
		{
			GooglePlayerTemplate playerById = Singleton<GooglePlayManager>.Instance.GetPlayerById(participantById.playerId);
			if (playerById != null)
			{
				str = playerById.name;
			}
		}
		AndroidMessage.Create("Data Eeceived", "player " + str + " \n data: " + @string);
	}
}
