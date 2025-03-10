using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MadLevelManager
{
	public class MadLevel
	{
		public enum Type
		{
			Other,
			Level,
			Extra
		}

		public delegate bool LevelPredicate(MadLevelConfiguration.Level level);

		private static MadLevelConfiguration _configuration;

		private static string _arguments;

		private static string _currentLevelName;

		private static MadLevelExtension _currentExtension;

		public static bool extensionDefined
		{
			get;
			private set;
		}

		public static string defaultGroupName => activeConfiguration.defaultGroup.name;

		public static MadLevelConfiguration activeConfiguration
		{
			get
			{
				if (_configuration == null || !_configuration.active)
				{
					_configuration = MadLevelConfiguration.GetActive();
				}
				return _configuration;
			}
		}

		public static bool hasActiveConfiguration
		{
			get
			{
				if (_configuration == null)
				{
					UnityEngine.Object[] array = Resources.LoadAll("LevelConfig", typeof(MadLevelConfiguration));
					return array.Length > 0;
				}
				return true;
			}
		}

		public static string arguments
		{
			get
			{
				if (_arguments == null)
				{
					FindCurrentSceneLevel();
				}
				return _arguments;
			}
			set
			{
				_arguments = value;
			}
		}

		public static string currentLevelName
		{
			get
			{
				if (!Application.isPlaying || _currentLevelName == null)
				{
					FindCurrentSceneLevel();
				}
				return _currentLevelName;
			}
			set
			{
				_currentLevelName = value;
			}
		}

		public static string currentGroupName
		{
			get
			{
				string currentLevelName = MadLevel.currentLevelName;
				MadLevelConfiguration.Level level = activeConfiguration.FindLevelByName(currentLevelName);
				return level.group.name;
			}
		}

		public static bool hasExtension => currentExtension != null;

		public static MadLevelExtension currentExtension
		{
			get
			{
				if (!extensionDefined)
				{
					string currentLevelName = MadLevel.currentLevelName;
					MadLevelConfiguration.Level level = activeConfiguration.FindLevelByName(currentLevelName);
					if (level != null)
					{
						currentExtension = level.extension;
						if (currentExtension != null)
						{
							currentExtensionProgress = currentExtension.scenesBefore.Count;
						}
					}
				}
				return _currentExtension;
			}
			set
			{
				_currentExtension = value;
				extensionDefined = true;
			}
		}

		public static int currentExtensionProgress
		{
			get;
			set;
		}

		[Obsolete("Use lastPlayedLevelName instead.")]
		public static string lastLevelName
		{
			get
			{
				return lastPlayedLevelName;
			}
			set
			{
				lastPlayedLevelName = value;
			}
		}

		public static string lastPlayedLevelName
		{
			get;
			set;
		}

		private static void FindCurrentSceneLevel()
		{
			string text = string.Empty;
			if (Application.isPlaying)
			{
				text = SceneManager.GetActiveScene().name;
			}
			bool hasMany;
			MadLevelConfiguration.Level level = activeConfiguration.FindFirstForScene(text, out hasMany);
			if (level != null)
			{
				currentLevelName = level.name;
				arguments = level.arguments;
				if (Application.isPlaying && hasMany)
				{
					UnityEngine.Debug.Log("Mad Level Manager: This was first scene opened. Assuming that this was '" + _currentLevelName + "' level, but there are many scenes with this name. http://madlevelmanager.madpixelmachine.com/doc/latest/faq.html");
				}
			}
			else
			{
				if (Application.isPlaying)
				{
					UnityEngine.Debug.LogError("Mad Level Manager: Cannot find scene " + text + " in the configuration. Is the level configuration broken or wrong configuration is active?");
				}
				currentLevelName = string.Empty;
				arguments = string.Empty;
			}
		}

		public static void ReloadCurrent()
		{
			LoadLevelByName(currentLevelName);
		}

		public static AsyncOperation ReloadCurrentAsync()
		{
			return SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
		}

		public static void LoadLevelByName(string levelName)
		{
			CheckHasConfiguration();
			MadLevelConfiguration.Level level = activeConfiguration.FindLevelByName(levelName);
			if (level != null)
			{
				LoadLevel(level);
			}
			else
			{
				UnityEngine.Debug.LogError($"Level \"{levelName}\" not found. Please verify your configuration.");
			}
		}

		public static AsyncOperation LoadLevelByNameAsync(string levelName)
		{
			CheckHasConfiguration();
			MadLevelConfiguration.Level level = activeConfiguration.FindLevelByName(levelName);
			if (level != null)
			{
				return LoadLevelAsync(level);
			}
			UnityEngine.Debug.LogError($"Level \"{levelName}\" not found. Please verify your configuration.");
			return null;
		}

		public static bool CanStreamedLevelBeLoaded(string levelName)
		{
			CheckHasConfiguration();
			MadLevelConfiguration.Level level = activeConfiguration.FindLevelByName(levelName);
			if (level != null)
			{
				return Application.CanStreamedLevelBeLoaded(level.name);
			}
			UnityEngine.Debug.LogError($"Level \"{levelName}\" not found. Please verify your configuration.");
			return false;
		}

		public static float GetStreamProgressForLevel(string levelName)
		{
			CheckHasConfiguration();
			MadLevelConfiguration.Level level = activeConfiguration.FindLevelByName(levelName);
			if (level != null)
			{
				return Application.GetStreamProgressForLevel(level.name);
			}
			UnityEngine.Debug.LogError($"Level \"{levelName}\" not found. Please verify your configuration.");
			return 0f;
		}

		public static bool HasNext()
		{
			CheckHasConfiguration();
			return activeConfiguration.FindNextLevel(currentLevelName) != null;
		}

		public static bool HasNextInGroup()
		{
			CheckHasConfiguration();
			return activeConfiguration.FindNextLevel(currentLevelName, sameGroup: true) != null;
		}

		public static bool HasNext(Type levelType)
		{
			CheckHasConfiguration();
			return activeConfiguration.FindNextLevel(currentLevelName, levelType) != null;
		}

		public static bool HasNextInGroup(Type levelType)
		{
			CheckHasConfiguration();
			return activeConfiguration.FindNextLevel(currentLevelName, levelType, sameGroup: true) != null;
		}

		public static void LoadNext()
		{
			CheckHasConfiguration();
			MadLevelConfiguration.Level level = activeConfiguration.FindNextLevel(currentLevelName);
			if (level != null)
			{
				LoadLevel(level);
			}
			else
			{
				UnityEngine.Debug.LogError("Cannot load next level: This is the last level.");
			}
		}

		public static void LoadNextInGroup()
		{
			CheckHasConfiguration();
			MadLevelConfiguration.Level level = activeConfiguration.FindNextLevel(currentLevelName, sameGroup: true);
			if (level != null)
			{
				LoadLevel(level);
			}
			else
			{
				UnityEngine.Debug.LogError("Cannot load next level: This is the last level.");
			}
		}

		public static AsyncOperation LoadNextAsync()
		{
			CheckHasConfiguration();
			MadLevelConfiguration.Level level = activeConfiguration.FindNextLevel(currentLevelName);
			if (level != null)
			{
				return LoadLevelAsync(level);
			}
			UnityEngine.Debug.LogError("Cannot load next level: This is the last level.");
			return null;
		}

		public static AsyncOperation LoadNextInGroupAsync()
		{
			CheckHasConfiguration();
			MadLevelConfiguration.Level level = activeConfiguration.FindNextLevel(currentLevelName, sameGroup: true);
			if (level != null)
			{
				return LoadLevelAsync(level);
			}
			UnityEngine.Debug.LogError("Cannot load next level: This is the last level.");
			return null;
		}

		public static void LoadNext(Type levelType)
		{
			CheckHasConfiguration();
			MadLevelConfiguration.Level level = activeConfiguration.FindNextLevel(currentLevelName, levelType);
			if (level != null)
			{
				LoadLevel(level);
			}
			else
			{
				UnityEngine.Debug.LogError("Cannot load next level: This is the last level of requested type.");
			}
		}

		public static void LoadNextInGroup(Type levelType)
		{
			CheckHasConfiguration();
			MadLevelConfiguration.Level level = activeConfiguration.FindNextLevel(currentLevelName, levelType, sameGroup: true);
			if (level != null)
			{
				LoadLevel(level);
			}
			else
			{
				UnityEngine.Debug.LogError("Cannot load next level: This is the last level of requested type.");
			}
		}

		public static AsyncOperation LoadNextAsync(Type levelType)
		{
			CheckHasConfiguration();
			MadLevelConfiguration.Level level = activeConfiguration.FindNextLevel(currentLevelName, levelType);
			if (level != null)
			{
				return LoadLevelAsync(level);
			}
			UnityEngine.Debug.LogError("Cannot load next level: This is the last level of requested type.");
			return null;
		}

		public static AsyncOperation LoadNextInGroupAsync(Type levelType)
		{
			CheckHasConfiguration();
			MadLevelConfiguration.Level level = activeConfiguration.FindNextLevel(currentLevelName, levelType, sameGroup: true);
			if (level != null)
			{
				return LoadLevelAsync(level);
			}
			UnityEngine.Debug.LogError("Cannot load next level: This is the last level of requested type.");
			return null;
		}

		public static bool HasPrevious()
		{
			CheckHasConfiguration();
			return activeConfiguration.FindPreviousLevel(currentLevelName) != null;
		}

		public static bool HasPreviousInGroup()
		{
			CheckHasConfiguration();
			return activeConfiguration.FindPreviousLevel(currentLevelName, sameGroup: true) != null;
		}

		public static bool HasPrevious(Type levelType)
		{
			CheckHasConfiguration();
			return activeConfiguration.FindPreviousLevel(currentLevelName, levelType) != null;
		}

		public static bool HasPreviousInGroup(Type levelType)
		{
			CheckHasConfiguration();
			return activeConfiguration.FindPreviousLevel(currentLevelName, levelType, sameGroup: true) != null;
		}

		public static void LoadPrevious()
		{
			CheckHasConfiguration();
			MadLevelConfiguration.Level level = activeConfiguration.FindPreviousLevel(currentLevelName);
			if (level != null)
			{
				LoadLevel(level);
			}
			else
			{
				UnityEngine.Debug.LogError("Cannot load previous level: This is the first level.");
			}
		}

		public static void LoadPreviousInGroup()
		{
			CheckHasConfiguration();
			MadLevelConfiguration.Level level = activeConfiguration.FindPreviousLevel(currentLevelName, sameGroup: true);
			if (level != null)
			{
				LoadLevel(level);
			}
			else
			{
				UnityEngine.Debug.LogError("Cannot load previous level: This is the first level.");
			}
		}

		public static AsyncOperation LoadPreviousAsync()
		{
			CheckHasConfiguration();
			MadLevelConfiguration.Level level = activeConfiguration.FindPreviousLevel(currentLevelName);
			if (level != null)
			{
				return LoadLevelAsync(level);
			}
			UnityEngine.Debug.LogError("Cannot load previous level: This is the first level.");
			return null;
		}

		public static AsyncOperation LoadPreviousInGroupAsync()
		{
			CheckHasConfiguration();
			MadLevelConfiguration.Level level = activeConfiguration.FindPreviousLevel(currentLevelName, sameGroup: true);
			if (level != null)
			{
				return LoadLevelAsync(level);
			}
			UnityEngine.Debug.LogError("Cannot load previous level: This is the first level.");
			return null;
		}

		public static void LoadPrevious(Type levelType)
		{
			CheckHasConfiguration();
			MadLevelConfiguration.Level level = activeConfiguration.FindPreviousLevel(currentLevelName, levelType);
			if (level != null)
			{
				LoadLevel(level);
			}
			else
			{
				UnityEngine.Debug.LogError("Cannot load previous level: This is the first level of requested type.");
			}
		}

		public static void LoadPreviousInGroup(Type levelType)
		{
			CheckHasConfiguration();
			MadLevelConfiguration.Level level = activeConfiguration.FindPreviousLevel(currentLevelName, levelType, sameGroup: true);
			if (level != null)
			{
				LoadLevel(level);
			}
			else
			{
				UnityEngine.Debug.LogError("Cannot load previous level: This is the first level of requested type.");
			}
		}

		public static AsyncOperation LoadPreviousAsync(Type levelType)
		{
			CheckHasConfiguration();
			MadLevelConfiguration.Level level = activeConfiguration.FindPreviousLevel(currentLevelName, levelType);
			if (level != null)
			{
				return LoadLevelAsync(level);
			}
			UnityEngine.Debug.LogError("Cannot load previous level: This is the first level of requested type.");
			return null;
		}

		public static AsyncOperation LoadPreviousInGroupAsync(Type levelType)
		{
			CheckHasConfiguration();
			MadLevelConfiguration.Level level = activeConfiguration.FindPreviousLevel(currentLevelName, levelType, sameGroup: true);
			if (level != null)
			{
				return LoadLevelAsync(level);
			}
			UnityEngine.Debug.LogError("Cannot load previous level: This is the first level of requested type.");
			return null;
		}

		public static bool HasFirst()
		{
			return activeConfiguration.LevelCount() != 0;
		}

		public static bool HasFirstInGroup()
		{
			return HasFirstInGroup(currentGroupName);
		}

		public static bool HasFirstInGroup(string groupName)
		{
			MadLevelConfiguration.Group group = activeConfiguration.FindGroupByName(groupName);
			if (group == null)
			{
				UnityEngine.Debug.LogError("There's no group named " + groupName);
				return false;
			}
			MadLevelConfiguration.Level level = activeConfiguration.GetLevel(group.id, 0);
			return level != null;
		}

		public static bool HasFirst(Type levelType)
		{
			return activeConfiguration.LevelCount(levelType) != 0;
		}

		public static bool HasFirstInGroup(Type levelType)
		{
			return HasFirstInGroup(levelType, currentGroupName);
		}

		public static bool HasFirstInGroup(Type levelType, string groupName)
		{
			MadLevelConfiguration.Group group = activeConfiguration.FindGroupByName(groupName);
			if (group == null)
			{
				UnityEngine.Debug.LogError("There's no group named " + groupName);
				return false;
			}
			MadLevelConfiguration.Level level = activeConfiguration.GetLevel(levelType, group.id, 0);
			return level != null;
		}

		public static void LoadFirst()
		{
			if (activeConfiguration.LevelCount() != 0)
			{
				MadLevelConfiguration.Level level = activeConfiguration.GetLevel(0);
				LoadLevel(level);
			}
			else
			{
				UnityEngine.Debug.LogError("Cannot load first level: there's no levels defined");
			}
		}

		public static void LoadFirstInGroup()
		{
			LoadFirstInGroup(currentGroupName);
		}

		public static void LoadFirstInGroup(string groupName)
		{
			MadLevelConfiguration.Group group = activeConfiguration.FindGroupByName(groupName);
			if (group == null)
			{
				UnityEngine.Debug.LogError("There's no group named " + groupName);
				return;
			}
			MadLevelConfiguration.Level level = activeConfiguration.GetLevel(group.id, 0);
			if (level != null)
			{
				LoadLevel(level);
			}
			else
			{
				UnityEngine.Debug.LogError("Cannot load first level: there are no levels defined in this group");
			}
		}

		public static AsyncOperation LoadFirstAsync()
		{
			if (activeConfiguration.LevelCount() != 0)
			{
				MadLevelConfiguration.Level level = activeConfiguration.GetLevel(0);
				return LoadLevelAsync(level);
			}
			UnityEngine.Debug.LogError("Cannot load first level: there are no levels defined");
			return null;
		}

		public static AsyncOperation LoadFirstInGroupAsync()
		{
			return LoadFirstInGroupAsync(currentGroupName);
		}

		public static AsyncOperation LoadFirstInGroupAsync(string groupName)
		{
			MadLevelConfiguration.Group group = activeConfiguration.FindGroupByName(groupName);
			if (group == null)
			{
				UnityEngine.Debug.LogError("There's no group named " + groupName);
				return null;
			}
			MadLevelConfiguration.Level level = activeConfiguration.GetLevel(group.id, 0);
			if (level != null)
			{
				return LoadLevelAsync(level);
			}
			UnityEngine.Debug.LogError("Cannot load first level: there are no levels defined in that group");
			return null;
		}

		public static void LoadFirst(Type levelType)
		{
			if (activeConfiguration.LevelCount(levelType) != 0)
			{
				MadLevelConfiguration.Level level = activeConfiguration.GetLevel(levelType, 0);
				LoadLevel(level);
			}
			else
			{
				UnityEngine.Debug.LogError("Cannot load first level: there's no level of type " + levelType);
			}
		}

		public static void LoadFirstInGroup(Type levelType)
		{
			LoadFirstInGroup(levelType, currentGroupName);
		}

		public static void LoadFirstInGroup(Type levelType, string groupName)
		{
			MadLevelConfiguration.Group group = activeConfiguration.FindGroupByName(groupName);
			if (group == null)
			{
				UnityEngine.Debug.LogError("There's no group named " + groupName);
				return;
			}
			MadLevelConfiguration.Level level = activeConfiguration.GetLevel(levelType, group.id, 0);
			if (level != null)
			{
				LoadLevel(level);
			}
			else
			{
				UnityEngine.Debug.LogError("Cannot find requested level.");
			}
		}

		public static AsyncOperation LoadFirstAsync(Type levelType)
		{
			if (activeConfiguration.LevelCount(levelType) != 0)
			{
				MadLevelConfiguration.Level level = activeConfiguration.GetLevel(levelType, 0);
				return LoadLevelAsync(level);
			}
			UnityEngine.Debug.LogError("Cannot load first level: there's no level of type " + levelType);
			return null;
		}

		public static AsyncOperation LoadFirstInGroupAsync(Type levelType)
		{
			return LoadFirstInGroupAsync(levelType, currentGroupName);
		}

		public static AsyncOperation LoadFirstInGroupAsync(Type levelType, string groupName)
		{
			MadLevelConfiguration.Group group = activeConfiguration.FindGroupByName(groupName);
			if (group == null)
			{
				UnityEngine.Debug.LogError("There's no group named " + groupName);
				return null;
			}
			MadLevelConfiguration.Level level = activeConfiguration.GetLevel(levelType, group.id, 0);
			if (level != null)
			{
				return LoadLevelAsync(level);
			}
			UnityEngine.Debug.LogError("Cannot find requested level");
			return null;
		}

		public static bool CanContinue()
		{
			if (currentExtension == null)
			{
				UnityEngine.Debug.LogWarning("CanContinue() should be called only within levels with extensions.");
				return false;
			}
			MadLevelConfiguration.Level currentLevel = activeConfiguration.FindLevelByName(currentLevelName);
			return currentExtension.CanContinue(currentLevel, currentExtensionProgress);
		}

		public static void Continue()
		{
			if (currentExtension == null)
			{
				UnityEngine.Debug.LogWarning("Continue() should be called only within levels with extensions.");
				return;
			}
			MadLevelConfiguration.Level currentLevel = activeConfiguration.FindLevelByName(currentLevelName);
			currentExtension.Continue(currentLevel, currentExtensionProgress);
		}

		public static AsyncOperation ContinueAsync()
		{
			if (currentExtension == null)
			{
				UnityEngine.Debug.LogWarning("Continue() should be called only within levels with extensions.");
				return null;
			}
			MadLevelConfiguration.Level currentLevel = activeConfiguration.FindLevelByName(currentLevelName);
			return currentExtension.ContinueAsync(currentLevel, currentExtensionProgress);
		}

		public static string[] GetAllLevelNames(string group = null)
		{
			CheckHasConfiguration();
			List<string> list = new List<string>();
			for (int i = 0; i < activeConfiguration.levels.Count; i++)
			{
				MadLevelConfiguration.Level level = activeConfiguration.levels[i];
				if (group == null || !(level.group.name != group))
				{
					list.Add(level.name);
				}
			}
			return list.ToArray();
		}

		public static string[] GetAllLevelNames(Type type, string group = null)
		{
			CheckHasConfiguration();
			List<string> list = new List<string>();
			for (int i = 0; i < activeConfiguration.levels.Count; i++)
			{
				MadLevelConfiguration.Level level = activeConfiguration.levels[i];
				if ((group == null || !(level.group.name != group)) && level.type == type)
				{
					list.Add(level.name);
				}
			}
			return list.ToArray();
		}

		public static string FindFirstCompletedLevelName()
		{
			return FindFirstCompletedLevelName(null);
		}

		public static string FindFirstCompletedLevelName(string groupName)
		{
			return FindFirstLevelName(groupName, (MadLevelConfiguration.Level level) => MadLevelProfile.IsCompleted(level.name));
		}

		public static string FindLastCompletedLevelName()
		{
			return FindLastCompletedLevelName(null);
		}

		public static string FindLastCompletedLevelName(string groupName)
		{
			return FindLastLevelName(groupName, (MadLevelConfiguration.Level level) => MadLevelProfile.IsCompleted(level.name));
		}

		public static string FindFirstLockedLevelName()
		{
			return FindFirstLockedLevelName(null);
		}

		public static string FindFirstLockedLevelName(string groupName)
		{
			return FindFirstLevelName(groupName, (MadLevelConfiguration.Level level) => MadLevelProfile.IsLocked(level.name));
		}

		public static string FindLastLockedLevelName()
		{
			return FindLastLockedLevelName(null);
		}

		public static string FindLastLockedLevelName(string groupName)
		{
			return FindLastLevelName(groupName, (MadLevelConfiguration.Level level) => MadLevelProfile.IsLocked(level.name));
		}

		public static string FindFirstUnlockedLevelName()
		{
			return FindFirstUnlockedLevelName(null);
		}

		public static string FindFirstUnlockedLevelName(string groupName)
		{
			LayoutUninitializedCheck();
			return FindFirstLevelName(groupName, (MadLevelConfiguration.Level level) => MadLevelProfile.IsLockedSet(level.name) ? (!MadLevelProfile.IsLocked(level.name)) : (!level.lockedByDefault));
		}

		public static string FindLastUnlockedLevelName()
		{
			return FindLastUnlockedLevelName(null);
		}

		public static string FindLastUnlockedLevelName(string groupName)
		{
			LayoutUninitializedCheck();
			return FindLastLevelName(groupName, (MadLevelConfiguration.Level level) => MadLevelProfile.IsLockedSet(level.name) ? (!MadLevelProfile.IsLocked(level.name)) : (!level.lockedByDefault));
		}

		private static void LayoutUninitializedCheck()
		{
			if (GameObject.Find("/Mad Level Root") != null)
			{
				MadLevelAbstractLayout current = MadLevelLayout.current;
				if (current != null && !current.fullyInitialized)
				{
					UnityEngine.Debug.LogWarning("This operation have unexpected behavior when executed so soon. Please move it to LateUpdate().");
				}
			}
		}

		public static string FindFirstLevelName(string groupName, LevelPredicate predicate)
		{
			CheckHasConfiguration();
			MadLevelConfiguration.Group group = null;
			if (groupName != null)
			{
				group = activeConfiguration.FindGroupByName(groupName);
				if (group == null)
				{
					UnityEngine.Debug.LogError("Cannot find group named " + groupName);
					return null;
				}
			}
			MadLevelConfiguration.Level[] levelsInOrder = activeConfiguration.GetLevelsInOrder();
			foreach (MadLevelConfiguration.Level level in levelsInOrder)
			{
				if ((group == null || level.groupId == group.id) && predicate(level))
				{
					return level.name;
				}
			}
			return null;
		}

		public static string FindFirstLevelName(LevelPredicate predicate)
		{
			return FindFirstLevelName(null, predicate);
		}

		public static string FindFirstLevelName(Type levelType, string groupName)
		{
			return FindFirstLevelName(groupName, (MadLevelConfiguration.Level level) => level.type == levelType);
		}

		public static string FindFirstLevelName(Type levelType)
		{
			return FindFirstLevelName(null, (MadLevelConfiguration.Level level) => level.type == levelType);
		}

		public static string FindLastLevelName(string groupName, LevelPredicate predicate)
		{
			CheckHasConfiguration();
			MadLevelConfiguration.Group group = null;
			if (groupName != null)
			{
				group = activeConfiguration.FindGroupByName(groupName);
				if (group == null)
				{
					UnityEngine.Debug.LogError("Cannot find group named " + groupName);
					return null;
				}
			}
			MadLevelConfiguration.Level[] levelsInOrder = activeConfiguration.GetLevelsInOrder();
			for (int num = levelsInOrder.Length - 1; num >= 0; num--)
			{
				MadLevelConfiguration.Level level = levelsInOrder[num];
				if ((group == null || level.groupId == group.id) && predicate(level))
				{
					return level.name;
				}
			}
			return null;
		}

		public static string FindLastLevelName(LevelPredicate predicate)
		{
			return FindLastLevelName(null, predicate);
		}

		public static string FindLastLevelName(Type levelType, string groupName)
		{
			return FindLastLevelName(groupName, (MadLevelConfiguration.Level level) => level.type == levelType);
		}

		public static string FindLastLevelName(Type levelType)
		{
			return FindLastLevelName(null, (MadLevelConfiguration.Level level) => level.type == levelType);
		}

		public static string GetPreviousLevelName()
		{
			CheckHasConfiguration();
			return activeConfiguration.FindPreviousLevel(currentLevelName)?.name;
		}

		public static string GetPreviousLevelName(Type type)
		{
			CheckHasConfiguration();
			return activeConfiguration.FindPreviousLevel(currentLevelName, type)?.name;
		}

		public static string GetPreviousLevelNameTo(string levelName)
		{
			CheckHasConfiguration();
			CheckLevelExists(levelName);
			return activeConfiguration.FindPreviousLevel(levelName)?.name;
		}

		public static string GetPreviousLevelNameTo(string levelName, Type type)
		{
			CheckHasConfiguration();
			CheckLevelExists(levelName);
			return activeConfiguration.FindPreviousLevel(levelName, type)?.name;
		}

		public static string GetNextLevelName()
		{
			CheckHasConfiguration();
			return activeConfiguration.FindNextLevel(currentLevelName)?.name;
		}

		public static string GetNextLevelName(Type type)
		{
			CheckHasConfiguration();
			return activeConfiguration.FindNextLevel(currentLevelName, type)?.name;
		}

		public static string GetNextLevelNameTo(string levelName)
		{
			CheckHasConfiguration();
			CheckLevelExists(levelName);
			return activeConfiguration.FindNextLevel(levelName)?.name;
		}

		public static string GetNextLevelNameTo(string levelName, Type type)
		{
			CheckHasConfiguration();
			CheckLevelExists(levelName);
			return activeConfiguration.FindNextLevel(levelName, type)?.name;
		}

		public static int GetOrdeal(string levelName, Type type)
		{
			int num = 1;
			for (int i = 0; i < activeConfiguration.levels.Count; i++)
			{
				MadLevelConfiguration.Level level = activeConfiguration.levels[i];
				if (level.name == levelName)
				{
					return num;
				}
				if (level.type == type)
				{
					num++;
				}
			}
			return -1;
		}

		private static void CheckLevelExists(string levelName)
		{
			MadLevelConfiguration.Level level = activeConfiguration.FindLevelByName(levelName);
			MadDebug.Assert(level != null, "There's no level with name '" + levelName + "'");
		}

		private static bool CheckLevelLoading()
		{
			return true;
		}

		private static void CheckHasConfiguration()
		{
			MadDebug.Assert(hasActiveConfiguration, "This method may only be used when level configuration is set. Please refer to http://madlevelmanager.madpixelmachine.com/doc/latest/basics/working_with_level_configurations.html");
		}

		private static void LoadLevel(MadLevelConfiguration.Level level)
		{
			if (CheckLevelLoading())
			{
				currentExtension = null;
				if (level.hasExtension)
				{
					MadLevelExtension extension = level.extension;
					extension.Load(level);
				}
				else
				{
					level.Load();
				}
			}
		}

		private static AsyncOperation LoadLevelAsync(MadLevelConfiguration.Level level)
		{
			if (!CheckLevelLoading())
			{
				return null;
			}
			currentExtension = null;
			if (level.hasExtension)
			{
				MadLevelExtension extension = level.extension;
				return extension.LoadAsync(level);
			}
			return level.LoadAsync();
		}
	}
}
