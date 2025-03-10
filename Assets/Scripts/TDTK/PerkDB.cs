using System.Collections.Generic;
using UnityEngine;

namespace TDTK
{
	public class PerkDB : MonoBehaviour
	{
		public List<Perk> perkList = new List<Perk>();

		public static PerkDB LoadDB()
		{
			GameObject gameObject = Resources.Load("DB_TDTK/PerkDB", typeof(GameObject)) as GameObject;
			return gameObject.GetComponent<PerkDB>();
		}

		public static List<Perk> Load()
		{
			GameObject gameObject = Resources.Load("DB_TDTK/PerkDB", typeof(GameObject)) as GameObject;
			PerkDB component = gameObject.GetComponent<PerkDB>();
			return component.perkList;
		}

		public static List<Perk> LoadClone()
		{
			GameObject gameObject = Resources.Load("DB_TDTK/PerkDB", typeof(GameObject)) as GameObject;
			PerkDB component = gameObject.GetComponent<PerkDB>();
			List<Perk> list = new List<Perk>();
			if (component != null)
			{
				for (int i = 0; i < component.perkList.Count; i++)
				{
					list.Add(component.perkList[i].Clone());
				}
			}
			return list;
		}
	}
}
