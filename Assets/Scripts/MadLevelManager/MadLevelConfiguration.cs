using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MadLevelManager
{
	[ExecuteInEditMode]
	public class MadLevelConfiguration : ScriptableObject
	{
		[Serializable]
		public class Group
		{
			public string name = "New Group";

			[SerializeField]
			private int _id;

			[SerializeField]
			internal MadLevelConfiguration parent;

			public int id
			{
				get
				{
					return _id;
				}
				private set
				{
					_id = value;
				}
			}

			internal Group(MadLevelConfiguration parent, int id)
			{
				MadDebug.Assert(parent != null, "Parent cannot be null");
				this.parent = parent;
				this.id = id;
			}

			public List<Level> GetLevels()
			{
				IEnumerable<Level> source = from l in parent.levels
					where l.groupId == id
					select l;
				return source.ToList();
			}

			public override int GetHashCode()
			{
				int num = 17;
				num = num * 31 + ((name != null) ? name.GetHashCode() : 0);
				return num * 31 + id.GetHashCode();
			}
		}

		[Serializable]
		public class Level : MadLevelScene
		{
			[SerializeField]
			internal MadLevelConfiguration parent;

			public int order;

			public string name = "New Level";

			public MadLevel.Type type;

			public string arguments = string.Empty;

			public int groupId;

			public string extensionGUID = string.Empty;

			public bool lockedByDefault = true;

			public Group group
			{
				get
				{
					return parent.FindGroupById(groupId);
				}
				set
				{
					MadDebug.Assert(value == parent.defaultGroup || parent.groups.Contains(value), "Unknown group: " + value);
					groupId = value.id;
				}
			}

			public bool hasExtension => extension != null;

			public MadLevelExtension extension
			{
				get
				{
					return parent.FindExtensionByGUID(extensionGUID);
				}
				set
				{
					if (value == null)
					{
						extensionGUID = string.Empty;
						return;
					}
					int num = parent.extensions.FindIndex((MadLevelExtension e) => e == value);
					if (num != -1)
					{
						extensionGUID = value.guid;
					}
					else
					{
						UnityEngine.Debug.LogError("Trying to add extesion to a level that is not in the configuration");
					}
				}
			}

			internal Level(MadLevelConfiguration parent)
			{
				this.parent = parent;
			}

			public Group GetGroup()
			{
				return group;
			}

			public override void Load()
			{
				MadLevel.lastPlayedLevelName = MadLevel.currentLevelName;
				MadLevel.arguments = arguments;
				MadLevel.currentLevelName = name;
				SceneManager.LoadScene(base.sceneName);
			}

			public override AsyncOperation LoadAsync()
			{
				MadLevel.lastPlayedLevelName = MadLevel.currentLevelName;
				MadLevel.arguments = arguments;
				MadLevel.currentLevelName = name;
				return SceneManager.LoadSceneAsync(base.sceneName);
			}

			public override bool IsValid()
			{
				if (!base.IsValid())
				{
					return false;
				}
				return !string.IsNullOrEmpty(name) && !HasDuplicatedName();
			}

			public bool HasDuplicatedName()
			{
				foreach (Level level in parent.levels)
				{
					if (level != this && level.name == name)
					{
						return true;
					}
				}
				return false;
			}

			public override int GetHashCode()
			{
				int num = 17;
				num = num * 31 + order.GetHashCode();
				num = num * 31 + ((base.sceneObject != null) ? base.sceneObject.GetHashCode() : 0);
				num = num * 31 + name.GetHashCode();
				num = num * 31 + type.GetHashCode();
				return num * 31 + arguments.GetHashCode();
			}

			public List<MadLevelScene> StandaloneScenes()
			{
				List<MadLevelScene> list = new List<MadLevelScene>();
				list.Add(this);
				if (extension != null)
				{
					list.AddRange(extension.scenesAfter);
				}
				return list;
			}
		}

		public delegate void Callback0();

		[SerializeField]
		private int creationDate = -1;

		[SerializeField]
		private bool _active;

		public List<Level> levels = new List<Level>();

		public List<Group> groups = new List<Group>();

		public List<MadLevelExtension> extensions = new List<MadLevelExtension>();

		[NonSerialized]
		public Callback0 callbackChanged = delegate
		{
		};

		[SerializeField]
		private int version;

		[SerializeField]
		public int flag;

		private Group _defaultGroup;

		public bool active
		{
			get
			{
				return _active;
			}
			set
			{
				_active = value;
				if (value)
				{
					DeactivateOthers();
				}
				SetDirty();
			}
		}

		public Group defaultGroup
		{
			get
			{
				if (_defaultGroup == null || _defaultGroup.parent == null)
				{
					_defaultGroup = new Group(this, 0);
					_defaultGroup.name = "(default)";
				}
				return _defaultGroup;
			}
		}

		public override int GetHashCode()
		{
			int num = 17;
			foreach (Level level in levels)
			{
				num = num * 31 + level.GetHashCode();
			}
			foreach (Group group in groups)
			{
				num = num * 31 + group.GetHashCode();
			}
			return num;
		}

		private void OnEnable()
		{
			Upgrade();
			Reorder();
		}

		private void Upgrade()
		{
			foreach (Level level3 in levels)
			{
				level3.parent = this;
				level3.Upgrade();
				if (version == 0)
				{
					level3.lockedByDefault = true;
				}
			}
			if (version == 0)
			{
				IOrderedEnumerable<Level> source = from l in levels
					where l.type == MadLevel.Type.Level && l.groupId == defaultGroup.id
					orderby l.order
					select l;
				Level level = source.FirstOrDefault();
				if (level != null)
				{
					level.lockedByDefault = false;
				}
				foreach (Group g in groups)
				{
					IOrderedEnumerable<Level> source2 = from l in levels
						where l.type == MadLevel.Type.Level && l.groupId == g.id
						orderby l.order
						select l;
					Level level2 = source2.FirstOrDefault();
					if (level2 != null)
					{
						level2.lockedByDefault = false;
					}
				}
				version = 1;
			}
		}

		public new void SetDirty()
		{
			Reorder();
			callbackChanged();
		}

		private void Reorder()
		{
			int num = 0;
			Level[] levelsInOrder = GetLevelsInOrder();
			Level[] array = levelsInOrder;
			foreach (Level level in array)
			{
				level.order = num;
				num += 10;
			}
			levels.Clear();
			levels.AddRange(levelsInOrder);
		}

		public Level CreateLevel()
		{
			return new Level(this);
		}

		public Group CreateGroup()
		{
			int num = 1;
			foreach (Group group in groups)
			{
				num = Mathf.Max(num, group.id + 1);
			}
			return new Group(this, num);
		}

		public void AddGroup(Group group)
		{
			groups.Add(group);
			SetDirty();
		}

		public Group FindGroupById(int groupId)
		{
			if (groupId == defaultGroup.id)
			{
				return defaultGroup;
			}
			IEnumerable<Group> source = from g in groups
				where g.id == groupId
				select g;
			return source.FirstOrDefault();
		}

		public Group FindGroupByName(string groupName)
		{
			if (defaultGroup.name == groupName)
			{
				return defaultGroup;
			}
			IEnumerable<Group> source = from g in groups
				where g.name == groupName
				select g;
			return source.FirstOrDefault();
		}

		public int LevelCount()
		{
			return levels.Count;
		}

		public int LevelCount(MadLevel.Type type)
		{
			return LevelCount(type, -1);
		}

		public int LevelCount(MadLevel.Type type, int groupId)
		{
			if (groupId == -1)
			{
				IEnumerable<Level> source = from level in levels
					where level.type == type
					select level;
				return source.Count();
			}
			IEnumerable<Level> source2 = from level in levels
				where level.type == type && level.groupId == groupId
				select level;
			return source2.Count();
		}

		public Level[] GetLevelsInOrder()
		{
			List<Level> list = new List<Level>();
			int i;
			for (i = -1; i < groups.Count; i++)
			{
				if (i == -1)
				{
					IOrderedEnumerable<Level> collection = from l in levels
						where l.groupId == defaultGroup.id
						orderby l.order
						select l;
					list.AddRange(collection);
				}
				else
				{
					IOrderedEnumerable<Level> collection2 = from l in levels
						where l.groupId == groups[i].id
						orderby l.order
						select l;
					list.AddRange(collection2);
				}
			}
			return list.ToArray();
		}

		public Level GetLevel(int index)
		{
			return GetLevel(-1, index);
		}

		public Level GetLevel(int groupId, int index)
		{
			Level[] array;
			if (groupId == -1)
			{
				IOrderedEnumerable<Level> source = from l in levels
					orderby l.order
					select l;
				array = source.ToArray();
			}
			else
			{
				IOrderedEnumerable<Level> source2 = from l in levels
					where l.groupId == groupId
					orderby l.order
					select l;
				array = source2.ToArray();
			}
			if (array.Length > index)
			{
				return array[index];
			}
			return null;
		}

		public Level GetLevel(MadLevel.Type type, int index)
		{
			return GetLevel(type, -1, index);
		}

		public Level GetLevel(MadLevel.Type type, int groupId, int index)
		{
			int num = 0;
			for (int i = 0; i < levels.Count; i++)
			{
				Level level = levels[i];
				if (level.type == type && (groupId == -1 || level.groupId == groupId))
				{
					if (num == index)
					{
						return level;
					}
					num++;
				}
			}
			return null;
		}

		public int FindLevelIndex(MadLevel.Type type, string levelName)
		{
			return FindLevelIndex(type, -1, levelName);
		}

		public int FindLevelIndex(MadLevel.Type type, int groupId, string levelName)
		{
			List<Level> list;
			if (groupId == -1)
			{
				IOrderedEnumerable<Level> source = from l in levels
					where l.type == type
					orderby l.order
					select l;
				list = source.ToList();
			}
			else
			{
				IOrderedEnumerable<Level> source2 = from l in levels
					where l.type == type && l.groupId == groupId
					orderby l.order
					select l;
				list = source2.ToList();
			}
			int num = 0;
			foreach (Level item in list)
			{
				if (item.name == levelName)
				{
					return num;
				}
				num++;
			}
			return -1;
		}

		public Level FindLevelByName(string levelName)
		{
			IEnumerable<Level> source = from l in levels
				where l.name == levelName
				select l;
			return source.FirstOrDefault();
		}

		public Level FindNextLevel(string currentLevelName)
		{
			return FindNextLevel(currentLevelName, sameGroup: false);
		}

		public Level FindNextLevel(string currentLevelName, bool sameGroup)
		{
			Level currentLevel = FindLevelByName(currentLevelName);
			MadDebug.Assert(currentLevel != null, "Cannot find level " + currentLevelName);
			if (sameGroup)
			{
				IOrderedEnumerable<Level> source = from l in levels
					where l.groupId == currentLevel.groupId && l.order > currentLevel.order
					orderby l.order
					select l;
				return source.FirstOrDefault();
			}
			IOrderedEnumerable<Level> source2 = from l in levels
				where l.order > currentLevel.order
				orderby l.order
				select l;
			return source2.FirstOrDefault();
		}

		public Level FindNextLevel(string currentLevelName, MadLevel.Type type)
		{
			return FindNextLevel(currentLevelName, type, sameGroup: false);
		}

		public Level FindNextLevel(string currentLevelName, MadLevel.Type type, bool sameGroup)
		{
			Level currentLevel = FindLevelByName(currentLevelName);
			MadDebug.Assert(currentLevel != null, "Cannot find level " + currentLevelName);
			if (sameGroup)
			{
				IOrderedEnumerable<Level> source = from l in levels
					where l.groupId == currentLevel.groupId && l.order > currentLevel.order && l.type == type
					orderby l.order
					select l;
				return source.FirstOrDefault();
			}
			IOrderedEnumerable<Level> source2 = from l in levels
				where l.order > currentLevel.order && l.type == type
				orderby l.order
				select l;
			return source2.FirstOrDefault();
		}

		public Level FindPreviousLevel(string currentLevelName)
		{
			return FindPreviousLevel(currentLevelName, sameGroup: false);
		}

		public Level FindPreviousLevel(string currentLevelName, bool sameGroup)
		{
			Level currentLevel = FindLevelByName(currentLevelName);
			MadDebug.Assert(currentLevel != null, "Cannot find level " + currentLevelName);
			if (sameGroup)
			{
				IOrderedEnumerable<Level> source = from l in levels
					where l.groupId == currentLevel.groupId && l.order < currentLevel.order
					orderby l.order descending
					select l;
				return source.FirstOrDefault();
			}
			IOrderedEnumerable<Level> source2 = from l in levels
				where l.order < currentLevel.order
				orderby l.order descending
				select l;
			return source2.FirstOrDefault();
		}

		public Level FindPreviousLevel(string currentLevelName, MadLevel.Type type)
		{
			return FindPreviousLevel(currentLevelName, type, sameGroup: false);
		}

		public Level FindPreviousLevel(string currentLevelName, MadLevel.Type type, bool sameGroup)
		{
			Level currentLevel = FindLevelByName(currentLevelName);
			MadDebug.Assert(currentLevel != null, "Cannot find level " + currentLevelName);
			if (sameGroup)
			{
				IOrderedEnumerable<Level> source = from l in levels
					where l.groupId == currentLevel.groupId && l.order < currentLevel.order && l.type == type
					orderby l.order descending
					select l;
				return source.FirstOrDefault();
			}
			IOrderedEnumerable<Level> source2 = from l in levels
				where l.order < currentLevel.order && l.type == type
				orderby l.order descending
				select l;
			return source2.FirstOrDefault();
		}

		public Level FindFirstForScene(string levelName, out bool hasMany)
		{
			IOrderedEnumerable<Level> source = from l in levels
				where l.sceneName == levelName
				orderby l.order
				select l;
			if (source.Count() > 1)
			{
				hasMany = true;
			}
			else
			{
				hasMany = false;
			}
			return source.FirstOrDefault();
		}

		public MadLevelExtension FindExtensionByGUID(string guid)
		{
			IEnumerable<MadLevelExtension> source = from e in extensions
				where e.guid == guid
				select e;
			return source.FirstOrDefault();
		}

		public List<MadLevelScene> ScenesInOrder()
		{
			List<MadLevelScene> list = new List<MadLevelScene>();
			IOrderedEnumerable<Level> orderedEnumerable = from l in levels
				orderby l.groupId, l.order
				select l;
			bool flag = true;
			foreach (Level item in orderedEnumerable)
			{
				list.Add(item);
				if (flag)
				{
					foreach (MadLevelExtension extension in extensions)
					{
						list.AddRange(extension.scenesBefore);
						list.AddRange(extension.scenesAfter);
					}
					flag = false;
				}
			}
			return list;
		}

		public static MadLevelConfiguration[] FindAll()
		{
			List<MadLevelConfiguration> list = new List<MadLevelConfiguration>();
			UnityEngine.Object[] array = Resources.LoadAll("LevelConfig", typeof(MadLevelConfiguration));
			UnityEngine.Object[] array2 = array;
			foreach (UnityEngine.Object @object in array2)
			{
				list.Add(@object as MadLevelConfiguration);
			}
			return list.ToArray();
		}

		public static MadLevelConfiguration GetActive()
		{
			MadLevelConfiguration madLevelConfiguration = FindActive();
			if (madLevelConfiguration == null)
			{
				UnityEngine.Debug.LogWarning("There's no active configuration. Please make at least one!");
			}
			return madLevelConfiguration;
		}

		public static MadLevelConfiguration FindActive()
		{
			MadLevelConfiguration[] source = FindAll();
			IOrderedEnumerable<MadLevelConfiguration> source2 = from conf in source
				where conf.active
				orderby conf.creationDate descending
				select conf;
			MadLevelConfiguration madLevelConfiguration = source2.FirstOrDefault();
			if (source2.Count() > 1)
			{
				UnityEngine.Debug.Log("There are more than one active configuration. This shouldn't happen (unless you've just upgraded Mad Level Manager). Anyway there's nothing to worry about! I will now use " + madLevelConfiguration.name + " and deactivate others.", madLevelConfiguration);
				madLevelConfiguration.active = true;
			}
			return madLevelConfiguration;
		}

		private void DeactivateOthers()
		{
			MadLevelConfiguration[] array = FindAll();
			MadLevelConfiguration[] array2 = array;
			foreach (MadLevelConfiguration madLevelConfiguration in array2)
			{
				if (madLevelConfiguration != this)
				{
					madLevelConfiguration.active = false;
				}
			}
		}

		public bool IsValid()
		{
			foreach (Level level in levels)
			{
				if (!level.IsValid())
				{
					return false;
				}
				if (level.sceneObject == null)
				{
					return false;
				}
			}
			foreach (MadLevelExtension extension in extensions)
			{
				if (!extension.IsValid())
				{
					return false;
				}
			}
			return true;
		}

		public void ApplyProfile()
		{
			for (int i = 0; i < levels.Count; i++)
			{
				Level level = levels[i];
				if (!MadLevelProfile.IsLockedSet(level.name))
				{
					MadLevelProfile.SetLocked(level.name, level.lockedByDefault);
				}
			}
		}
	}
}
