using UnityEngine;

namespace MadLevelManager
{
	public class MadTrial
	{
		public const string TRIAL_DURATION = "_TRIAL_DURATION_";

		public static bool isTrialVersion => !"_TRIAL_DURATION_".StartsWith("_");

		public static void InfoLabel(string text)
		{
			GUIContent content = new GUIContent(text);
			Vector2 vector = GUI.skin.label.CalcSize(content);
			GUI.color = Color.black;
			GUI.Label(new Rect((float)Screen.width - vector.x - 5f + 1f, (float)Screen.height - vector.y - 5f + 1f, vector.x, vector.y), content);
			GUI.color = Color.white;
			GUI.Label(new Rect((float)Screen.width - vector.x - 5f, (float)Screen.height - vector.y - 5f, vector.x, vector.y), content);
		}
	}
}
