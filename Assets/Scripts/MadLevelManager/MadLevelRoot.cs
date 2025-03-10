using UnityEngine;

namespace MadLevelManager
{
	[ExecuteInEditMode]
	public class MadLevelRoot : MadNode
	{
		public const int CameraNearClip = -10;

		public const int CameraFarClip = 5;

		private const int CurrentVersion = 230;

		[SerializeField]
		private int version;

		private void Update()
		{
			if (version < 230)
			{
				Upgrade();
			}
		}

		private void Upgrade()
		{
			Camera camera = MadTransform.FindChild(base.transform, (Camera obj) => obj.name == "Camera 2D");
			if (camera != null)
			{
				camera.nearClipPlane = -10f;
				camera.farClipPlane = 5f;
				UnityEngine.Debug.Log("Camera 2D clip planes has been updated to recommended values. Please save the scene afterwards.", camera);
			}
			SetCurrentVersion();
		}

		public void SetCurrentVersion()
		{
			version = 230;
		}
	}
}
