using SA.Common.Pattern;
using SIS;
using TDTK;
using UnityEngine;
using UnityEngine.UI;

public class EndLessControl : MonoBehaviour
{
	public Text TextWave;

	public Text TextGear;

	public Text TextBest;

	public GameObject BestTag;

	private int Score;

	private int HighScore;

	private const string LEADERBOARD_ID = "CgkI2Pu7krcSEAIQAA";

	private void Start()
	{
		Score = SpawnManager.GetCurrentWaveID();
		Score++;
		HighScore = GetHighScore(Score);
		SubmitScore(HighScore);
		int score = Score;
		score *= 3 + UnityEngine.Random.Range(0, 4);
		TextBest.text = "Best wave\n" + HighScore.ToString();
		TextWave.text = "Current wave\n" + Score.ToString();
		TextGear.text = score.ToString();
		DBManager.IncreaseFunds("coins", score);
	}

	public void SubmitScore(int NumScore)
	{
		Singleton<GooglePlayManager>.Instance.SubmitScoreById("CgkI2Pu7krcSEAIQAA", NumScore);
	}

	public void showLeaderBoard()
	{
		Singleton<GooglePlayManager>.Instance.ShowLeaderBoardById("CgkI2Pu7krcSEAIQAA");
	}

	private int GetHighScore(int ScoreNum)
	{
		int num = ES2.Load<int>("KeyHighScore");
		if (ScoreNum >= num)
		{
			ES2.Save(ScoreNum, "KeyHighScore");
			BestTag.SetActive(value: true);
			return ScoreNum;
		}
		return num;
	}
}
