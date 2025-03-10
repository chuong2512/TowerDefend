using SA.Common.Pattern;
using SA.Common.Util;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class GooglePlayRTM : Singleton<GooglePlayRTM>
{
	public static Action<GP_RTM_Network_Package> ActionDataRecieved = delegate
	{
	};

	public static Action<GP_RTM_Room> ActionRoomUpdated = delegate
	{
	};

	public static Action<GP_RTM_ReliableMessageSentResult> ActionReliableMessageSent = delegate
	{
	};

	public static Action<GP_RTM_ReliableMessageDeliveredResult> ActionReliableMessageDelivered = delegate
	{
	};

	public static Action ActionConnectedToRoom = delegate
	{
	};

	public static Action ActionDisconnectedFromRoom = delegate
	{
	};

	public static Action<string> ActionP2PConnected = delegate
	{
	};

	public static Action<string> ActionP2PDisconnected = delegate
	{
	};

	public static Action<string[]> ActionPeerDeclined = delegate
	{
	};

	public static Action<string[]> ActionPeerInvitedToRoom = delegate
	{
	};

	public static Action<string[]> ActionPeerJoined = delegate
	{
	};

	public static Action<string[]> ActionPeerLeft = delegate
	{
	};

	public static Action<string[]> ActionPeersConnected = delegate
	{
	};

	public static Action<string[]> ActionPeersDisconnected = delegate
	{
	};

	public static Action ActionRoomAutomatching = delegate
	{
	};

	public static Action ActionRoomConnecting = delegate
	{
	};

	public static Action<GP_GamesStatusCodes> ActionJoinedRoom = delegate
	{
	};

	public static Action<GP_RTM_Result> ActionLeftRoom = delegate
	{
	};

	public static Action<GP_GamesStatusCodes> ActionRoomConnected = delegate
	{
	};

	public static Action<GP_GamesStatusCodes> ActionRoomCreated = delegate
	{
	};

	public static Action<AndroidActivityResult> ActionInvitationBoxUIClosed = delegate
	{
	};

	public static Action<AndroidActivityResult> ActionWatingRoomIntentClosed = delegate
	{
	};

	public static Action<GP_Invite> ActionInvitationAccepted = delegate
	{
	};

	public static Action<GP_Invite> ActionInvitationReceived = delegate
	{
	};

	public static Action<string> ActionInvitationRemoved = delegate
	{
	};

	private const int BYTE_LIMIT = 256;

	private GP_RTM_Room _currentRoom = new GP_RTM_Room();

	private List<GP_Invite> _invitations = new List<GP_Invite>();

	private Dictionary<int, GP_RTM_ReliableMessageListener> _ReliableMassageListeners = new Dictionary<int, GP_RTM_ReliableMessageListener>();

	public GP_RTM_Room currentRoom => _currentRoom;

	public List<GP_Invite> invitations => _invitations;

	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		_currentRoom = new GP_RTM_Room();
		GooglePlayInvitationManager.ActionInvitationReceived += OnInvitationReceived;
		GooglePlayInvitationManager.ActionInvitationRemoved += OnInvitationRemoved;
		GooglePlayInvitationManager.ActionInvitationAccepted += OnInvitationAccepted;
		Singleton<GooglePlayInvitationManager>.Instance.Init();
		UnityEngine.Debug.Log("GooglePlayRTM Created");
	}

	public void FindMatch(int minPlayers, int maxPlayers)
	{
		FindMatch(minPlayers, maxPlayers, new string[0]);
	}

	public void FindMatch(int minPlayers, int maxPlayers, params GooglePlayerTemplate[] playersToInvite)
	{
		List<string> list = new List<string>();
		foreach (GooglePlayerTemplate googlePlayerTemplate in playersToInvite)
		{
			list.Add(googlePlayerTemplate.playerId);
		}
		AN_GMSRTMProxy.RTMFindMatch(minPlayers, maxPlayers, list.ToArray());
	}

	public void FindMatch(int minPlayers, int maxPlayers, params string[] playersToInvite)
	{
		AN_GMSRTMProxy.RTMFindMatch(minPlayers, maxPlayers, playersToInvite);
	}

	public void FindMatch(GooglePlayerTemplate[] playersToInvite)
	{
		List<string> list = new List<string>();
		foreach (GooglePlayerTemplate googlePlayerTemplate in playersToInvite)
		{
			list.Add(googlePlayerTemplate.playerId);
		}
		AN_GMSRTMProxy.RTMFindMatch(list.ToArray());
	}

	public void FindMatch(string[] playersToInvite)
	{
		AN_GMSRTMProxy.RTMFindMatch(playersToInvite);
	}

	public void SendDataToAll(byte[] data, GP_RTM_PackageType sendType)
	{
		string data2 = ConvertByteDataToString(data);
		switch (sendType)
		{
		case GP_RTM_PackageType.RELIABLE:
		{
			GP_RTM_ReliableMessageListener gP_RTM_ReliableMessageListener = new GP_RTM_ReliableMessageListener(IdFactory.NextId, data);
			_ReliableMassageListeners.Add(gP_RTM_ReliableMessageListener.DataTokenId, gP_RTM_ReliableMessageListener);
			AN_GMSRTMProxy.sendDataToAll(data2, (int)sendType);
			break;
		}
		case GP_RTM_PackageType.UNRELIABLE:
			AN_GMSRTMProxy.sendDataToAll(data2, (int)sendType);
			break;
		}
	}

	public void sendDataToPlayers(byte[] data, GP_RTM_PackageType sendType, params string[] players)
	{
		string data2 = ConvertByteDataToString(data);
		string players2 = string.Join("|", players);
		switch (sendType)
		{
		case GP_RTM_PackageType.RELIABLE:
		{
			GP_RTM_ReliableMessageListener gP_RTM_ReliableMessageListener = new GP_RTM_ReliableMessageListener(IdFactory.NextId, data);
			_ReliableMassageListeners.Add(gP_RTM_ReliableMessageListener.DataTokenId, gP_RTM_ReliableMessageListener);
			AN_GMSRTMProxy.sendDataToPlayers(data2, players2, (int)sendType);
			break;
		}
		case GP_RTM_PackageType.UNRELIABLE:
			AN_GMSRTMProxy.sendDataToPlayers(data2, players2, (int)sendType);
			break;
		}
	}

	public void ShowWaitingRoomIntent()
	{
		AN_GMSRTMProxy.ShowWaitingRoomIntent();
	}

	public void OpenInvitationBoxUI(int minPlayers, int maxPlayers)
	{
		AN_GMSRTMProxy.InvitePlayers(minPlayers, maxPlayers);
	}

	public void LeaveRoom()
	{
		AN_GMSGiftsProxy.leaveRoom();
	}

	public void AcceptInvitation(string invitationId)
	{
		AN_GMSRTMProxy.RTM_AcceptInvitation(invitationId);
	}

	public void DeclineInvitation(string invitationId)
	{
		AN_GMSRTMProxy.RTM_DeclineInvitation(invitationId);
	}

	public void DismissInvitation(string invitationId)
	{
		AN_GMSRTMProxy.RTM_DismissInvitation(invitationId);
	}

	public void OpenInvitationInBoxUI()
	{
		AN_GMSGiftsProxy.showInvitationBox();
	}

	public void SetVariant(int val)
	{
		AN_GMSRTMProxy.RTM_SetVariant(val);
	}

	public void SetExclusiveBitMask(int val)
	{
		AN_GMSRTMProxy.RTM_SetExclusiveBitMask(val);
	}

	public void ClearReliableMessageListener(int dataTokenId)
	{
		if (_ReliableMassageListeners.ContainsKey(dataTokenId))
		{
			_ReliableMassageListeners.Remove(dataTokenId);
			UnityEngine.Debug.Log("[ClearReliableMessageListener] Remove data with token " + dataTokenId);
		}
	}

	private void OnWatingRoomIntentClosed(string data)
	{
		UnityEngine.Debug.Log("[OnWatingRoomIntentClosed] data " + data);
		string[] array = data.Split("|"[0]);
		AndroidActivityResult obj = new AndroidActivityResult(array[0], array[1]);
		ActionWatingRoomIntentClosed(obj);
	}

	private void OnRoomUpdate(string data)
	{
		string[] array = data.Split("|"[0]);
		_currentRoom = new GP_RTM_Room();
		_currentRoom.id = array[0];
		_currentRoom.creatorId = array[1];
		string[] array2 = array[2].Split(","[0]);
		for (int i = 0; i < array2.Length && !(array2[i] == "endofline"); i += 6)
		{
			GP_Participant p = new GP_Participant(array2[i], array2[i + 1], array2[i + 2], array2[i + 3], array2[i + 4], array2[i + 5]);
			_currentRoom.AddParticipant(p);
		}
		_currentRoom.status = (GP_RTM_RoomStatus)Convert.ToInt32(array[3]);
		_currentRoom.creationTimestamp = Convert.ToInt64(array[4]);
		UnityEngine.Debug.Log("GooglePlayRTM OnRoomUpdate Room State: " + _currentRoom.status.ToString());
		ActionRoomUpdated(_currentRoom);
	}

	private void OnReliableMessageSent(string data)
	{
		UnityEngine.Debug.Log("[OnReliableMessageSent] " + data);
		string[] array = data.Split("|"[0]);
		int messageTokedId = int.Parse(array[2]);
		int key = int.Parse(array[3]);
		if (_ReliableMassageListeners.ContainsKey(key))
		{
			GP_RTM_ReliableMessageSentResult obj = new GP_RTM_ReliableMessageSentResult(array[0], array[1], messageTokedId, _ReliableMassageListeners[key].Data);
			ActionReliableMessageSent(obj);
			_ReliableMassageListeners[key].ReportSentMessage();
		}
		else
		{
			GP_RTM_ReliableMessageSentResult obj2 = new GP_RTM_ReliableMessageSentResult(array[0], array[1], messageTokedId, null);
			ActionReliableMessageSent(obj2);
		}
	}

	private void OnReliableMessageDelivered(string data)
	{
		UnityEngine.Debug.Log("[OnReliableMessageDelivered] " + data);
		string[] array = data.Split("|"[0]);
		int messageTokedId = int.Parse(array[2]);
		int key = int.Parse(array[3]);
		if (_ReliableMassageListeners.ContainsKey(key))
		{
			GP_RTM_ReliableMessageDeliveredResult obj = new GP_RTM_ReliableMessageDeliveredResult(array[0], array[1], messageTokedId, _ReliableMassageListeners[key].Data);
			ActionReliableMessageDelivered(obj);
			_ReliableMassageListeners[key].ReportDeliveredMessage();
		}
		else
		{
			GP_RTM_ReliableMessageDeliveredResult obj2 = new GP_RTM_ReliableMessageDeliveredResult(array[0], array[1], messageTokedId, null);
			ActionReliableMessageDelivered(obj2);
		}
	}

	private void OnMatchDataRecieved(string data)
	{
		if (data.Equals(string.Empty))
		{
			UnityEngine.Debug.Log("OnMatchDataRecieved, no data avaiable");
			return;
		}
		string[] array = data.Split("|"[0]);
		GP_RTM_Network_Package obj = new GP_RTM_Network_Package(array[0], array[1]);
		ActionDataRecieved(obj);
		UnityEngine.Debug.Log("GooglePlayManager -> DATA_RECEIVED");
	}

	private void OnConnectedToRoom(string data)
	{
		UnityEngine.Debug.Log("[OnConnectedToRoom] data " + data);
		ActionConnectedToRoom();
	}

	private void OnDisconnectedFromRoom(string data)
	{
		UnityEngine.Debug.Log("[OnDisconnectedFromRoom] data " + data);
		ActionDisconnectedFromRoom();
	}

	private void OnP2PConnected(string participantId)
	{
		UnityEngine.Debug.Log("[OnP2PConnected] participantId " + participantId);
		ActionP2PConnected(participantId);
	}

	private void OnP2PDisconnected(string participantId)
	{
		UnityEngine.Debug.Log("[OnP2PDisconnected] participantId " + participantId);
		ActionP2PDisconnected(participantId);
	}

	private void OnPeerDeclined(string data)
	{
		UnityEngine.Debug.Log("[OnPeerDeclined] data " + data);
		string[] obj = data.Split(","[0]);
		ActionPeerDeclined(obj);
	}

	private void OnPeerInvitedToRoom(string data)
	{
		UnityEngine.Debug.Log("[OnPeerInvitedToRoom] data " + data);
		string[] obj = data.Split(","[0]);
		ActionPeerInvitedToRoom(obj);
	}

	private void OnPeerJoined(string data)
	{
		UnityEngine.Debug.Log("[OnPeerJoined] data " + data);
		string[] obj = data.Split(","[0]);
		ActionPeerJoined(obj);
	}

	private void OnPeerLeft(string data)
	{
		UnityEngine.Debug.Log("[OnPeerLeft] data " + data);
		string[] obj = data.Split(","[0]);
		ActionPeerLeft(obj);
	}

	private void OnPeersConnected(string data)
	{
		UnityEngine.Debug.Log("[OnPeersConnected] data " + data);
		string[] obj = data.Split(","[0]);
		ActionPeersConnected(obj);
	}

	private void OnPeersDisconnected(string data)
	{
		UnityEngine.Debug.Log("[OnPeersDisconnected] data " + data);
		string[] obj = data.Split(","[0]);
		ActionPeersDisconnected(obj);
	}

	private void OnRoomAutoMatching(string data)
	{
		UnityEngine.Debug.Log("[OnRoomAutoMatching] data " + data);
		ActionRoomAutomatching();
	}

	private void OnRoomConnecting(string data)
	{
		UnityEngine.Debug.Log("[OnRoomConnecting] data " + data);
		ActionRoomConnecting();
	}

	private void OnJoinedRoom(string data)
	{
		UnityEngine.Debug.Log("[OnJoinedRoom] data " + data);
		GP_GamesStatusCodes obj = (GP_GamesStatusCodes)Convert.ToInt32(data);
		ActionJoinedRoom(obj);
	}

	private void OnLeftRoom(string data)
	{
		UnityEngine.Debug.Log("[OnLeftRoom] Created OnRoomUpdate data " + data);
		string[] array = data.Split("|"[0]);
		GP_RTM_Result obj = new GP_RTM_Result(array[0], array[1]);
		_currentRoom = new GP_RTM_Room();
		ActionRoomUpdated(_currentRoom);
		ActionLeftRoom(obj);
	}

	private void OnRoomConnected(string data)
	{
		UnityEngine.Debug.Log("[OnRoomConnected] data " + data);
		GP_GamesStatusCodes obj = (GP_GamesStatusCodes)Convert.ToInt32(data);
		ActionRoomConnected(obj);
	}

	private void OnRoomCreated(string data)
	{
		UnityEngine.Debug.Log("[OnRoomCreated] data " + data);
		GP_GamesStatusCodes obj = (GP_GamesStatusCodes)Convert.ToInt32(data);
		ActionRoomCreated(obj);
	}

	private void OnInvitationBoxUiClosed(string data)
	{
		UnityEngine.Debug.Log("[OnInvitationBoxUiClosed] data " + data);
		string[] array = data.Split("|"[0]);
		AndroidActivityResult obj = new AndroidActivityResult(array[0], array[1]);
		ActionInvitationBoxUIClosed(obj);
	}

	private void OnInvitationReceived(GP_Invite inv)
	{
		if (inv.InvitationType == GP_InvitationType.INVITATION_TYPE_REAL_TIME)
		{
			_invitations.Add(inv);
			ActionInvitationReceived(inv);
		}
	}

	private void OnInvitationRemoved(string invitationId)
	{
		UnityEngine.Debug.Log("[OnInvitationRemoved] invitationId " + invitationId);
		foreach (GP_Invite invitation in _invitations)
		{
			if (invitation.Id.Equals(invitationId))
			{
				_invitations.Remove(invitation);
				return;
			}
		}
		ActionInvitationRemoved(invitationId);
	}

	private void OnInvitationAccepted(GP_Invite inv)
	{
		ActionInvitationAccepted(inv);
	}

	public static byte[] ConvertStringToByteData(string data)
	{
		if (data == null)
		{
			return null;
		}
		data = data.Replace("endofline", string.Empty);
		if (data.Equals(string.Empty))
		{
			return null;
		}
		string[] array = data.Split(","[0]);
		List<byte> list = new List<byte>();
		string[] array2 = array;
		foreach (string value in array2)
		{
			int num = Convert.ToInt32(value);
			int value2 = (num >= 0) ? num : (256 + num);
			list.Add(Convert.ToByte(value2));
		}
		return list.ToArray();
	}

	public static string ConvertByteDataToString(byte[] data)
	{
		StringBuilder stringBuilder = new StringBuilder(string.Empty);
		for (int i = 0; i < data.Length; i++)
		{
			if (i != 0)
			{
				stringBuilder.Append(",");
			}
			stringBuilder.Append(data[i]);
		}
		return stringBuilder.ToString();
	}
}
