using System;
using System.Collections.Generic;

public class GP_TBM_Match
{
	public string Id;

	public string RematchId;

	public string CreatorId;

	public string LastUpdaterId;

	public string PendingParticipantId;

	public int MatchNumber;

	public string Description;

	public int AvailableAutoMatchSlots;

	public long CreationTimestamp;

	public long LastUpdatedTimestamp;

	public GP_TBM_MatchStatus Status;

	public GP_TBM_MatchTurnStatus TurnStatus;

	public bool CanRematch;

	public int Variant;

	public int Version;

	public byte[] Data;

	public byte[] PreviousMatchData;

	public List<GP_Participant> Participants;

	public void SetData(string val)
	{
		byte[] array = Data = Convert.FromBase64String(val);
	}

	public void SetPreviousMatchData(string val)
	{
		byte[] array = PreviousMatchData = Convert.FromBase64String(val);
	}
}
