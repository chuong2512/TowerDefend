using System.Collections.Generic;
using UnityEngine;

namespace TDTK
{
	public class FPSWeaponDB : MonoBehaviour
	{
		public List<FPSWeapon> weaponList = new List<FPSWeapon>();

		public static FPSWeaponDB LoadDB()
		{
			GameObject gameObject = Resources.Load("DB_TDTK/FPSWeaponDB", typeof(GameObject)) as GameObject;
			return gameObject.GetComponent<FPSWeaponDB>();
		}

		public static List<FPSWeapon> Load()
		{
			GameObject gameObject = Resources.Load("DB_TDTK/FPSWeaponDB", typeof(GameObject)) as GameObject;
			FPSWeaponDB component = gameObject.GetComponent<FPSWeaponDB>();
			return component.weaponList;
		}
	}
}
