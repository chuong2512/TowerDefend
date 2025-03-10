using UnityEngine;

namespace MadLevelManager
{
	public class MadGameObject : MonoBehaviour
	{
		public static void SetActive(GameObject go, bool active)
		{
			go.SetActive(active);
		}

		public static bool IsActive(GameObject go)
		{
			return go.activeInHierarchy;
		}

		public static void SafeDestroy(UnityEngine.Object go)
		{
			if (Application.isPlaying)
			{
				UnityEngine.Object.Destroy(go);
			}
			else
			{
				UnityEngine.Object.DestroyImmediate(go);
			}
		}

		public static bool AnyNull(params object[] objects)
		{
			foreach (object obj in objects)
			{
				if (obj == null)
				{
					return true;
				}
			}
			return false;
		}
	}
}
