using UnityEngine;
using UnityEngine.SceneManagement;

namespace SIS
{
	public class UIButtonScene : MonoBehaviour
	{
		public void LoadScene(string sceneName)
		{
			if (!string.IsNullOrEmpty(sceneName))
			{
				SceneManager.LoadScene(sceneName);
			}
		}
	}
}
