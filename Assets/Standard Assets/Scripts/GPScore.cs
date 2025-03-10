using SA.Common.Pattern;
using System;

[Serializable]
public class GPScore
{
	private int _rank;

	private long _score;

	private string _playerId;

	private string _leaderboardId;

	private GPCollectionType _collection;

	private GPBoardTimeSpan _timeSpan;

	[Obsolete("rank is deprectaed, plase use Rank instead")]
	public int rank => _rank;

	public int Rank => _rank;

	[Obsolete("score is deprectaed, plase use LongScore instead")]
	public long score => _score;

	public long LongScore => _score;

	public float CurrencyScore => (float)_score / 100f;

	public TimeSpan TimeScore => System.TimeSpan.FromMilliseconds(_score);

	[Obsolete("playerId is deprectaed, plase use PlayerId instead")]
	public string playerId => _playerId;

	public string PlayerId => _playerId;

	public GooglePlayerTemplate Player => Singleton<GooglePlayManager>.Instance.GetPlayerById(PlayerId);

	[Obsolete("leaderboardId is deprectaed, plase use LeaderboardId instead")]
	public string leaderboardId => _leaderboardId;

	public string LeaderboardId => _leaderboardId;

	[Obsolete("collection is deprectaed, plase use Collection instead")]
	public GPCollectionType collection => _collection;

	public GPCollectionType Collection => _collection;

	[Obsolete("timeSpan is deprectaed, plase use TimeSpan instead")]
	public GPBoardTimeSpan timeSpan => _timeSpan;

	public GPBoardTimeSpan TimeSpan => _timeSpan;

	public GPScore(long vScore, int vRank, GPBoardTimeSpan vTimeSpan, GPCollectionType sCollection, string lid, string pid)
	{
		_score = vScore;
		_rank = vRank;
		_playerId = pid;
		_leaderboardId = lid;
		_timeSpan = vTimeSpan;
		_collection = sCollection;
	}

	public void UpdateScore(long vScore)
	{
		_score = vScore;
	}
}
