using System;
using System.Collections.Generic;
using UnityEngine;

namespace MadLevelManager
{
	[Serializable]
	public class MadLevelExtension
	{
		public string name = string.Empty;

		public string guid = "0";

		public List<MadLevelScene> scenesBefore = new List<MadLevelScene>();

		public List<MadLevelScene> scenesAfter = new List<MadLevelScene>();

		public MadLevelExtension(string name)
		{
			this.name = name;
			guid = Guid.NewGuid().ToString();
		}

		public void Load(MadLevelConfiguration.Level level)
		{
			MadLevel.currentExtension = this;
			MadLevel.currentExtensionProgress = 0;
			if (scenesBefore.Count != 0)
			{
				MadLevelScene madLevelScene = scenesBefore[0];
				MadLevel.lastPlayedLevelName = MadLevel.currentLevelName;
				MadLevel.currentLevelName = level.name;
				MadLevel.arguments = level.arguments;
				madLevelScene.Load();
			}
			else
			{
				level.Load();
			}
		}

		public AsyncOperation LoadAsync(MadLevelConfiguration.Level level)
		{
			MadLevel.currentExtension = this;
			MadLevel.currentExtensionProgress = 0;
			if (scenesBefore.Count != 0)
			{
				MadLevelScene madLevelScene = scenesBefore[0];
				MadLevel.lastPlayedLevelName = MadLevel.currentLevelName;
				MadLevel.currentLevelName = level.name;
				return madLevelScene.LoadAsync();
			}
			return level.LoadAsync();
		}

		public void Continue(MadLevelScene currentLevel, int progress)
		{
			if (CheckCanContinue(currentLevel, progress))
			{
				List<MadLevelScene> list = ScenesInOrder(currentLevel);
				MadLevelScene madLevelScene = list[progress + 1];
				MadLevel.currentExtensionProgress = progress + 1;
				madLevelScene.Load();
			}
		}

		public AsyncOperation ContinueAsync(MadLevelScene currentLevel, int progress)
		{
			if (!CheckCanContinue(currentLevel, progress))
			{
				return null;
			}
			List<MadLevelScene> list = ScenesInOrder(currentLevel);
			MadLevelScene madLevelScene = list[progress + 1];
			MadLevel.currentExtensionProgress = progress + 1;
			return madLevelScene.LoadAsync();
		}

		private bool CheckCanContinue(MadLevelScene currentLevel, int progress)
		{
			if (!CanContinue(currentLevel, progress))
			{
				UnityEngine.Debug.LogError("Cannot continue the level, this is the last scene. Please use MadLevel.CanContinue() to check if level can be continued. Now you have to use MadLevel.LoadNext(), MadLevel.LoadLevelByName() or similar.");
				return false;
			}
			return true;
		}

		public bool CanContinue(MadLevelScene currentLevel, int progress)
		{
			List<MadLevelScene> list = ScenesInOrder(currentLevel);
			if (list.Count > progress + 1)
			{
				return true;
			}
			return false;
		}

		private List<MadLevelScene> ScenesInOrder(MadLevelScene currentLevel)
		{
			List<MadLevelScene> list = new List<MadLevelScene>();
			list.AddRange(scenesBefore);
			list.Add(currentLevel);
			list.AddRange(scenesAfter);
			return list;
		}

		internal bool IsValid()
		{
			foreach (MadLevelScene item in scenesBefore)
			{
				if (!item.IsValid())
				{
					return false;
				}
			}
			foreach (MadLevelScene item2 in scenesAfter)
			{
				if (!item2.IsValid())
				{
					return false;
				}
			}
			return true;
		}
	}
}
