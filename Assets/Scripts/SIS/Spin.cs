using UnityEngine;

namespace SIS
{
	public class Spin : MonoBehaviour
	{
		public float rotationsPerSecond = 0.1f;

		private void Update()
		{
			Vector3 localEulerAngles = base.transform.localEulerAngles;
			localEulerAngles.z -= rotationsPerSecond * 360f * Time.deltaTime;
			base.transform.localEulerAngles = localEulerAngles;
		}
	}
}
