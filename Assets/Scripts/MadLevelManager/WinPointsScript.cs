using UnityEngine;

namespace MadLevelManager
{
	public class WinPointsScript : MonoBehaviour
	{
		public int points = 100;

		private void OnMouseDown()
		{
			UnityEngine.Debug.Log("Level won", this);
			MadLevelProfile.SetCompleted(MadLevel.currentLevelName, completed: true);
			MadLevelProfile.SetLevelInteger(MadLevel.currentLevelName, "score", points);
			MadLevel.LoadLevelByName("Level Select");
		}

		private void OnGUI()
		{
			Vector3 vector = Camera.main.WorldToScreenPoint(base.transform.position);
			GUI.Label(new Rect(vector.x - 50f, vector.y + 100f, 100f, 50f), "Win " + points);
		}
	}
}
