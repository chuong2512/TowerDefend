using UnityEngine;

namespace MadLevelManager
{
	[ExecuteInEditMode]
	public class MadFollow : MonoBehaviour
	{
		public Transform followTransform;

		private void Update()
		{
			if (followTransform != null)
			{
				base.transform.position = followTransform.position;
			}
		}
	}
}
