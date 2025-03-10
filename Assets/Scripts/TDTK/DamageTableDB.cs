using System.Collections.Generic;
using UnityEngine;

namespace TDTK
{
	public class DamageTableDB : MonoBehaviour
	{
		public List<ArmorType> armorTypeList = new List<ArmorType>();

		public List<DamageType> damageTypeList = new List<DamageType>();

		public static DamageTableDB LoadDB()
		{
			GameObject gameObject = Resources.Load("DB_TDTK/DamageTableDB", typeof(GameObject)) as GameObject;
			return gameObject.GetComponent<DamageTableDB>();
		}
	}
}
