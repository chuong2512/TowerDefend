using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MadLevelManager
{
	public class MadHashCode
	{
		public const int FirstPrime = 37;

		public const int SecondPrime = 13;

		private int currentHash;

		public MadHashCode()
		{
			currentHash = 37;
		}

		public void Add(object obj)
		{
			currentHash = Add(currentHash, obj);
		}

		public void AddArray(object[] arr)
		{
			currentHash = AddArray(currentHash, arr);
		}

		public void AddList<T>(List<T> list)
		{
			currentHash = AddList(currentHash, list);
		}

		public void AddEnumerable(IEnumerable enumerable)
		{
			if (enumerable == null)
			{
				Add(null);
				return;
			}
			IEnumerator enumerator = enumerable.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object current = enumerator.Current;
					Add(current);
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
		}

		public override int GetHashCode()
		{
			return currentHash;
		}

		public static int Add(int currentHash, bool a)
		{
			return (currentHash * 13) ^ a.GetHashCode();
		}

		public static int Add(int currentHash, int a)
		{
			return (currentHash * 13) ^ a.GetHashCode();
		}

		public static int Add(int currentHash, float a)
		{
			return (currentHash * 13) ^ a.GetHashCode();
		}

		public static int Add(int currentHash, double a)
		{
			return (currentHash * 13) ^ a.GetHashCode();
		}

		public static int Add(int currentHash, Vector2 v)
		{
			currentHash = Add(currentHash, Mathf.Abs(v.x));
			currentHash = Add(currentHash, Mathf.Abs(v.y));
			currentHash = Add(currentHash, (!(v.x > 0f)) ? 1 : 0);
			currentHash = Add(currentHash, (!(v.y > 0f)) ? 1 : 0);
			return currentHash;
		}

		public static int Add(int currentHash, Vector3 v)
		{
			currentHash = Add(currentHash, Mathf.Abs(v.x));
			currentHash = Add(currentHash, Mathf.Abs(v.y));
			currentHash = Add(currentHash, Mathf.Abs(v.z));
			currentHash = Add(currentHash, (!(v.x > 0f)) ? 1 : 0);
			currentHash = Add(currentHash, (!(v.y > 0f)) ? 1 : 0);
			currentHash = Add(currentHash, (!(v.z > 0f)) ? 1 : 0);
			return currentHash;
		}

		public static int Add(int currentHash, Vector4 v)
		{
			currentHash = Add(currentHash, Mathf.Abs(v.w));
			currentHash = Add(currentHash, Mathf.Abs(v.x));
			currentHash = Add(currentHash, Mathf.Abs(v.y));
			currentHash = Add(currentHash, Mathf.Abs(v.z));
			currentHash = Add(currentHash, (!(v.w > 0f)) ? 1 : 0);
			currentHash = Add(currentHash, (!(v.x > 0f)) ? 1 : 0);
			currentHash = Add(currentHash, (!(v.y > 0f)) ? 1 : 0);
			currentHash = Add(currentHash, (!(v.z > 0f)) ? 1 : 0);
			return currentHash;
		}

		public static int Add(int currentHash, object obj)
		{
			return (currentHash * 13) ^ (obj?.GetHashCode() ?? 0);
		}

		public static int AddArray(int currentHash, object[] arr)
		{
			if (arr == null)
			{
				return Add(currentHash, null);
			}
			int num = arr.Length;
			for (int i = 0; i < num; i++)
			{
				currentHash = Add(currentHash, arr[i]);
			}
			return currentHash;
		}

		public static int AddList<T>(int currentHash, List<T> list)
		{
			if (list == null)
			{
				return Add(currentHash, null);
			}
			int count = list.Count;
			for (int i = 0; i < count; i++)
			{
				currentHash = Add(currentHash, list[i]);
			}
			return currentHash;
		}
	}
}
