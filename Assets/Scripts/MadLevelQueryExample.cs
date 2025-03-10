using MadLevelManager;
using UnityEngine;

public class MadLevelQueryExample : MonoBehaviour
{
	private MadText text;

	private void Start()
	{
		text = GetComponent<MadText>();
		int num = new MadLevelQuery().ForGroup(MadLevel.currentGroupName).OfLevelType(MadLevel.Type.Level).CountLevels();
		int num2 = new MadLevelQuery().ForGroup(MadLevel.currentGroupName).OfLevelType(MadLevel.Type.Level).CountUnlocked();
		int num3 = new MadLevelQuery().ForGroup(MadLevel.currentGroupName).OfLevelType(MadLevel.Type.Level).SelectProperty("star_1", "star_2", "star_3")
			.CountProperties();
		int num4 = new MadLevelQuery().ForGroup(MadLevel.currentGroupName).OfLevelType(MadLevel.Type.Level).SelectProperty("star_1", "star_2", "star_3")
			.CountEnabled();
		text.text = "Levels: " + num + ", Unlocked: " + num2 + "\nStars: " + num3 + ", Acquired: " + num4;
	}
}
