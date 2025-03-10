using MadLevelManager;
using UnityEngine;
using UnityEngine.UI;

public class StarControl : MonoBehaviour
{
	public Button DeadmodeBtn;

	public Text TextStar;

	private string GroupName = "Campaign Level";

	private void Start()
	{
	}

	public static int CountAvailableStars(string groupName)
	{
		string[] allLevelNames = MadLevel.GetAllLevelNames(MadLevel.Type.Level, groupName);
		return allLevelNames.Length * 3;
	}

	public static int CountAcquiredStars(string groupName)
	{
		string[] allLevelNames = MadLevel.GetAllLevelNames(MadLevel.Type.Level, groupName);
		int num = 0;
		for (int i = 0; i < allLevelNames.Length; i++)
		{
			if (MadLevelProfile.GetLevelBoolean(allLevelNames[i], "star_1"))
			{
				num++;
			}
			if (MadLevelProfile.GetLevelBoolean(allLevelNames[i], "star_2"))
			{
				num++;
			}
			if (MadLevelProfile.GetLevelBoolean(allLevelNames[i], "star_3"))
			{
				num++;
			}
		}
		return num;
	}
}
