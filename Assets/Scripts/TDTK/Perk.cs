using System;
using System.Collections.Generic;
using UnityEngine;

namespace TDTK
{
	[Serializable]
	public class Perk : TDTKItem
	{
		public Sprite iconUnavailable;

		public Sprite iconPurchased;

		public bool repeatable;

		public bool purchased;

		public int purchasedCount;

		public _PerkType type;

		public List<int> cost = new List<int>();

		public int minLevel = 1;

		public int minWave;

		public int minPerkPoint;

		public List<int> prereq = new List<int>();

		public List<int> itemIDList = new List<int>();

		public int itemID;

		public float value;

		public float valueAlt;

		public List<float> valueRscList = new List<float>();

		public UnitStat stats = new UnitStat();

		public AbilityEffect effects = new AbilityEffect();

		public float HP;

		public float HPRegen;

		public float HPStagger;

		public float shield;

		public float shieldRegen;

		public float shieldStagger;

		public float buildCost;

		public float upgradeCost;

		public float abCost;

		public float abCooldown;

		public float abAOERadius;

		public string desp = string.Empty;

		public Perk()
		{
			stats.damageMin = 0f;
			stats.damageMax = 0f;
			stats.cooldown = 0f;
			stats.clipSize = 0f;
			stats.reloadDuration = 0f;
			stats.range = 0f;
			stats.aoeRadius = 0f;
		}

		public Perk Clone()
		{
			Perk perk = new Perk();
			perk.ID = ID;
			perk.name = name;
			perk.icon = icon;
			perk.iconUnavailable = iconUnavailable;
			perk.repeatable = repeatable;
			perk.purchased = purchased;
			perk.purchasedCount = purchasedCount;
			perk.type = type;
			perk.cost = new List<int>(cost);
			perk.minLevel = minLevel;
			perk.minWave = minWave;
			perk.minPerkPoint = minPerkPoint;
			perk.prereq = new List<int>(prereq);
			perk.itemIDList = new List<int>(itemIDList);
			perk.itemID = itemID;
			perk.value = value;
			perk.valueAlt = valueAlt;
			perk.valueRscList = new List<float>(valueRscList);
			perk.stats = stats.Clone();
			perk.effects = effects.Clone();
			perk.HP = HP;
			perk.HPRegen = HPRegen;
			perk.HPStagger = HPStagger;
			perk.shield = shield;
			perk.shieldRegen = shieldRegen;
			perk.shieldStagger = shieldStagger;
			perk.buildCost = buildCost;
			perk.upgradeCost = upgradeCost;
			perk.abCost = abCost;
			perk.abCooldown = abCooldown;
			perk.abAOERadius = abAOERadius;
			perk.desp = desp;
			return perk;
		}

		public string IsAvailable()
		{
			if (GameControl.GetLevelID() < minLevel)
			{
				return "Unlocked at level " + minLevel;
			}
			if (Mathf.Max(SpawnManager.GetCurrentWaveID() + 1, 1) < minWave)
			{
				return "Unlocked at Wave " + minWave;
			}
			if (PerkManager.GetPerkPoint() < minPerkPoint)
			{
				return "Insufficient perk point";
			}
			if (prereq.Count > 0)
			{
				string text = "Require: ";
				bool flag = true;
				List<Perk> perkList = PerkManager.GetPerkList();
				for (int i = 0; i < prereq.Count; i++)
				{
					for (int j = 0; j < perkList.Count; j++)
					{
						if (perkList[j].ID == prereq[i])
						{
							text = text + (flag ? string.Empty : ", ") + perkList[j].name;
							flag = false;
							break;
						}
					}
				}
				return text;
			}
			return string.Empty;
		}

		public string Purchase(bool useRsc = true)
		{
			if (purchased)
			{
				return "Purchased";
			}
			if (useRsc)
			{
				int num = ResourceManager.HasSufficientResource(cost);
				if (num != -1)
				{
					return "Insufficient " + ResourceManager.GetResourceList()[num].name;
				}
				ResourceManager.SpendResource(cost);
			}
			if (!repeatable)
			{
				purchased = true;
			}
			purchasedCount++;
			return string.Empty;
		}

		public List<int> GetCost()
		{
			return cost;
		}
	}
}
