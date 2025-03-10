using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MadLevelManager
{
	public class MadLevelTesterController : MonoBehaviour
	{
		public MadText levelNameText;

		public MadText argumentsText;

		public MadText backToMenu;

		public MadText levelCompletedText;

		public MadText levelNotCompletedText;

		public MadSprite[] other;

		private void Start()
		{
			levelNameText.text = "Level Name: '" + MadLevel.currentLevelName + "'";
			if (!string.IsNullOrEmpty(MadLevel.arguments))
			{
				argumentsText.text = "Arguments: " + MadLevel.arguments;
			}
			MadText madText = backToMenu;
			MadSprite.Action onMouseDown = madText.onMouseDown;
			MadSprite.Action action = delegate
			{
				LoadLevelSelectScreen();
			};
			backToMenu.onTap = action;
			madText.onMouseDown = (MadSprite.Action)Delegate.Combine(onMouseDown, action);
		}

		public void LoadLevelSelectScreen()
		{
			if (MadLevel.activeConfiguration.FindLevelByName("Level Select") != null)
			{
				MadLevel.LoadLevelByName("Level Select");
				return;
			}
			string currentGroupName = MadLevel.currentGroupName;
			MadLevelConfiguration.Group g = MadLevel.activeConfiguration.FindGroupByName(currentGroupName);
			IOrderedEnumerable<MadLevelConfiguration.Level> source = from level in MadLevel.activeConfiguration.levels
				where level.groupId == g.id && level.type == MadLevel.Type.Other
				orderby level.order
				select level;
			MadLevelConfiguration.Level level2 = source.FirstOrDefault();
			if (level2 != null)
			{
				MadLevel.LoadLevelByName(level2.name);
			}
			else
			{
				UnityEngine.Debug.LogError("Cannot found level to get back to :-(");
			}
		}

		public void PlayFinishAnimation(MadSprite chosenSprite, bool completed)
		{
			levelNameText.eventFlags = MadSprite.EventFlags.None;
			argumentsText.eventFlags = MadSprite.EventFlags.None;
			backToMenu.eventFlags = MadSprite.EventFlags.None;
			Color color = new Color(1f, 1f, 1f, 0f);
			Color color2 = new Color(1f, 1f, 1f, 1f);
			levelNameText.AnimColorTo(color, 1f, MadiTween.EaseType.linear);
			argumentsText.AnimColorTo(color, 1f, MadiTween.EaseType.linear);
			backToMenu.AnimColorTo(color, 1f, MadiTween.EaseType.linear);
			if (completed)
			{
				levelCompletedText.tint = color;
				levelCompletedText.visible = true;
				levelCompletedText.AnimColorTo(color2, 1f, MadiTween.EaseType.linear);
			}
			else
			{
				levelNotCompletedText.tint = color;
				levelNotCompletedText.visible = true;
				levelNotCompletedText.AnimColorTo(color2, 1f, MadiTween.EaseType.linear);
			}
			MadSprite[] array = other;
			foreach (MadSprite madSprite in array)
			{
				List<MadSprite> list = MadTransform.FindChildren<MadSprite>(madSprite.transform);
				madSprite.eventFlags = MadSprite.EventFlags.None;
				foreach (MadSprite item in list)
				{
					item.eventFlags = MadSprite.EventFlags.None;
				}
				if (madSprite != chosenSprite)
				{
					madSprite.AnimColorTo(color, 1f, MadiTween.EaseType.linear);
					foreach (MadSprite item2 in list)
					{
						item2.AnimColorTo(color, 1f, MadiTween.EaseType.linear);
					}
				}
			}
			chosenSprite.AnimMoveTo(default(Vector3), 1f, MadiTween.EaseType.easeOutSine);
			MadiTween.ScaleTo(chosenSprite.gameObject, MadiTween.Hash("scale", new Vector3(7f, 7f, 7f), "time", 0.5f, "easetype", MadiTween.EaseType.easeInQuint, "delay", 1.5f));
			MadiTween.ValueTo(chosenSprite.gameObject, MadiTween.Hash("from", chosenSprite.tint, "to", color, "time", 0.5f, "onupdate", "OnTintChange", "easetype", MadiTween.EaseType.easeInQuint, "delay", 1.5f));
			foreach (MadSprite item3 in MadTransform.FindChildren<MadSprite>(chosenSprite.transform))
			{
				MadiTween.ValueTo(item3.gameObject, MadiTween.Hash("from", item3.tint, "to", color, "time", 0.5f, "onupdate", "OnTintChange", "easetype", MadiTween.EaseType.easeInQuint, "delay", 1.5f));
			}
		}
	}
}
