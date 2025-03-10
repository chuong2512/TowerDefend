using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MadLevelManager
{
	public class MadAtlas : MonoBehaviour
	{
		[Serializable]
		public class Item
		{
			public string name;

			public Rect region;

			public int pixelsWidth;

			public int pixelsHeight;

			public string textureGUID;
		}

		public Texture2D atlasTexture;

		public List<Item> items = new List<Item>();

		private Dictionary<string, Item> map = new Dictionary<string, Item>();

		public bool AddItem(Item item)
		{
			if (!map.ContainsKey(item.textureGUID))
			{
				items.Add(item);
				map.Add(item.textureGUID, item);
				return true;
			}
			return false;
		}

		public void AddItemRange(IEnumerable<Item> items)
		{
			foreach (Item item in items)
			{
				AddItem(item);
			}
		}

		public Item GetItem(string guid)
		{
			Refresh();
			if (map.ContainsKey(guid))
			{
				return map[guid];
			}
			return null;
		}

		public void ClearItems()
		{
			items.Clear();
			map.Clear();
		}

		public List<Item> ListItems()
		{
			return items;
		}

		public List<string> ListItemNames()
		{
			IEnumerable<string> source = from item in items
				select item.name;
			return source.ToList();
		}

		public List<string> ListItemGUIDs()
		{
			IEnumerable<string> source = from item in items
				select item.textureGUID;
			return source.ToList();
		}

		private void Refresh()
		{
			if (map.Count != items.Count)
			{
				map.Clear();
				foreach (Item item in items)
				{
					map[item.textureGUID] = item;
				}
			}
		}
	}
}
