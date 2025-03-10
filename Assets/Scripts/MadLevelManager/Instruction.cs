using UnityEngine;

namespace MadLevelManager
{
	public class Instruction : MonoBehaviour
	{
		public string text;

		private void OnGUI()
		{
			GUILayout.Box(text);
		}
	}
}
