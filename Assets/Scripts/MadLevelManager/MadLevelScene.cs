using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MadLevelManager
{
	[Serializable]
	public class MadLevelScene
	{
		[SerializeField]
		private UnityEngine.Object _sceneObject;

		[SerializeField]
		private string _scenePath;

		[SerializeField]
		private string _sceneName;

		[SerializeField]
		private string scene = string.Empty;

		public UnityEngine.Object sceneObject
		{
			get
			{
				return _sceneObject;
			}
			set
			{
				if (!Application.isEditor)
				{
					UnityEngine.Debug.LogError("This method has no effect when calling from play mode");
				}
			}
		}

		public string scenePath
		{
			get
			{
				UpdateScenePath();
				return _scenePath;
			}
		}

		public string sceneName
		{
			get
			{
				UpdateSceneName();
				return _sceneName;
			}
		}

		public virtual void Load()
		{
			SceneManager.LoadScene(sceneName);
		}

		public virtual AsyncOperation LoadAsync()
		{
			return SceneManager.LoadSceneAsync(sceneName);
		}

		public void Upgrade()
		{
		}

		public virtual bool IsValid()
		{
			return sceneObject != null;
		}

		private void UpdateScenePath()
		{
		}

		private void UpdateSceneName()
		{
		}
	}
}
