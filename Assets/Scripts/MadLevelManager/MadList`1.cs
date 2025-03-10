using System;
using UnityEngine;

namespace MadLevelManager
{
	public class MadList<T>
	{
		private T[] arr;

		private int size;

		public T[] Array
		{
			get
			{
				return arr;
			}
			set
			{
				arr = value;
			}
		}

		public int Count => size;

		public T this[int index]
		{
			get
			{
				CheckRange(index);
				return arr[index];
			}
			set
			{
				CheckRange(index);
				arr[index] = value;
			}
		}

		public MadList()
			: this(32)
		{
		}

		public MadList(int capacity)
		{
			arr = new T[capacity];
		}

		public void Add(T e)
		{
			EnsureCapacity(size + 1);
			arr[size] = e;
			size++;
		}

		private void CheckRange(int index)
		{
			if (index >= size)
			{
				throw new IndexOutOfRangeException("index " + index + " out of range (size = " + size + ")");
			}
		}

		public void Clear()
		{
			size = 0;
		}

		public void Trim()
		{
			if (size != arr.Length)
			{
				System.Array.Resize(ref arr, size);
			}
		}

		private void EnsureCapacity(int targetSize)
		{
			if (arr.Length < targetSize)
			{
				System.Array.Resize(ref arr, Mathf.Min(targetSize * 2, 1048576));
			}
		}

		public int FindIndex(T obj)
		{
			for (int i = 0; i < size; i++)
			{
				T val = arr[i];
				if (val == null && obj == null)
				{
					return i;
				}
				if (val != null && val.Equals(obj))
				{
					return i;
				}
			}
			return -1;
		}

		public bool Contains(T obj)
		{
			return FindIndex(obj) != -1;
		}

		public bool Remove(T obj)
		{
			int num = FindIndex(obj);
			if (num == -1)
			{
				return false;
			}
			RemoveAt(num);
			return true;
		}

		public void RemoveAt(int index)
		{
			CheckRange(index);
			if (size > index + 1)
			{
				ShiftLeft(index + 1);
			}
			size--;
		}

		private void ShiftLeft(int firstIndex)
		{
			for (int i = firstIndex; i < size; i++)
			{
				Copy(i, i - 1);
			}
		}

		private void Copy(int from, int to)
		{
			arr[to] = arr[from];
		}
	}
}
