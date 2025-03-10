using System.Collections.Generic;
using UnityEngine;

namespace TDTK
{
	public class TowerDB : MonoBehaviour
	{
		public List<UnitTower> towerList = new List<UnitTower>();

		public static TowerDB LoadDB()
		{
			GameObject gameObject = Resources.Load("DB_TDTK/TowerDB", typeof(GameObject)) as GameObject;
			return gameObject.GetComponent<TowerDB>();
		}

		public static List<UnitTower> Load()
		{
			GameObject gameObject = Resources.Load("DB_TDTK/TowerDB", typeof(GameObject)) as GameObject;
			TowerDB component = gameObject.GetComponent<TowerDB>();
			return component.towerList;
		}
	}
}
