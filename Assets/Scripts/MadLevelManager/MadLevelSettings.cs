using MadLevelManager.Backend;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace MadLevelManager
{
	public class MadLevelSettings : ScriptableObject
	{
		[Serializable]
		public class Property
		{
			public string name;

			public string value;
		}

		private static MadLevelSettings _current;

		public string profileBackend = typeof(MadLevelProfile.DefaultBackend).ToString();

		public List<Property> profileBackendProperties = new List<Property>();

		public static MadLevelSettings current
		{
			get
			{
				if (_current == null)
				{
					_current = (MadLevelSettings)Resources.Load("MLM_Settings", typeof(MadLevelSettings));
				}
				return _current;
			}
		}

		public IMadLevelProfileBackend CreateBackend()
		{
			if (string.IsNullOrEmpty(profileBackend))
			{
				return new MadLevelProfile.DefaultBackend();
			}
			Type type = Type.GetType(profileBackend);
			if (type == null)
			{
				UnityEngine.Debug.LogWarning("Cannot find backend " + profileBackend + ", using default.");
				return new MadLevelProfile.DefaultBackend();
			}
			try
			{
				if (!typeof(IMadLevelProfileBackend).IsAssignableFrom(type))
				{
					throw new Exception("Not a instance of IMadLevelProfileBackend");
				}
				object obj = Activator.CreateInstance(type);
				ConfigureProperties(obj);
				IMadLevelProfileBackend madLevelProfileBackend = (IMadLevelProfileBackend)obj;
				madLevelProfileBackend.Start();
				return madLevelProfileBackend;
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.LogError("Cannot create instance of " + profileBackend + ": " + ex);
				return new MadLevelProfile.DefaultBackend();
			}
		}

		private void ConfigureProperties(object instance)
		{
			List<FieldInfo> requiredFields = GetRequiredFields(instance.GetType());
			if (!AssignFields(instance, requiredFields))
			{
				UnityEngine.Debug.LogError("Do not have all required properties! Please adjust correct MLM_Settings.", this);
			}
			List<FieldInfo> optionalFields = GetOptionalFields(instance.GetType());
			AssignFields(instance, optionalFields);
		}

		private bool AssignFields(object instance, List<FieldInfo> fields)
		{
			bool result = true;
			for (int i = 0; i < fields.Count; i++)
			{
				FieldInfo fieldInfo = fields[i];
				string name = fieldInfo.Name;
				if (FindPropertyValue(name, out string outValue))
				{
					SetValue(fieldInfo, instance, outValue);
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		private void SetValue(FieldInfo field, object instance, string value)
		{
			Type fieldType = field.FieldType;
			if (fieldType == typeof(string))
			{
				field.SetValue(instance, value);
			}
			else if (fieldType == typeof(int))
			{
				int.TryParse(value, out int result);
				field.SetValue(instance, result);
			}
			else if (fieldType == typeof(float))
			{
				float.TryParse(value, out float result2);
				field.SetValue(instance, result2);
			}
			else if (fieldType == typeof(bool))
			{
				bool.TryParse(value, out bool result3);
				field.SetValue(instance, result3);
			}
			else if (typeof(Enum).IsAssignableFrom(fieldType))
			{
				Enum value2 = string.IsNullOrEmpty(value) ? ((Enum)Enum.Parse(fieldType, "0")) : ((Enum)Enum.Parse(fieldType, value));
				field.SetValue(fieldType, value2);
			}
			else
			{
				UnityEngine.Debug.LogError("Unsupported type: " + fieldType);
			}
		}

		private List<FieldInfo> GetRequiredFields(Type type)
		{
			return GetFieldsWithAttribute(type, typeof(Required));
		}

		private List<FieldInfo> GetOptionalFields(Type type)
		{
			return GetFieldsWithAttribute(type, typeof(Optional));
		}

		private List<FieldInfo> GetFieldsWithAttribute(Type type, Type attribute)
		{
			List<FieldInfo> list = new List<FieldInfo>();
			FieldInfo[] fields = type.GetFields();
			foreach (FieldInfo fieldInfo in fields)
			{
				if (fieldInfo.GetCustomAttributes(attribute, inherit: false).Length > 0)
				{
					list.Add(fieldInfo);
				}
			}
			return list;
		}

		private object CreateInstance(ConstructorInfo constructor)
		{
			if (CreateParameters(constructor, out object[] outResult))
			{
				return constructor.Invoke(null, outResult);
			}
			throw new Exception("Cannot create instance. Check assigned parameters.");
		}

		private bool CreateParameters(ConstructorInfo constructor, out object[] outResult)
		{
			ParameterInfo[] parameters = constructor.GetParameters();
			object[] array = new object[parameters.Length];
			for (int i = 0; i < parameters.Length; i++)
			{
				ParameterInfo parameterInfo = parameters[i];
				string name = parameterInfo.Name;
				if (FindPropertyValue(name, out string outValue))
				{
					array[i] = outValue;
					continue;
				}
				outResult = null;
				return false;
			}
			outResult = array;
			return true;
		}

		public bool FindPropertyValue(string name, out string outValue)
		{
			Property property = FindProperty(name);
			if (property != null)
			{
				outValue = property.value;
				return true;
			}
			outValue = null;
			return false;
		}

		private Property FindProperty(string name)
		{
			for (int i = 0; i < profileBackendProperties.Count; i++)
			{
				Property property = profileBackendProperties[i];
				if (property.name == name)
				{
					return property;
				}
			}
			return null;
		}

		public void SetPropertyValue(string name, string value)
		{
			Property property = FindProperty(name);
			if (property == null)
			{
				property = new Property();
				property.name = name;
				profileBackendProperties.Add(property);
			}
			property.value = value;
		}

		public static ConstructorInfo FindConstructor(Type backendType)
		{
			ConstructorInfo[] constructors = backendType.GetConstructors();
			ConstructorInfo[] array = constructors;
			int num = 0;
			goto IL_001a;
			IL_001a:
			if (num < array.Length)
			{
				return array[num];
			}
			return null;
			IL_0016:
			num++;
			goto IL_001a;
		}

		private static void VerifyConstructor(ConstructorInfo constructorInfo)
		{
			ParameterInfo[] parameters = constructorInfo.GetParameters();
			ParameterInfo[] array = parameters;
			int num = 0;
			while (true)
			{
				if (num < array.Length)
				{
					ParameterInfo parameterInfo = array[num];
					if (parameterInfo.ParameterType != typeof(string))
					{
						break;
					}
					num++;
					continue;
				}
				return;
			}
			throw new Exception("Invalid constructor, should only have strings as parameters");
		}
	}
}
