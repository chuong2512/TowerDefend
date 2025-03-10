using System.Collections.Generic;
using UnityEngine;

namespace TDTK
{
	public class CreepDB : MonoBehaviour
	{
		public List<UnitCreep> creepList = new List<UnitCreep>();

		public static CreepDB LoadDB()
		{
			GameObject gameObject = Resources.Load("DB_TDTK/CreepDB", typeof(GameObject)) as GameObject;
			return gameObject.GetComponent<CreepDB>();
		}

		public static List<UnitCreep> Load()
		{
			GameObject gameObject = Resources.Load("DB_TDTK/CreepDB", typeof(GameObject)) as GameObject;
			CreepDB component = gameObject.GetComponent<CreepDB>();
			return component.creepList;
		}

		public static UnitCreep GetFirstPrefab()
		{
			GameObject gameObject = Resources.Load("DB_TDTK/CreepDB", typeof(GameObject)) as GameObject;
			CreepDB component = gameObject.GetComponent<CreepDB>();
			return (component.creepList.Count != 0) ? component.creepList[0] : null;
		}
	}
}
