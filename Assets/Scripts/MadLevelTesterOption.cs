using MadLevelManager;
using System;
using System.Collections;
using UnityEngine;

public class MadLevelTesterOption : MonoBehaviour
{
	public int points;

	private bool completed;

	private MadLevelTesterController controller;

	private void Start()
	{
		controller = GameObject.FindGameObjectWithTag("GameController").GetComponent<MadLevelTesterController>();
		MadSprite sprite = GetComponent<MadSprite>();
		MadSprite madSprite = sprite;
		MadSprite.Action onMouseUp = madSprite.onMouseUp;
		MadSprite.Action action = delegate
		{
			if (points >= 100)
			{
				EarnStar("star_1");
				EarnMedal("bronze");
				MarkLevelCompleted();
			}
			if (points >= 150)
			{
				EarnStar("star_2");
				EarnMedal("silver");
				MarkLevelCompleted();
			}
			if (points >= 200)
			{
				EarnStar("star_3");
				EarnMedal("gold");
				MarkLevelCompleted();
			}
			controller.PlayFinishAnimation(sprite, completed);
			StartCoroutine(WaitForAnimation());
		};
		sprite.onTap = action;
		madSprite.onMouseUp = (MadSprite.Action)Delegate.Combine(onMouseUp, action);
	}

	private void EarnStar(string name)
	{
		MadLevelProfile.SetLevelBoolean(MadLevel.currentLevelName, name, val: true);
	}

	private void EarnMedal(string name)
	{
		MadLevelProfile.SetLevelString(MadLevel.currentLevelName, "medal", name);
	}

	private void MarkLevelCompleted()
	{
		MadLevelProfile.SetCompleted(MadLevel.currentLevelName, completed: true);
		completed = true;
	}

	private IEnumerator WaitForAnimation()
	{
		yield return new WaitForSeconds(2.2f);
		if (completed)
		{
			if (MadLevel.hasExtension && MadLevel.CanContinue())
			{
				MadLevel.Continue();
			}
			else if (MadLevel.HasNextInGroup(MadLevel.Type.Level))
			{
				MadLevel.LoadNextInGroup(MadLevel.Type.Level);
			}
			else
			{
				controller.LoadLevelSelectScreen();
			}
		}
		else
		{
			controller.LoadLevelSelectScreen();
		}
	}
}
