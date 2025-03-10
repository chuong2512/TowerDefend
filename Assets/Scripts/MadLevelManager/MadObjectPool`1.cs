using System.Collections.Generic;
using UnityEngine;

namespace MadLevelManager
{
	public class MadObjectPool<T>
	{
		private class Item
		{
			public bool free = true;

			public T obj;

			public Item(T obj)
			{
				this.obj = obj;
			}
		}

		private List<Item> list;

		public MadObjectPool(int capacity)
		{
			list = new List<Item>(capacity);
		}

		public void Add(T obj)
		{
			list.Add(new Item(obj));
		}

		public bool CanTake()
		{
			return FindFree() != -1;
		}

		public T Take()
		{
			int num = FindFree();
			if (num == -1)
			{
				return default(T);
			}
			Item item = list[num];
			item.free = false;
			return item.obj;
		}

		public void Release(T obj)
		{
			int index = Find(obj);
			Item item = list[index];
			if (item.free)
			{
				UnityEngine.Debug.LogError("Item is already free");
			}
			else
			{
				item.free = true;
			}
		}

		private int FindFree()
		{
			int count = list.Count;
			for (int i = 0; i < count; i++)
			{
				if (list[i].free)
				{
					return i;
				}
			}
			return -1;
		}

		private int Find(T obj)
		{
			int count = list.Count;
			for (int i = 0; i < count; i++)
			{
				if ((object)list[i].obj == (object)obj)
				{
					return i;
				}
			}
			return -1;
		}
	}
}
