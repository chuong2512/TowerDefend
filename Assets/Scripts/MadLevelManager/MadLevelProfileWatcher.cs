using UnityEngine;

namespace MadLevelManager
{
	public class MadLevelProfileWatcher : MonoBehaviour
	{
		private bool alreadyWatching;

		private MadLevelProfileBufferedBackend bufferedBackend;

		private void OnEnable()
		{
			Object.DontDestroyOnLoad(base.gameObject);
		}

		private void OnApplicationPause()
		{
			if (bufferedBackend != null)
			{
				bufferedBackend.Flush();
			}
		}

		private void OnApplicationQuit()
		{
			if (bufferedBackend != null)
			{
				bufferedBackend.Flush();
			}
		}

		public void Watch(MadLevelProfileBufferedBackend bufferedBackend)
		{
			this.bufferedBackend = bufferedBackend;
			if (alreadyWatching)
			{
				UnityEngine.Debug.LogWarning("You're creating more than one BufferedBackend for this project. Please make sure that you will do that only once.");
			}
			StartCoroutine(bufferedBackend.Run());
			alreadyWatching = true;
		}
	}
}
