using MadLevelManager.Backend;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MadLevelManager
{
	public class MadLevelProfile
	{
		private class Level
		{
			private const int Version = 3;

			private const int LowestSupportedVersion = 1;

			private const string SpaceSubstitue = "%20";

			public string name;

			private Dictionary<string, PropertyValue> properties = new Dictionary<string, PropertyValue>();

			public Level(string name)
			{
				this.name = name;
			}

			public List<string> GetPropertyNames()
			{
				return properties.Keys.ToList();
			}

			public bool SetPropertyBoolean(string key, bool val)
			{
				PropertyValue propertyValue = PropertyValue.FromBoolean(val);
				if (properties.ContainsKey(key) && properties[key].Equals(propertyValue))
				{
					return false;
				}
				properties[key] = propertyValue;
				return true;
			}

			public bool SetPropertyInteger(string key, int val)
			{
				PropertyValue propertyValue = PropertyValue.FromInteger(val);
				if (properties.ContainsKey(key) && properties[key].Equals(propertyValue))
				{
					return false;
				}
				properties[key] = propertyValue;
				return true;
			}

			public bool SetPropertyFloat(string key, float val)
			{
				PropertyValue propertyValue = PropertyValue.FromFloat(val);
				if (properties.ContainsKey(key) && properties[key].Equals(propertyValue))
				{
					return false;
				}
				properties[key] = propertyValue;
				return true;
			}

			public bool SetPropertyString(string key, string val)
			{
				PropertyValue propertyValue = PropertyValue.FromString(val);
				if (properties.ContainsKey(key) && properties[key].Equals(propertyValue))
				{
					return false;
				}
				properties[key] = propertyValue;
				return true;
			}

			public bool HasProperty(string key)
			{
				return properties.ContainsKey(key);
			}

			public bool GetPropertyBoolean(string key)
			{
				return properties[key].BooleanValue();
			}

			public int GetPropertyInteger(string key)
			{
				return properties[key].IntegerValue();
			}

			public float GetPropertyFloat(string key)
			{
				return properties[key].FloatValue();
			}

			public string GetPropertyString(string key)
			{
				return properties[key].StringValue();
			}

			public string GetPropertyAny(string key)
			{
				return properties[key].AnyValue();
			}

			public static Level Read(string line)
			{
				string[] array = line.Split(' ');
				int num = int.Parse(array[0]);
				if (num != 3)
				{
					if (num < 1)
					{
						UnityEngine.Debug.LogError($"Expected version {3} but {num} found");
						return null;
					}
					UnityEngine.Debug.Log($"Current save version is {3}, but {num} found. Will be upgraded.");
				}
				string text = In(array[1]);
				Level level = new Level(text);
				for (int i = 2; i < array.Length; i += 2)
				{
					string text2 = In(array[i]);
					PropertyValue value;
					if (num == 1)
					{
						bool val = bool.Parse(In(array[i + 1]));
						value = PropertyValue.FromBoolean(val);
					}
					else
					{
						value = PropertyValue.Read(array[i + 1]);
					}
					if (num <= 2 && text2 == "locked")
					{
						text2 = "@locked@";
						if (!level.properties.ContainsKey(text2))
						{
							level.properties.Add(text2, value);
						}
					}
					else
					{
						level.properties.Add(text2, value);
					}
				}
				return level;
			}

			public string Write()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append($"{3} {Out(name)}");
				foreach (string key in properties.Keys)
				{
					stringBuilder.Append(" ");
					stringBuilder.Append(Out(key));
					stringBuilder.Append(" ");
					stringBuilder.Append(properties[key].Write());
				}
				return stringBuilder.ToString();
			}

			private static string Out(string str)
			{
				str = str.Replace("%", "%25");
				str = str.Replace(" ", "%20");
				return str;
			}

			private static string In(string str)
			{
				str = str.Replace("%20", " ");
				str = str.Replace("%25", "%");
				return str;
			}
		}

		private class PropertyValue
		{
			public enum Type
			{
				Boolean,
				Integer,
				Float,
				String
			}

			public readonly Type type;

			public readonly string strValue;

			private PropertyValue(Type t, string s)
			{
				type = t;
				strValue = s;
			}

			public static PropertyValue FromBoolean(bool val)
			{
				return new PropertyValue(Type.Boolean, val.ToString());
			}

			public static PropertyValue FromInteger(int val)
			{
				return new PropertyValue(Type.Integer, val.ToString());
			}

			public static PropertyValue FromFloat(float val)
			{
				return new PropertyValue(Type.Float, val.ToString());
			}

			public static PropertyValue FromString(string val)
			{
				return new PropertyValue(Type.String, val);
			}

			public string Write()
			{
				string text = strValue;
				if (type == Type.String)
				{
					byte[] bytes = Encoding.UTF8.GetBytes(text.ToCharArray(), 0, text.Length);
					text = Convert.ToBase64String(bytes);
				}
				return $"{TypeToString()}:{text}";
			}

			public static PropertyValue Read(string data)
			{
				Type type = StringToType(data);
				string s = data.Substring(2);
				if (type == Type.String)
				{
					byte[] array = Convert.FromBase64String(s);
					s = Encoding.UTF8.GetString(array, 0, array.Length);
				}
				return new PropertyValue(type, s);
			}

			private string TypeToString()
			{
				switch (type)
				{
				case Type.Boolean:
					return "b";
				case Type.Integer:
					return "i";
				case Type.Float:
					return "f";
				case Type.String:
					return "s";
				default:
					MadDebug.Assert(condition: false, "Unknown type: " + type);
					return "?";
				}
			}

			private static Type StringToType(string str)
			{
				string text = str.Substring(0, 2);
				if (text == "b:")
				{
					return Type.Boolean;
				}
				if (text == "i:")
				{
					return Type.Integer;
				}
				if (text == "f:")
				{
					return Type.Float;
				}
				if (text == "s:")
				{
					return Type.String;
				}
				MadDebug.Assert(condition: false, "Unknown type prefix: " + text);
				return Type.Boolean;
			}

			public bool BooleanValue()
			{
				MadDebug.Assert(type == Type.Boolean, "Property type is " + type);
				return bool.Parse(strValue);
			}

			public int IntegerValue()
			{
				MadDebug.Assert(type == Type.Integer, "Property type is " + type);
				return int.Parse(strValue);
			}

			public float FloatValue()
			{
				MadDebug.Assert(type == Type.Float, "Property type is " + type);
				return float.Parse(strValue);
			}

			public string StringValue()
			{
				MadDebug.Assert(type == Type.String, "Property type is " + type);
				return strValue;
			}

			public string AnyValue()
			{
				return strValue;
			}

			public override bool Equals(object obj)
			{
				if (!(obj is PropertyValue))
				{
					return false;
				}
				PropertyValue propertyValue = obj as PropertyValue;
				if (type != propertyValue.type)
				{
					return false;
				}
				if (strValue != propertyValue.strValue)
				{
					return false;
				}
				return true;
			}

			public override int GetHashCode()
			{
				int num = 37;
				num += num * 17 + type.GetHashCode();
				return num + (num * 17 + ((strValue != null) ? strValue.GetHashCode() : 0));
			}
		}

		[DisplayedName("Default")]
		public class DefaultBackend : IMadLevelProfileBackend
		{
			public void Start()
			{
			}

			public string LoadProfile(string profileName)
			{
				return PlayerPrefs.GetString(profileName + "__levels", string.Empty);
			}

			public void SaveProfile(string profileName, string value)
			{
				PlayerPrefs.SetString(profileName + "__levels", value);
			}

			public void Flush()
			{
				PlayerPrefs.Save();
			}

			public bool CanWorkInEditMode()
			{
				return true;
			}
		}

		private const string ProfileLevelName = "__profile__";

		private const string KeyProfileCurrent = "profile_current";

		private const string KeyProfileList = "profile_list";

		private const string KeyRecentLevelSelected = "recent_level_selected";

		private const string KeyRecentLevelScreen = "recent_level_screen";

		private const string KeyLevels = "_levels";

		public const string DefaultProfile = "_default";

		private const string PropertyCompleted = "@completed@";

		private const string PropertyLocked = "@locked@";

		private static IMadLevelProfileBackend _backend;

		private static Level _profileLevel;

		private static Dictionary<string, Level> _levels;

		private static bool initialized;

		public static IMadLevelProfileBackend backend
		{
			get
			{
				if (_backend == null)
				{
					if (MadLevelSettings.current == null)
					{
						_backend = new DefaultBackend();
					}
					else
					{
						_backend = MadLevelSettings.current.CreateBackend();
					}
				}
				return _backend;
			}
			set
			{
				if (_backend != null)
				{
					if (_backend is DefaultBackend)
					{
						UnityEngine.Debug.LogError("The backend is already set to default. Most probably you're trying to override it too late!");
					}
					else
					{
						UnityEngine.Debug.LogWarning("The backend is already set. Are you trying to set it more than once?");
					}
				}
				else
				{
					_backend = value;
				}
			}
		}

		private static Level profileLevel
		{
			get
			{
				if (_profileLevel == null)
				{
					LoadProfile();
				}
				return _profileLevel;
			}
		}

		private static Dictionary<string, Level> levels
		{
			get
			{
				if (_levels == null)
				{
					LoadProfile();
				}
				return _levels;
			}
		}

		public static string profile
		{
			get
			{
				return PlayerPrefs.GetString("profile_current", "_default");
			}
			set
			{
				if (value != profile)
				{
					RegisterProfile(value);
					PlayerPrefs.SetString("profile_current", value);
					PlayerPrefs.Save();
					_profileLevel = null;
					_levels = null;
				}
			}
		}

		public static string[] profileList
		{
			get
			{
				string text = PlayerPrefs.GetString("profile_list");
				if (string.IsNullOrEmpty(text))
				{
					text = "_default";
				}
				string[] array = text.Split(' ');
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = array[i].Replace("%20", " ");
				}
				return array;
			}
			set
			{
				for (int i = 0; i < value.Length; i++)
				{
					value[i] = value[i].Replace(" ", "%20");
				}
				string value2 = string.Join(" ", value);
				PlayerPrefs.SetString("profile_list", value2);
				PlayerPrefs.Save();
			}
		}

		[Obsolete("Use MadLevel.currentLevelName instead.")]
		public static string recentLevelSelected
		{
			get
			{
				return PlayerPrefs.GetString(profile + "_recent_level_selected");
			}
			set
			{
				PlayerPrefs.SetString(profile + "_recent_level_selected", value);
				PlayerPrefs.Save();
			}
		}

		public static void LoadProfileFromString(string str)
		{
			try
			{
				Dictionary<string, Level> dictionary = new Dictionary<string, Level>();
				using (StringReader stringReader = new StringReader(str))
				{
					while (true)
					{
						string text = stringReader.ReadLine();
						if (text != null)
						{
							text = text.Trim();
						}
						if (string.IsNullOrEmpty(text))
						{
							break;
						}
						Level level = Level.Read(text);
						if (level != null)
						{
							if (level.name == "__profile__")
							{
								_profileLevel = level;
							}
							else
							{
								dictionary.Add(level.name, level);
							}
						}
					}
				}
				_levels = dictionary;
				if (_profileLevel == null)
				{
					_profileLevel = new Level("__profile__");
				}
				WriteProfile();
			}
			catch (Exception)
			{
				UnityEngine.Debug.LogError("Something went wrong when reading profile string. Please copy this message and send it to support@madpixelmachine.com\nPROFILE_STRING:\n" + str + "\nEND_OF_PROFILE_STRING");
				throw;
			}
		}

		public static void Init()
		{
			if (!initialized)
			{
				LoadProfile();
			}
		}

		public static void Reload()
		{
			LoadProfile();
		}

		private static void LoadProfile()
		{
			initialized = true;
			string str = backend.LoadProfile(profile);
			LoadProfileFromString(str);
			ApplyConfigurationProfile();
		}

		private static void ApplyConfigurationProfile()
		{
			MadLevelConfiguration activeConfiguration = MadLevel.activeConfiguration;
			if (activeConfiguration != null)
			{
				activeConfiguration.ApplyProfile();
			}
		}

		public static string SaveProfileToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (Level value in levels.Values)
			{
				stringBuilder.AppendLine(value.Write());
			}
			stringBuilder.AppendLine(profileLevel.Write());
			return stringBuilder.ToString();
		}

		private static void WriteProfile()
		{
			string value = SaveProfileToString();
			backend.SaveProfile(profile, value);
		}

		private static void EraseProfileData()
		{
			PlayerPrefs.DeleteKey(profile + "__levels");
		}

		public static void RegisterProfile(string profileName)
		{
			string[] array = profileList;
			if (Array.Find(array, (string obj) => obj == profileName) == null)
			{
				Array.Resize(ref array, array.Length + 1);
				array[array.Length - 1] = profileName;
				profileList = array;
			}
		}

		public static void UnregisterProfile(string profileName)
		{
			if (profileName == "_default")
			{
				UnityEngine.Debug.LogWarning("Cannot unregister default profile");
				return;
			}
			MadDebug.Assert(MadLevelProfile.profileList.Contains(profileName), "No profile called '" + profileName + "' found.");
			if (MadLevelProfile.profile != profileName)
			{
				string profile = MadLevelProfile.profile;
				MadLevelProfile.profile = profileName;
				EraseProfileData();
				MadLevelProfile.profile = profile;
			}
			else
			{
				EraseProfileData();
				MadLevelProfile.profile = "_default";
			}
			List<string> list = new List<string>();
			string[] profileList = MadLevelProfile.profileList;
			string[] array = profileList;
			foreach (string text in array)
			{
				if (text != profileName)
				{
					list.Add(text);
				}
			}
			MadLevelProfile.profileList = list.ToArray();
		}

		public static List<string> GetLevelNames()
		{
			List<string> list = new List<string>();
			foreach (KeyValuePair<string, Level> level in levels)
			{
				list.Add(level.Key);
			}
			return list;
		}

		public static List<string> GetLevelPropertyNames(string levelName)
		{
			if (levels.ContainsKey(levelName))
			{
				Level level = levels[levelName];
				return level.GetPropertyNames();
			}
			return new List<string>();
		}

		public static List<string> GetProfilePropertyNames()
		{
			return profileLevel.GetPropertyNames();
		}

		public static string GetLevelAny(string levelName, string property)
		{
			return GetLevelAny(levelName, property, string.Empty);
		}

		public static string GetLevelAny(string levelName, string property, string def)
		{
			if (levels.ContainsKey(levelName))
			{
				Level level = levels[levelName];
				if (level.HasProperty(property))
				{
					return level.GetPropertyAny(property);
				}
				return def;
			}
			return def;
		}

		public static string GetProfileAny(string property)
		{
			if (IsProfilePropertySet(property))
			{
				return profileLevel.GetPropertyAny(property);
			}
			return string.Empty;
		}

		public static bool GetLevelBoolean(string levelName, string property)
		{
			return GetLevelBoolean(levelName, property, def: false);
		}

		public static bool GetLevelBoolean(string levelName, string property, bool def)
		{
			if (levels.ContainsKey(levelName))
			{
				Level level = levels[levelName];
				if (level.HasProperty(property))
				{
					return level.GetPropertyBoolean(property);
				}
				return def;
			}
			return def;
		}

		public static void SetLevelBoolean(string levelName, string property, bool val)
		{
			Level level;
			if (!levels.ContainsKey(levelName))
			{
				level = new Level(levelName);
				levels.Add(levelName, level);
			}
			else
			{
				level = levels[levelName];
			}
			if (level.SetPropertyBoolean(property, val))
			{
				WriteProfile();
			}
		}

		public static void SetProfileBoolean(string property, bool val)
		{
			if (profileLevel.SetPropertyBoolean(property, val))
			{
				WriteProfile();
			}
		}

		public static bool GetProfileBoolean(string property)
		{
			return GetProfileBoolean(property, def: false);
		}

		public static bool GetProfileBoolean(string property, bool def)
		{
			if (IsProfilePropertySet(property))
			{
				return profileLevel.GetPropertyBoolean(property);
			}
			return def;
		}

		public static int GetLevelInteger(string levelName, string property)
		{
			return GetLevelInteger(levelName, property, 0);
		}

		public static int GetLevelInteger(string levelName, string property, int def)
		{
			if (levels.ContainsKey(levelName))
			{
				Level level = levels[levelName];
				if (level.HasProperty(property))
				{
					return level.GetPropertyInteger(property);
				}
				return def;
			}
			return def;
		}

		public static void SetLevelInteger(string levelName, string property, int val)
		{
			Level level;
			if (!levels.ContainsKey(levelName))
			{
				level = new Level(levelName);
				levels.Add(levelName, level);
			}
			else
			{
				level = levels[levelName];
			}
			if (level.SetPropertyInteger(property, val))
			{
				WriteProfile();
			}
		}

		public static void SetProfileInteger(string property, int val)
		{
			if (profileLevel.SetPropertyInteger(property, val))
			{
				WriteProfile();
			}
		}

		public static int GetProfileInteger(string property)
		{
			return GetProfileInteger(property, 0);
		}

		public static int GetProfileInteger(string property, int def)
		{
			if (IsProfilePropertySet(property))
			{
				return profileLevel.GetPropertyInteger(property);
			}
			return def;
		}

		public static float GetLevelFloat(string levelName, string property)
		{
			return GetLevelFloat(levelName, property, 0f);
		}

		public static float GetLevelFloat(string levelName, string property, float def)
		{
			if (levels.ContainsKey(levelName))
			{
				Level level = levels[levelName];
				if (level.HasProperty(property))
				{
					return level.GetPropertyFloat(property);
				}
				return def;
			}
			return def;
		}

		public static void SetLevelFloat(string levelName, string property, float val)
		{
			Level level;
			if (!levels.ContainsKey(levelName))
			{
				level = new Level(levelName);
				levels.Add(levelName, level);
			}
			else
			{
				level = levels[levelName];
			}
			if (level.SetPropertyFloat(property, val))
			{
				WriteProfile();
			}
		}

		public static void SetProfileFloat(string property, float val)
		{
			if (profileLevel.SetPropertyFloat(property, val))
			{
				WriteProfile();
			}
		}

		public static float GetProfileFloat(string property)
		{
			return GetProfileFloat(property, 0f);
		}

		public static float GetProfileFloat(string property, float def)
		{
			if (IsProfilePropertySet(property))
			{
				return profileLevel.GetPropertyFloat(property);
			}
			return def;
		}

		public static string GetLevelString(string levelName, string property)
		{
			return GetLevelString(levelName, property, string.Empty);
		}

		public static string GetLevelString(string levelName, string property, string def)
		{
			if (levels.ContainsKey(levelName))
			{
				Level level = levels[levelName];
				if (level.HasProperty(property))
				{
					return level.GetPropertyString(property);
				}
				return def;
			}
			return def;
		}

		public static void SetLevelString(string levelName, string property, string val)
		{
			Level level;
			if (!levels.ContainsKey(levelName))
			{
				level = new Level(levelName);
				levels.Add(levelName, level);
			}
			else
			{
				level = levels[levelName];
			}
			if (level.SetPropertyString(property, val))
			{
				WriteProfile();
			}
		}

		public static void SetProfileString(string property, string val)
		{
			if (profileLevel.SetPropertyString(property, val))
			{
				WriteProfile();
			}
		}

		public static string GetProfileString(string property)
		{
			return GetProfileString(property, string.Empty);
		}

		public static string GetProfileString(string property, string def)
		{
			if (IsProfilePropertySet(property))
			{
				return profileLevel.GetPropertyString(property);
			}
			return def;
		}

		[Obsolete("Use MadLevelProfile.GetLevelBoolean instead.")]
		public static bool IsPropertyEnabled(string levelName, string property)
		{
			return GetLevelBoolean(levelName, property);
		}

		[Obsolete("Use MadLevelProfile.SetLevelBoolean instead.")]
		public static void SetPropertyEnabled(string levelName, string property, bool enabled)
		{
			SetLevelBoolean(levelName, property, enabled);
		}

		public static bool IsProfilePropertySet(string property)
		{
			return profileLevel.HasProperty(property);
		}

		public static bool IsLevelPropertySet(string levelName, string property)
		{
			if (levels.ContainsKey(levelName))
			{
				Level level = levels[levelName];
				return level.HasProperty(property);
			}
			return false;
		}

		[Obsolete("Use MadLevelProfile.IsLevelPropertySet instead.")]
		public static bool IsPropertySet(string levelName, string property)
		{
			return IsLevelPropertySet(levelName, property);
		}

		public static bool IsCompleted(string levelName)
		{
			return GetLevelBoolean(levelName, "@completed@");
		}

		public static bool IsCompleted(string levelName, bool def)
		{
			return GetLevelBoolean(levelName, "@completed@", def);
		}

		public static void SetCompleted(string levelName, bool completed)
		{
			SetLevelBoolean(levelName, "@completed@", completed);
		}

		public static bool IsCompletedSet(string levelName)
		{
			return IsLevelPropertySet(levelName, "@completed@");
		}

		public static bool IsLocked(string levelName)
		{
			return GetLevelBoolean(levelName, "@locked@");
		}

		public static bool IsLocked(string levelName, bool def)
		{
			return GetLevelBoolean(levelName, "@locked@", def);
		}

		public static void SetLocked(string levelName, bool locked)
		{
			SetLevelBoolean(levelName, "@locked@", locked);
		}

		public static bool IsLockedSet(string levelName)
		{
			return IsLevelPropertySet(levelName, "@locked@");
		}

		public static bool IsLevelSet(string levelName)
		{
			return levels.ContainsKey(levelName);
		}

		public static void RenameLevel(string oldName, string newName)
		{
			if (IsLevelSet(oldName) && !IsLevelSet(newName))
			{
				Level value = levels[oldName];
				levels[newName] = value;
			}
			else
			{
				UnityEngine.Debug.LogError("Cannot rename level");
			}
		}

		public static void Save()
		{
			backend.Flush();
		}

		public static void Reset()
		{
			ResetLevelScope();
			ResetProfileScope();
			ApplyConfigurationProfile();
			WriteProfile();
			Save();
		}

		private static void ResetLevelScope()
		{
			levels.Clear();
		}

		private static void ResetProfileScope()
		{
			_profileLevel = new Level("__profile__");
		}
	}
}
