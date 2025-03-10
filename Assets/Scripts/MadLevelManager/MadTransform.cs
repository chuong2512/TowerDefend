using System.Collections.Generic;
using UnityEngine;

namespace MadLevelManager
{
	public class MadTransform
	{
		public delegate bool Predicate<T>(T t);

		public static bool registerUndo = true;

		public static bool instantiating
		{
			get;
			set;
		}

		public static T CreateChild<T>(Transform parent, string name, bool disabled = false) where T : Component
		{
			GameObject gameObject = null;
			gameObject = new GameObject(name);
			if (disabled)
			{
				MadGameObject.SetActive(gameObject, active: false);
			}
			gameObject.transform.parent = parent;
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject.transform.localScale = Vector3.one;
			gameObject.transform.localPosition = Vector3.zero;
			T val = gameObject.GetComponent<T>();
			if ((Object)val == (Object)null)
			{
				val = gameObject.AddComponent<T>();
			}
			if (registerUndo)
			{
				MadUndo.RegisterCreatedObjectUndo(gameObject, "Created " + name);
			}
			return val;
		}

		public static T GetOrCreateChild<T>(Transform parent, string name) where T : Component
		{
			T val;
			if (parent != null)
			{
				val = FindChild(parent, (T t) => t.name == name, 0);
			}
			else
			{
				GameObject gameObject = GameObject.Find("/" + name);
				val = ((!(gameObject != null)) ? ((T)null) : gameObject.GetComponent<T>());
			}
			if ((Object)val != (Object)null)
			{
				return val;
			}
			return CreateChild<T>(parent, name);
		}

		public static T CreateChild<T>(Transform parent, string name, T template) where T : Component
		{
			GameObject gameObject = CreateChild(parent, name, template.gameObject);
			return gameObject.GetComponent<T>();
		}

		public static T GetOrCreateChild<T>(Transform parent, string name, T template) where T : Component
		{
			T val = FindChild(parent, (T t) => t.name == name, 0);
			if ((Object)val != (Object)null)
			{
				return val;
			}
			return CreateChild(parent, name, template);
		}

		public static GameObject CreateChild(Transform parent, string name)
		{
			GameObject gameObject = new GameObject();
			gameObject.transform.parent = parent;
			gameObject.transform.localScale = Vector3.one;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.name = name;
			if (registerUndo)
			{
				MadUndo.RegisterCreatedObjectUndo(gameObject, "Created " + name);
			}
			return gameObject;
		}

		public static GameObject CreateChild(Transform parent, string name, GameObject template)
		{
			GameObject gameObject = null;
			instantiating = true;
			try
			{
				gameObject = Object.Instantiate(template);
				gameObject.transform.parent = parent;
				gameObject.name = name;
				if (!registerUndo)
				{
					return gameObject;
				}
				MadUndo.RegisterCreatedObjectUndo(gameObject, "Created " + name);
				return gameObject;
			}
			finally
			{
				instantiating = false;
			}
		}

		public static T FindChild<T>(Transform parent) where T : Component
		{
			return FindChild(parent, (T t) => true);
		}

		public static T FindChild<T>(Transform parent, int depth) where T : Component
		{
			return FindChild(parent, (T t) => true, depth);
		}

		public static T FindChild<T>(Transform parent, Predicate<T> predicate) where T : Component
		{
			return FindChild(parent, predicate, int.MaxValue, 0);
		}

		public static T FindChild<T>(Transform parent, Predicate<T> predicate, int depth) where T : Component
		{
			return FindChild(parent, predicate, depth, 0);
		}

		private static T FindChild<T>(Transform parent, Predicate<T> predicate, int depth, int currDepth) where T : Component
		{
			int childCount = parent.childCount;
			for (int i = 0; i < childCount; i++)
			{
				Transform child = parent.GetChild(i);
				T component = child.GetComponent<T>();
				if ((Object)component != (Object)null && predicate(component))
				{
					return component;
				}
				if (currDepth < depth)
				{
					T val = FindChild(child, predicate, depth, currDepth + 1);
					if ((Object)val != (Object)null)
					{
						return val;
					}
				}
			}
			return (T)null;
		}

		public static T FindChildWithName<T>(Transform parent, string name) where T : Component
		{
			return FindChild(parent, (T t) => t.name == name);
		}

		public static List<T> FindChildren<T>(Transform parent) where T : Component
		{
			return FindChildren(parent, (T t) => true);
		}

		public static List<T> FindChildren<T>(Transform parent, Predicate<T> predicate) where T : Component
		{
			return FindChildren(parent, predicate, int.MaxValue);
		}

		public static List<T> FindChildren<T>(Transform parent, Predicate<T> predicate, int depth) where T : Component
		{
			return FindChildren(parent, predicate, depth, 0);
		}

		private static List<T> FindChildren<T>(Transform parent, Predicate<T> predicate, int depth, int currDepth) where T : Component
		{
			List<T> list = new List<T>();
			int childCount = parent.childCount;
			for (int i = 0; i < childCount; i++)
			{
				Transform child = parent.GetChild(i);
				T component = child.GetComponent<T>();
				if ((Object)component != (Object)null && predicate(component))
				{
					list.Add(component);
				}
				if (currDepth < depth)
				{
					list.AddRange(FindChildren(child, predicate, depth, currDepth + 1));
				}
			}
			return list;
		}

		public static void FindChildren<T>(Transform parent, ref MadList<T> output) where T : Component
		{
			FindChildren(parent, (T t) => true, ref output);
		}

		public static void FindChildren<T>(Transform parent, Predicate<T> predicate, ref MadList<T> output) where T : Component
		{
			FindChildren(parent, predicate, int.MaxValue, ref output);
		}

		public static void FindChildren<T>(Transform parent, Predicate<T> predicate, int depth, ref MadList<T> output) where T : Component
		{
			FindChildren(parent, predicate, depth, 0, ref output);
		}

		private static void FindChildren<T>(Transform parent, Predicate<T> predicate, int depth, int currDepth, ref MadList<T> output) where T : Component
		{
			int childCount = parent.childCount;
			for (int i = 0; i < childCount; i++)
			{
				Transform child = parent.GetChild(i);
				T component = child.GetComponent<T>();
				if ((Object)component != (Object)null && predicate(component))
				{
					output.Add(component);
				}
				if (currDepth < depth)
				{
					FindChildren(child, predicate, depth, currDepth + 1, ref output);
				}
			}
		}

		public static T FindParent<T>(Transform t) where T : Component
		{
			return FindParent<T>(t, int.MaxValue);
		}

		public static T FindParent<T>(Transform t, int depth) where T : Component
		{
			return FindParent(t, depth, (T c) => true);
		}

		public static T FindParent<T>(Transform t, Predicate<T> predicate) where T : Component
		{
			return FindParent(t, int.MaxValue, predicate);
		}

		public static T FindParent<T>(Transform t, int depth, Predicate<T> predicate) where T : Component
		{
			Transform parent = t.parent;
			int num = 0;
			while (parent != null && num <= depth)
			{
				T component = parent.GetComponent<T>();
				if ((Object)component != (Object)null && predicate(component))
				{
					return component;
				}
				parent = parent.parent;
				num++;
			}
			return (T)null;
		}

		public static void SetLocalScale(Transform transform, float scale)
		{
			SetLocalScale(transform, scale, scale, scale);
		}

		public static void SetLocalScale(Transform transform, float x, float y, float z)
		{
			SetLocalScale(transform, new Vector3(x, y, z));
		}

		public static void SetLocalScale(Transform transform, Vector3 localScale)
		{
			if (Application.isPlaying || !MadMath.Approximately(transform.localScale, localScale))
			{
				transform.localScale = localScale;
			}
		}

		public static void SetPosition(Transform transform, Vector3 position)
		{
			if (Application.isPlaying || !MadMath.Approximately(transform.position, position))
			{
				transform.position = position;
			}
		}

		public static void SetLocalPosition(Transform transform, Vector3 localPosition)
		{
			if (Application.isPlaying || !MadMath.Approximately(transform.localPosition, localPosition))
			{
				transform.localPosition = localPosition;
			}
		}

		public static void SetLocalEulerAngles(Transform transform, Vector3 localEulerAngles)
		{
			if (Application.isPlaying || !MadMath.Approximately(transform.localEulerAngles, localEulerAngles))
			{
				transform.localEulerAngles = localEulerAngles;
			}
		}

		public static void SetRotation(Transform transform, Quaternion rotation)
		{
			if (Application.isPlaying || !MadMath.Approximately(transform.rotation, rotation))
			{
				transform.rotation = rotation;
			}
		}

		public static void SetLocalRotation(Transform transform, Quaternion localRotation)
		{
			if (Application.isPlaying || !MadMath.Approximately(transform.localRotation, localRotation))
			{
				transform.localRotation = localRotation;
			}
		}
	}
}
