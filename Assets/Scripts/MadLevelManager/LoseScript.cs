using UnityEngine;

namespace MadLevelManager
{
	public class LoseScript : MonoBehaviour
	{
		private void OnMouseDown()
		{
			UnityEngine.Debug.Log("Level lost", this);
			MadLevel.LoadLevelByName("Level Select");
		}

		private void OnGUI()
		{
			Vector3 vector = Camera.main.WorldToScreenPoint(base.transform.position);
			GUI.Label(new Rect(vector.x - 50f, vector.y + 100f, 100f, 50f), "Lose");
		}
	}
}
