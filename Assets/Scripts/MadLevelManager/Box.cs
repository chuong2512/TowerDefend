using UnityEngine;

namespace MadLevelManager
{
	public class Box : MonoBehaviour
	{
		public int starsCount;

		public bool completeLevel;

		public void Update()
		{
			if (Input.GetMouseButton(0))
			{
				CheckPressed(UnityEngine.Input.mousePosition);
			}
			if (UnityEngine.Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
			{
				CheckPressed(Input.touches[0].position);
			}
		}

		private void CheckPressed(Vector2 screenPosition)
		{
			Ray ray = Camera.main.ScreenPointToRay(screenPosition);
			if (Physics.Raycast(ray, out RaycastHit hitInfo) && hitInfo.collider.gameObject == base.gameObject)
			{
				Execute();
			}
		}

		private void Execute()
		{
			for (int i = 1; i <= starsCount; i++)
			{
				string property = "star_" + i;
				MadLevelProfile.SetLevelBoolean(MadLevel.currentLevelName, property, val: true);
			}
			if (completeLevel)
			{
				MadLevelProfile.SetCompleted(MadLevel.currentLevelName, completed: true);
			}
			MadLevel.LoadLevelByName("Level Select");
		}
	}
}
