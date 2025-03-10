using UnityEngine;

namespace MadLevelManager
{
	[ExecuteInEditMode]
	public class MadRootNode : MadNode
	{
		public enum ResizeMode
		{
			PixelPerfect,
			FixedSize
		}

		public ResizeMode resizeMode = ResizeMode.FixedSize;

		public int manualHeight = 720;

		public int minimumHeight = 320;

		public int maximumHeight = 1536;

		public float pixelSize
		{
			get
			{
				if (resizeMode == ResizeMode.FixedSize)
				{
					return (float)Screen.height / (float)manualHeight;
				}
				return 0.5f;
			}
		}

		public float screenHeight
		{
			get
			{
				switch (resizeMode)
				{
				case ResizeMode.FixedSize:
					return manualHeight;
				case ResizeMode.PixelPerfect:
					return MadScreen.height;
				default:
					UnityEngine.Debug.Log("Unknown resize mode: " + resizeMode);
					return 128f;
				}
			}
		}

		public float screenWidth
		{
			get
			{
				int width = MadScreen.width;
				int height = MadScreen.height;
				switch (resizeMode)
				{
				case ResizeMode.FixedSize:
					return (float)manualHeight * ((float)width / (float)height);
				case ResizeMode.PixelPerfect:
					return width;
				default:
					UnityEngine.Debug.Log("Unknown resize mode: " + resizeMode);
					return 128f;
				}
			}
		}

		private void OnEnable()
		{
			UpdateScale();
		}

		private void Start()
		{
			UpdateScale();
		}

		private void Update()
		{
			UpdateScale();
		}

		private void UpdateScale()
		{
			float scale;
			switch (resizeMode)
			{
			case ResizeMode.FixedSize:
				scale = 1f / (float)manualHeight * 2f;
				break;
			case ResizeMode.PixelPerfect:
				scale = 1f / Mathf.Clamp(screenHeight, minimumHeight, maximumHeight) * 2f;
				break;
			default:
				UnityEngine.Debug.LogError("Unknown option: " + resizeMode);
				scale = 1f / (float)manualHeight * 2f;
				break;
			}
			MadTransform.SetLocalScale(base.transform, scale);
		}

		public Vector3 ScreenToLocal(Vector3 v)
		{
			float x = v.x;
			Vector3 localScale = base.transform.localScale;
			float x2 = x / localScale.x;
			float y = v.y;
			Vector3 localScale2 = base.transform.localScale;
			return new Vector3(x2, y / localScale2.y, v.z);
		}

		public Vector3 ScreenGlobal(float x, float y)
		{
			x = x * 2f - 1f;
			y = y * 2f - 1f;
			float num = screenWidth / screenHeight;
			Vector3 position = base.transform.position;
			float x2 = x * num;
			float y2 = y;
			Vector3 position2 = base.transform.position;
			return position + new Vector3(x2, y2, position2.z);
		}
	}
}
