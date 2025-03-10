using System.Collections.Generic;
using UnityEngine;

namespace TDTK
{
	public class DamageTable : MonoBehaviour
	{
		private static List<ArmorType> armorTypeList = new List<ArmorType>();

		private static List<DamageType> damageTypeList = new List<DamageType>();

		public static List<DamageType> GetAllDamageType()
		{
			return damageTypeList;
		}

		public static List<ArmorType> GetAllArmorType()
		{
			return armorTypeList;
		}

		private void Awake()
		{
			LoadPrefab();
		}

		private static void LoadPrefab()
		{
			DamageTableDB damageTableDB = DamageTableDB.LoadDB();
			armorTypeList = damageTableDB.armorTypeList;
			damageTypeList = damageTableDB.damageTypeList;
		}

		public static float GetModifier(int armorID = 0, int dmgID = 0)
		{
			armorID = Mathf.Max(0, armorID);
			dmgID = Mathf.Max(0, dmgID);
			if (armorID < armorTypeList.Count && dmgID < damageTypeList.Count)
			{
				return armorTypeList[armorID].modifiers[dmgID];
			}
			return 1f;
		}

		public static ArmorType GetArmorTypeInfo(int ID)
		{
			if (ID < 0 || ID >= armorTypeList.Count)
			{
				UnityEngine.Debug.Log("ArmorType requested does not exist");
				return null;
			}
			return armorTypeList[ID];
		}

		public static DamageType GetDamageTypeInfo(int ID)
		{
			if (ID < 0 || ID >= damageTypeList.Count)
			{
				UnityEngine.Debug.Log("DamageType requested does not exist");
				return null;
			}
			return damageTypeList[ID];
		}
	}
}
