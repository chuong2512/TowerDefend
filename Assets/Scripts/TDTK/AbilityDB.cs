using System.Collections.Generic;
using UnityEngine;

namespace TDTK
{
	public class AbilityDB : MonoBehaviour
	{
		public List<Ability> abilityList = new List<Ability>();

		public static AbilityDB LoadDB()
		{
			GameObject gameObject = Resources.Load("DB_TDTK/AbilityDB", typeof(GameObject)) as GameObject;
			return gameObject.GetComponent<AbilityDB>();
		}

		public static List<Ability> Load()
		{
			GameObject gameObject = Resources.Load("DB_TDTK/AbilityDB", typeof(GameObject)) as GameObject;
			AbilityDB component = gameObject.GetComponent<AbilityDB>();
			return component.abilityList;
		}

		public static List<Ability> LoadClone()
		{
			GameObject gameObject = Resources.Load("DB_TDTK/AbilityDB", typeof(GameObject)) as GameObject;
			AbilityDB component = gameObject.GetComponent<AbilityDB>();
			List<Ability> list = new List<Ability>();
			if (component != null)
			{
				for (int i = 0; i < component.abilityList.Count; i++)
				{
					list.Add(component.abilityList[i].Clone());
				}
			}
			return list;
		}
	}
}
