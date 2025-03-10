using System.Collections.Generic;
using UnityEngine;

namespace TDTK
{
	public class PerkManager : MonoBehaviour
	{
		private int perkPoint;

		public List<int> unavailableIDList = new List<int>();

		[HideInInspector]
		public List<int> availableIDList = new List<int>();

		public List<int> purchasedIDList = new List<int>();

		private List<Perk> perkList = new List<Perk>();

		public static PerkManager instance;

		private bool init;

		public bool enableSave;

		public bool enableLoad;

		private List<UnitTower> unlockedTowerList = new List<UnitTower>();

		private List<Ability> unlockedAbilityList = new List<Ability>();

		private List<FPSWeapon> unlockedWeaponList = new List<FPSWeapon>();

		public int lifeCap;

		public float lifeRegen;

		public int lifeWaveClearedBonus;

		public List<float> rscRegen = new List<float>();

		public List<float> rscGain = new List<float>();

		public List<float> rscCreepKilledGain = new List<float>();

		public List<float> rscWaveClearedGain = new List<float>();

		public List<float> rscRscTowerGain = new List<float>();

		public float energyRegen;

		public float energyCap;

		public float energyCreepKilledBonus;

		public float energyWaveClearedBonus;

		public PerkTowerModifier emptyTowerModifier;

		public PerkTowerModifier globalTowerModifier;

		public List<PerkTowerModifier> towerModifierList = new List<PerkTowerModifier>();

		public PerkFPSWeaponModifier emptyFPSWeaponModifier;

		public PerkFPSWeaponModifier globalFPSWeaponModifier;

		public List<PerkFPSWeaponModifier> FPSWeaponModifierList = new List<PerkFPSWeaponModifier>();

		public PerkAbilityModifier emptyAbilityModifier;

		public PerkAbilityModifier globalAbilityModifier;

		public List<PerkAbilityModifier> abilityModifierList = new List<PerkAbilityModifier>();

		public static int GetPerkPoint()
		{
			return instance.perkPoint;
		}

		public static List<Perk> GetPerkList()
		{
			return instance.perkList;
		}

		public static bool IsOn()
		{
			return (!(instance == null)) ? true : false;
		}

		public void Awake()
		{
			if (init)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		public void Init()
		{
			if (init)
			{
				return;
			}
			init = true;
			if (instance != null)
			{
				UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
			instance = this;
			availableIDList = new List<int>();
			List<Perk> perkDBList = TDTK.GetPerkDBList();
			for (int i = 0; i < perkDBList.Count; i++)
			{
				if (!unavailableIDList.Contains(perkDBList[i].ID))
				{
					perkList.Add(perkDBList[i].Clone());
					availableIDList.Add(perkDBList[i].ID);
				}
			}
			globalTowerModifier = new PerkTowerModifier();
			globalAbilityModifier = new PerkAbilityModifier();
			globalFPSWeaponModifier = new PerkFPSWeaponModifier();
			emptyTowerModifier = new PerkTowerModifier();
			emptyAbilityModifier = new PerkAbilityModifier();
			emptyFPSWeaponModifier = new PerkFPSWeaponModifier();
			int resourceCount = ResourceManager.GetResourceCount();
			for (int j = 0; j < resourceCount; j++)
			{
				rscRegen.Add(0f);
				rscGain.Add(0f);
				rscCreepKilledGain.Add(0f);
				rscWaveClearedGain.Add(0f);
				rscRscTowerGain.Add(0f);
			}
			for (int k = 0; k < perkList.Count; k++)
			{
				while (perkList[k].cost.Count < resourceCount)
				{
					perkList[k].cost.Add(0);
				}
				while (perkList[k].cost.Count > resourceCount)
				{
					perkList[k].cost.RemoveAt(perkList[k].cost.Count - 1);
				}
				while (perkList[k].valueRscList.Count < resourceCount)
				{
					perkList[k].valueRscList.Add(0f);
				}
				while (perkList[k].valueRscList.Count > resourceCount)
				{
					perkList[k].valueRscList.RemoveAt(perkList[k].valueRscList.Count - 1);
				}
			}
			UnityEngine.Debug.Log("mod repetable perk");
			LoadPurchasedPerk();
			for (int l = 0; l < perkList.Count; l++)
			{
				if (purchasedIDList.Contains(perkList[l].ID))
				{
					_PurchasePerk(perkList[l], useRsc: false);
				}
			}
		}

		public void SavePurchasedPerk()
		{
			if (enableSave)
			{
				PlayerPrefs.SetInt("TDTK_UnlockedPerkCount", purchasedIDList.Count);
				for (int i = 0; i < purchasedIDList.Count; i++)
				{
					PlayerPrefs.SetInt("TDTK_UnlockedPerk_ID_" + i, purchasedIDList[i]);
				}
			}
		}

		public void LoadPurchasedPerk()
		{
			if (!enableLoad)
			{
				return;
			}
			int @int = PlayerPrefs.GetInt("TDTK_UnlockedPerkCount", 0);
			for (int i = 0; i < @int; i++)
			{
				int int2 = PlayerPrefs.GetInt("TDTK_UnlockedPerk_ID_" + i, -1);
				if (int2 >= 0)
				{
					purchasedIDList.Add(int2);
				}
			}
		}

		public void ClearProgress()
		{
			int @int = PlayerPrefs.GetInt("TDTK_UnlockedPerkCount", 0);
			for (int i = 0; i < @int; i++)
			{
				PlayerPrefs.DeleteKey("TDTK_UnlockedPerk_ID_" + i);
			}
			PlayerPrefs.DeleteKey("TDTK_UnlockedPerkCount");
		}

		public static Perk GetPerk(int perkID)
		{
			return instance._GetPerk(perkID);
		}

		public Perk _GetPerk(int perkID)
		{
			for (int i = 0; i < perkList.Count; i++)
			{
				if (perkList[i].ID == perkID)
				{
					return perkList[i];
				}
			}
			return null;
		}

		public static string IsPerkAvailable(int perkID)
		{
			return instance._IsPerkAvailable(perkID);
		}

		public string _IsPerkAvailable(int perkID)
		{
			for (int i = 0; i < perkList.Count; i++)
			{
				if (perkList[i].ID == perkID)
				{
					return perkList[i].IsAvailable();
				}
			}
			return "PerkID doesnt correspond to any perk in the list   " + perkID;
		}

		public static bool IsPerkPurchased(int perkID)
		{
			return instance._IsPerkPurchased(perkID);
		}

		public bool _IsPerkPurchased(int perkID)
		{
			for (int i = 0; i < perkList.Count; i++)
			{
				if (perkList[i].ID == perkID)
				{
					return perkList[i].purchased;
				}
			}
			return false;
		}

		public static string PurchasePerk(int perkID, bool useRsc = true)
		{
			return instance._PurchasePerk(perkID, useRsc);
		}

		public string _PurchasePerk(int perkID, bool useRsc = true)
		{
			for (int i = 0; i < perkList.Count; i++)
			{
				if (perkList[i].ID == perkID)
				{
					return instance._PurchasePerk(perkList[i], useRsc);
				}
			}
			return "PerkID doesnt correspond to any perk in the list";
		}

		public static string PurchasePerk(Perk perk, bool useRsc = true)
		{
			return instance._PurchasePerk(perk, useRsc);
		}

		public string _PurchasePerk(Perk perk, bool useRsc = true)
		{
			string text = perk.Purchase(useRsc);
			if (text != string.Empty)
			{
				return text;
			}
			if (!purchasedIDList.Contains(perk.ID))
			{
				purchasedIDList.Add(perk.ID);
			}
			SavePurchasedPerk();
			TDTK.OnPerkPurchased(perk);
			for (int i = 0; i < perkList.Count; i++)
			{
				Perk perk2 = perkList[i];
				if (!perk2.purchased && perk2.prereq.Count != 0)
				{
					perk2.prereq.Remove(perk.ID);
				}
			}
			perkPoint++;
			TDTK.OnPerkPoint();
			if (perk.type == _PerkType.NewTower)
			{
				UnitTower dBTower = TDTK.GetDBTower(perk.itemIDList[0]);
				unlockedTowerList.Add(dBTower);
				BuildManager.AddNewTower(dBTower);
			}
			else if (perk.type == _PerkType.NewAbility)
			{
				Ability dBAbility = TDTK.GetDBAbility(perk.itemIDList[0]);
				unlockedAbilityList.Add(dBAbility);
				AbilityManager.AddNewAbility(dBAbility);
			}
			else if (perk.type == _PerkType.NewFPSWeapon)
			{
				FPSWeapon dBFpsWeapon = TDTK.GetDBFpsWeapon(perk.itemIDList[0]);
				unlockedWeaponList.Add(dBFpsWeapon);
				FPSControl.AddNewWeapon(dBFpsWeapon);
			}
			else if (perk.type == _PerkType.GainLife)
			{
				GameControl.GainLife((int)UnityEngine.Random.Range(perk.value, perk.valueAlt));
			}
			else if (perk.type == _PerkType.LifeCap)
			{
				lifeCap += (int)perk.value;
				GameControl.GainLife(0);
			}
			else if (perk.type == _PerkType.LifeRegen)
			{
				lifeRegen += perk.value;
			}
			else if (perk.type == _PerkType.LifeWaveClearedBonus)
			{
				lifeWaveClearedBonus += (int)perk.value;
			}
			else if (perk.type == _PerkType.GainRsc)
			{
				List<int> list = new List<int>();
				for (int j = 0; j < perk.valueRscList.Count; j++)
				{
					list.Add((int)perk.valueRscList[j]);
				}
				ResourceManager.GainResource(list, null, useMul: false);
			}
			else if (perk.type == _PerkType.RscRegen)
			{
				for (int k = 0; k < perk.valueRscList.Count; k++)
				{
					List<float> list2;
					int index;
					(list2 = rscRegen)[index = k] = list2[index] + perk.valueRscList[k];
				}
			}
			else if (perk.type == _PerkType.RscGain)
			{
				for (int l = 0; l < perk.valueRscList.Count; l++)
				{
					List<float> list2;
					int index2;
					(list2 = rscGain)[index2 = l] = list2[index2] + perk.valueRscList[l];
				}
			}
			else if (perk.type == _PerkType.RscCreepKilledGain)
			{
				for (int m = 0; m < perk.valueRscList.Count; m++)
				{
					List<float> list2;
					int index3;
					(list2 = rscCreepKilledGain)[index3 = m] = list2[index3] + perk.valueRscList[m];
				}
			}
			else if (perk.type == _PerkType.RscWaveClearedGain)
			{
				for (int n = 0; n < perk.valueRscList.Count; n++)
				{
					List<float> list2;
					int index4;
					(list2 = rscWaveClearedGain)[index4 = n] = list2[index4] + perk.valueRscList[n];
				}
			}
			else if (perk.type == _PerkType.RscResourceTowerGain)
			{
				for (int num = 0; num < perk.valueRscList.Count; num++)
				{
					List<float> list2;
					int index5;
					(list2 = rscRscTowerGain)[index5 = num] = list2[index5] + perk.valueRscList[num];
				}
			}
			else if (perk.type == _PerkType.Tower)
			{
				ModifyTowerModifier(globalTowerModifier, perk);
			}
			else if (perk.type == _PerkType.TowerSpecific)
			{
				for (int num2 = 0; num2 < perk.itemIDList.Count; num2++)
				{
					int num3 = TowerModifierExist(perk.itemIDList[num2]);
					if (num3 == -1)
					{
						PerkTowerModifier perkTowerModifier = new PerkTowerModifier();
						perkTowerModifier.prefabID = perk.itemIDList[num2];
						towerModifierList.Add(perkTowerModifier);
						num3 = towerModifierList.Count - 1;
					}
					ModifyTowerModifierInList(num3, perk);
				}
			}
			else if (perk.type == _PerkType.Ability)
			{
				ModifyAbilityModifier(globalAbilityModifier, perk);
			}
			else if (perk.type == _PerkType.AbilitySpecific)
			{
				for (int num4 = 0; num4 < perk.itemIDList.Count; num4++)
				{
					int num5 = AbilityModifierExist(perk.itemIDList[num4]);
					if (num5 == -1)
					{
						PerkAbilityModifier perkAbilityModifier = new PerkAbilityModifier();
						perkAbilityModifier.abilityID = perk.itemIDList[num4];
						abilityModifierList.Add(perkAbilityModifier);
						num5 = abilityModifierList.Count - 1;
					}
					ModifyAbilityModifierInList(num5, perk);
				}
			}
			else if (perk.type == _PerkType.FPSWeapon)
			{
				ModifyFPSWeaponModifier(globalFPSWeaponModifier, perk);
			}
			else if (perk.type == _PerkType.FPSWeaponSpecific)
			{
				for (int num6 = 0; num6 < perk.itemIDList.Count; num6++)
				{
					int num7 = FPSWeaponModifierExist(perk.itemIDList[num6]);
					if (num7 == -1)
					{
						PerkFPSWeaponModifier perkFPSWeaponModifier = new PerkFPSWeaponModifier();
						perkFPSWeaponModifier.prefabID = perk.itemIDList[num6];
						FPSWeaponModifierList.Add(perkFPSWeaponModifier);
						num7 = FPSWeaponModifierList.Count - 1;
					}
					ModifyFPSWeaponModifierInList(num7, perk);
				}
			}
			else if (perk.type == _PerkType.EnergyRegen)
			{
				energyRegen += perk.value;
			}
			else if (perk.type == _PerkType.EnergyIncreaseCap)
			{
				energyCap += perk.value;
			}
			else if (perk.type == _PerkType.EnergyCreepKilledBonus)
			{
				energyCreepKilledBonus += perk.value;
			}
			else if (perk.type == _PerkType.EnergyWaveClearedBonus)
			{
				energyWaveClearedBonus += perk.value;
			}
			return string.Empty;
		}

		private int TowerModifierExist(int prefabID)
		{
			for (int i = 0; i < towerModifierList.Count; i++)
			{
				if (towerModifierList[i].prefabID == prefabID)
				{
					return i;
				}
			}
			return -1;
		}

		private void ModifyTowerModifierInList(int ID, Perk perk)
		{
			ModifyTowerModifier(towerModifierList[ID], perk);
		}

		private void ModifyTowerModifier(PerkTowerModifier towerModifier, Perk perk)
		{
			towerModifier.HP += perk.HP;
			towerModifier.HPRegen += perk.HPRegen;
			towerModifier.HPStagger += perk.HPStagger;
			towerModifier.shield += perk.shield;
			towerModifier.shieldRegen += perk.shieldRegen;
			towerModifier.shieldStagger += perk.shieldStagger;
			towerModifier.buildCost += perk.buildCost;
			towerModifier.upgradeCost += perk.upgradeCost;
			ModifyUnitStats(towerModifier.stats, perk.stats);
		}

		private int AbilityModifierExist(int abilityID)
		{
			for (int i = 0; i < abilityModifierList.Count; i++)
			{
				if (abilityModifierList[i].abilityID == abilityID)
				{
					return i;
				}
			}
			return -1;
		}

		private void ModifyAbilityModifierInList(int ID, Perk perk)
		{
			ModifyAbilityModifier(abilityModifierList[ID], perk);
		}

		private void ModifyAbilityModifier(PerkAbilityModifier abilityModifier, Perk perk)
		{
			abilityModifier.cost += perk.abCost;
			abilityModifier.cooldown += perk.abCooldown;
			abilityModifier.aoeRadius += perk.abAOERadius;
			abilityModifier.effects.damageMin += perk.effects.damageMin;
			abilityModifier.effects.damageMax += perk.effects.damageMax;
			abilityModifier.effects.stunChance += perk.effects.stunChance;
			abilityModifier.effects.slow.duration += perk.effects.duration;
			abilityModifier.effects.slow.slowMultiplier += perk.effects.slow.slowMultiplier;
			abilityModifier.effects.dot.duration += perk.effects.duration;
			abilityModifier.effects.dot.interval += perk.effects.dot.interval;
			abilityModifier.effects.dot.value += perk.effects.dot.value;
			abilityModifier.effects.damageBuff += perk.effects.damageBuff;
			abilityModifier.effects.rangeBuff += perk.effects.rangeBuff;
			abilityModifier.effects.cooldownBuff += perk.effects.cooldownBuff;
			abilityModifier.effects.HPGainMin += perk.effects.HPGainMin;
			abilityModifier.effects.HPGainMax += perk.effects.HPGainMax;
		}

		private int FPSWeaponModifierExist(int prefabID)
		{
			for (int i = 0; i < FPSWeaponModifierList.Count; i++)
			{
				if (FPSWeaponModifierList[i].prefabID == prefabID)
				{
					return i;
				}
			}
			return -1;
		}

		private void ModifyFPSWeaponModifierInList(int ID, Perk perk)
		{
			ModifyUnitStats(FPSWeaponModifierList[ID].stats, perk.stats);
		}

		private void ModifyFPSWeaponModifier(PerkFPSWeaponModifier weaponModifier, Perk perk)
		{
			ModifyUnitStats(weaponModifier.stats, perk.stats);
		}

		private void ModifyUnitStats(UnitStat tgtStats, UnitStat srcStats)
		{
			tgtStats.damageMin += srcStats.damageMin;
			tgtStats.cooldown += srcStats.cooldown;
			tgtStats.clipSize += srcStats.clipSize;
			tgtStats.reloadDuration += srcStats.reloadDuration;
			tgtStats.range += srcStats.range;
			tgtStats.aoeRadius += srcStats.aoeRadius;
			tgtStats.hit += srcStats.hit;
			tgtStats.shieldBreak += srcStats.shieldBreak;
			tgtStats.shieldPierce += srcStats.shieldPierce;
			tgtStats.crit.chance += srcStats.crit.chance;
			tgtStats.crit.dmgMultiplier += srcStats.crit.dmgMultiplier;
			tgtStats.stun.chance += srcStats.stun.chance;
			tgtStats.stun.duration += srcStats.stun.duration;
			tgtStats.slow.duration += srcStats.slow.duration;
			tgtStats.slow.slowMultiplier += srcStats.slow.slowMultiplier;
			tgtStats.dot.duration += srcStats.dot.duration;
			tgtStats.dot.interval += srcStats.dot.interval;
			tgtStats.dot.value += srcStats.dot.value;
			tgtStats.instantKill.chance += srcStats.instantKill.chance;
			tgtStats.instantKill.HPThreshold += srcStats.instantKill.HPThreshold;
		}

		public static List<UnitTower> GetUnlockedTowerList()
		{
			return (!(instance == null)) ? instance.unlockedTowerList : new List<UnitTower>();
		}

		public static List<Ability> GetUnlockedAbilityList()
		{
			return (!(instance == null)) ? instance.unlockedAbilityList : new List<Ability>();
		}

		public static List<FPSWeapon> GetUnlockedWeaponList()
		{
			return (!(instance == null)) ? instance.unlockedWeaponList : new List<FPSWeapon>();
		}

		public static int GetLifeCapModifier()
		{
			return (!(instance == null)) ? instance.lifeCap : 0;
		}

		public static float GetLifeRegenModifier()
		{
			return (!(instance == null)) ? instance.lifeRegen : 0f;
		}

		public static int GetLifeWaveClearedModifier()
		{
			return (!(instance == null)) ? instance.lifeWaveClearedBonus : 0;
		}

		public static List<float> GetRscRegen()
		{
			return (!(instance == null)) ? new List<float>(instance.rscRegen) : new List<float>();
		}

		public static List<float> GetRscGain()
		{
			return (!(instance == null)) ? new List<float>(instance.rscGain) : new List<float>();
		}

		public static List<float> GetRscCreepKilled()
		{
			return (!(instance == null)) ? new List<float>(instance.rscCreepKilledGain) : new List<float>();
		}

		public static List<float> GetRscWaveCleared()
		{
			return (!(instance == null)) ? new List<float>(instance.rscWaveClearedGain) : new List<float>();
		}

		public static List<float> GetRscTowerGain()
		{
			return (!(instance == null)) ? new List<float>(instance.rscRscTowerGain) : new List<float>();
		}

		public static float GetEnergyRegenModifier()
		{
			return (!(instance == null)) ? instance.energyRegen : 0f;
		}

		public static float GetEnergyCapModifier()
		{
			return (!(instance == null)) ? instance.energyCap : 0f;
		}

		public static float GetEnergyCreepKilledModifier()
		{
			return (!(instance == null)) ? instance.energyCreepKilledBonus : 0f;
		}

		public static float GetEnergyWaveClearedModifier()
		{
			return (!(instance == null)) ? instance.energyWaveClearedBonus : 0f;
		}

		public static PerkTowerModifier GetTowerModifier(int prefabID)
		{
			for (int i = 0; i < instance.towerModifierList.Count; i++)
			{
				if (instance.towerModifierList[i].prefabID == prefabID)
				{
					return instance.towerModifierList[i];
				}
			}
			return instance.emptyTowerModifier;
		}

		public static float GetTowerHP(int prefabID)
		{
			if (instance == null)
			{
				return 0f;
			}
			return instance.globalTowerModifier.HP + GetTowerModifier(prefabID).HP;
		}

		public static float GetTowerHPRegen(int prefabID)
		{
			if (instance == null)
			{
				return 0f;
			}
			return instance.globalTowerModifier.HPRegen + GetTowerModifier(prefabID).HPRegen;
		}

		public static float GetTowerHPStagger(int prefabID)
		{
			if (instance == null)
			{
				return 0f;
			}
			return instance.globalTowerModifier.HPStagger + GetTowerModifier(prefabID).HPStagger;
		}

		public static float GetTowerShield(int prefabID)
		{
			if (instance == null)
			{
				return 0f;
			}
			return instance.globalTowerModifier.shield + GetTowerModifier(prefabID).shield;
		}

		public static float GetTowerShieldRegen(int prefabID)
		{
			if (instance == null)
			{
				return 0f;
			}
			return instance.globalTowerModifier.shieldRegen + GetTowerModifier(prefabID).shieldRegen;
		}

		public static float GetTowerShieldStagger(int prefabID)
		{
			if (instance == null)
			{
				return 0f;
			}
			return instance.globalTowerModifier.shieldStagger + GetTowerModifier(prefabID).shieldStagger;
		}

		public static float GetTowerBuildCost(int prefabID)
		{
			if (instance == null)
			{
				return 0f;
			}
			return instance.globalTowerModifier.buildCost + GetTowerModifier(prefabID).buildCost;
		}

		public static float GetTowerUpgradeCost(int prefabID)
		{
			if (instance == null)
			{
				return 0f;
			}
			return instance.globalTowerModifier.upgradeCost + GetTowerModifier(prefabID).upgradeCost;
		}

		public static float GetTowerDamage(int prefabID)
		{
			if (instance == null)
			{
				return 0f;
			}
			return instance.globalTowerModifier.stats.damageMin + GetTowerModifier(prefabID).stats.damageMin;
		}

		public static float GetTowerCD(int prefabID)
		{
			if (instance == null)
			{
				return 0f;
			}
			return instance.globalTowerModifier.stats.cooldown + GetTowerModifier(prefabID).stats.cooldown;
		}

		public static float GetTowerClipSize(int prefabID)
		{
			if (instance == null)
			{
				return 0f;
			}
			return instance.globalTowerModifier.stats.clipSize + GetTowerModifier(prefabID).stats.clipSize;
		}

		public static float GetTowerReloadDuration(int prefabID)
		{
			if (instance == null)
			{
				return 0f;
			}
			return instance.globalTowerModifier.stats.reloadDuration + GetTowerModifier(prefabID).stats.reloadDuration;
		}

		public static float GetTowerRange(int prefabID)
		{
			if (instance == null)
			{
				return 0f;
			}
			return instance.globalTowerModifier.stats.range + GetTowerModifier(prefabID).stats.range;
		}

		public static float GetTowerAOERadius(int prefabID)
		{
			if (instance == null)
			{
				return 0f;
			}
			return instance.globalTowerModifier.stats.aoeRadius + GetTowerModifier(prefabID).stats.aoeRadius;
		}

		public static float GetTowerHit(int prefabID)
		{
			if (instance == null)
			{
				return 0f;
			}
			return instance.globalTowerModifier.stats.hit + GetTowerModifier(prefabID).stats.hit;
		}

		public static float GetTowerDodge(int prefabID)
		{
			return 0f;
		}

		public static float GetTowerCritChance(int prefabID)
		{
			if (instance == null)
			{
				return 0f;
			}
			return instance.globalTowerModifier.stats.crit.chance + GetTowerModifier(prefabID).stats.crit.chance;
		}

		public static float GetTowerCritMultiplier(int prefabID)
		{
			if (instance == null)
			{
				return 0f;
			}
			return instance.globalTowerModifier.stats.crit.dmgMultiplier + GetTowerModifier(prefabID).stats.crit.dmgMultiplier;
		}

		public static float GetTowerShieldBreakMultiplier(int prefabID)
		{
			if (instance == null)
			{
				return 0f;
			}
			return instance.globalTowerModifier.stats.shieldBreak + GetTowerModifier(prefabID).stats.shieldBreak;
		}

		public static float GetTowerShieldPierceMultiplier(int prefabID)
		{
			if (instance == null)
			{
				return 0f;
			}
			return instance.globalTowerModifier.stats.shieldPierce + GetTowerModifier(prefabID).stats.shieldPierce;
		}

		public static Stun GetTowerStunMultiplier(int prefabID)
		{
			if (instance == null)
			{
				return new Stun();
			}
			Stun stun = instance.globalTowerModifier.stats.stun;
			Stun stun2 = GetTowerModifier(prefabID).stats.stun;
			return new Stun(stun.chance + stun2.chance, stun.duration + stun2.duration);
		}

		public static Slow GetTowerSlowMultiplier(int prefabID)
		{
			if (instance == null)
			{
				return new Slow();
			}
			Slow slow = instance.globalTowerModifier.stats.slow;
			Slow slow2 = GetTowerModifier(prefabID).stats.slow;
			return new Slow(slow.slowMultiplier + slow2.slowMultiplier, slow.duration + slow2.duration);
		}

		public static Dot GetTowerDotMultiplier(int prefabID)
		{
			if (instance == null)
			{
				return new Dot();
			}
			Dot dot = instance.globalTowerModifier.stats.dot;
			Dot dot2 = GetTowerModifier(prefabID).stats.dot;
			return new Dot(dot.duration + dot2.duration, dot.interval + dot2.interval, dot.value + dot2.value);
		}

		public static InstantKill GetTowerInstantKillMultiplier(int prefabID)
		{
			if (instance == null)
			{
				return new InstantKill();
			}
			InstantKill instantKill = instance.globalTowerModifier.stats.instantKill;
			InstantKill instantKill2 = GetTowerModifier(prefabID).stats.instantKill;
			return new InstantKill(instantKill.chance + instantKill2.chance, instantKill.HPThreshold + instantKill2.HPThreshold);
		}

		public static Stun ModifyStunWithPerkBonus(Stun stun, int prefabID, int type = 0)
		{
			Stun stun2 = new Stun();
			switch (type)
			{
			case 0:
				stun2 = GetTowerStunMultiplier(prefabID);
				break;
			case 1:
				stun2 = GetFPSWeaponStunMultiplier(prefabID);
				break;
			}
			stun.chance *= 1f + stun2.chance;
			stun.duration *= 1f + stun2.duration;
			return stun;
		}

		public static Slow ModifySlowWithPerkBonus(Slow slow, int prefabID, int type = 0)
		{
			Slow slow2 = new Slow();
			switch (type)
			{
			case 0:
				slow2 = GetTowerSlowMultiplier(prefabID);
				break;
			case 1:
				slow2 = GetFPSWeaponSlowMultiplier(prefabID);
				break;
			case 2:
				slow2 = GetAbilitySlowMultiplier(prefabID);
				break;
			}
			slow.slowMultiplier *= 1f - slow2.slowMultiplier;
			slow.duration *= 1f + slow2.duration;
			return slow;
		}

		public static Dot ModifyDotWithPerkBonus(Dot dot, int prefabID, int type = 0)
		{
			Dot dot2 = new Dot();
			switch (type)
			{
			case 0:
				dot2 = GetTowerDotMultiplier(prefabID);
				break;
			case 1:
				dot2 = GetFPSWeaponDotMultiplier(prefabID);
				break;
			case 2:
				dot2 = GetAbilityDotMultiplier(prefabID);
				break;
			}
			dot.duration *= 1f + dot2.duration;
			dot.value *= 1f + dot2.value;
			return dot;
		}

		public static InstantKill ModifyInstantKillWithPerkBonus(InstantKill instKill, int prefabID, int type = 0)
		{
			InstantKill instantKill = new InstantKill();
			switch (type)
			{
			case 0:
				instantKill = GetTowerInstantKillMultiplier(prefabID);
				break;
			case 1:
				instantKill = GetFPSWeaponInstantKillMultiplier(prefabID);
				break;
			}
			instKill.chance *= 1f + instantKill.chance;
			instKill.HPThreshold *= 1f + instantKill.HPThreshold;
			return instKill;
		}

		public static PerkFPSWeaponModifier GetFPSWeaponModifier(int prefabID)
		{
			for (int i = 0; i < instance.FPSWeaponModifierList.Count; i++)
			{
				if (instance.FPSWeaponModifierList[i].prefabID == prefabID)
				{
					return instance.FPSWeaponModifierList[i];
				}
			}
			return instance.emptyFPSWeaponModifier;
		}

		public static float GetFPSWeaponDamage(int prefabID)
		{
			if (instance == null)
			{
				return 0f;
			}
			return instance.globalFPSWeaponModifier.stats.damageMin + GetFPSWeaponModifier(prefabID).stats.damageMin;
		}

		public static float GetFPSWeaponCD(int prefabID)
		{
			if (instance == null)
			{
				return 0f;
			}
			return instance.globalFPSWeaponModifier.stats.cooldown + GetFPSWeaponModifier(prefabID).stats.cooldown;
		}

		public static float GetFPSWeaponClipSize(int prefabID)
		{
			if (instance == null)
			{
				return 0f;
			}
			return instance.globalFPSWeaponModifier.stats.clipSize + GetFPSWeaponModifier(prefabID).stats.clipSize;
		}

		public static float GetFPSWeaponReloadDuration(int prefabID)
		{
			if (instance == null)
			{
				return 0f;
			}
			return instance.globalFPSWeaponModifier.stats.reloadDuration + GetFPSWeaponModifier(prefabID).stats.reloadDuration;
		}

		public static float GetFPSWeaponAOERadius(int prefabID)
		{
			if (instance == null)
			{
				return 0f;
			}
			return instance.globalFPSWeaponModifier.stats.aoeRadius + GetFPSWeaponModifier(prefabID).stats.aoeRadius;
		}

		public static float GetFPSWeaponShieldBreak(int prefabID)
		{
			if (instance == null)
			{
				return 0f;
			}
			return instance.globalFPSWeaponModifier.stats.shieldBreak + GetFPSWeaponModifier(prefabID).stats.shieldBreak;
		}

		public static float GetFPSWeaponShieldPierce(int prefabID)
		{
			if (instance == null)
			{
				return 0f;
			}
			return instance.globalFPSWeaponModifier.stats.shieldPierce + GetFPSWeaponModifier(prefabID).stats.shieldPierce;
		}

		public static float GetFPSWeaponCritChance(int prefabID)
		{
			if (instance == null)
			{
				return 0f;
			}
			return instance.globalFPSWeaponModifier.stats.crit.chance + GetFPSWeaponModifier(prefabID).stats.crit.chance;
		}

		public static float GetFPSWeaponCritMultiplier(int prefabID)
		{
			if (instance == null)
			{
				return 0f;
			}
			return instance.globalFPSWeaponModifier.stats.crit.dmgMultiplier + GetFPSWeaponModifier(prefabID).stats.crit.dmgMultiplier;
		}

		public static Stun GetFPSWeaponStunMultiplier(int prefabID)
		{
			if (instance == null)
			{
				return new Stun();
			}
			Stun stun = instance.globalFPSWeaponModifier.stats.stun;
			Stun stun2 = GetFPSWeaponModifier(prefabID).stats.stun;
			return new Stun(stun.chance + stun2.chance, stun.duration + stun2.duration);
		}

		public static Slow GetFPSWeaponSlowMultiplier(int prefabID)
		{
			if (instance == null)
			{
				return new Slow();
			}
			Slow slow = instance.globalFPSWeaponModifier.stats.slow;
			Slow slow2 = GetFPSWeaponModifier(prefabID).stats.slow;
			return new Slow(slow.slowMultiplier + slow2.slowMultiplier, slow.duration + slow2.duration);
		}

		public static Dot GetFPSWeaponDotMultiplier(int prefabID)
		{
			if (instance == null)
			{
				return new Dot();
			}
			Dot dot = instance.globalFPSWeaponModifier.stats.dot;
			Dot dot2 = GetFPSWeaponModifier(prefabID).stats.dot;
			return new Dot(dot.duration + dot2.duration, dot.interval + dot2.interval, dot.value + dot2.value);
		}

		public static InstantKill GetFPSWeaponInstantKillMultiplier(int prefabID)
		{
			if (instance == null)
			{
				return new InstantKill();
			}
			InstantKill instantKill = instance.globalFPSWeaponModifier.stats.instantKill;
			InstantKill instantKill2 = GetFPSWeaponModifier(prefabID).stats.instantKill;
			return new InstantKill(instantKill.chance + instantKill2.chance, instantKill.HPThreshold + instantKill2.HPThreshold);
		}

		public static PerkAbilityModifier GetAbilityModifier(int prefabID)
		{
			for (int i = 0; i < instance.abilityModifierList.Count; i++)
			{
				if (instance.abilityModifierList[i].abilityID == prefabID)
				{
					return instance.abilityModifierList[i];
				}
			}
			return instance.emptyAbilityModifier;
		}

		public static float GetAbilityCost(int abilityID)
		{
			if (instance == null)
			{
				return 0f;
			}
			return instance.globalAbilityModifier.cost + GetAbilityModifier(abilityID).cost;
		}

		public static float GetAbilityCooldown(int abilityID)
		{
			if (instance == null)
			{
				return 0f;
			}
			return instance.globalAbilityModifier.cooldown + GetAbilityModifier(abilityID).cooldown;
		}

		public static float GetAbilityAOERadius(int abilityID)
		{
			if (instance == null)
			{
				return 0f;
			}
			return instance.globalAbilityModifier.aoeRadius + GetAbilityModifier(abilityID).aoeRadius;
		}

		public static float GetAbilityDuration(int abilityID)
		{
			if (instance == null)
			{
				return 0f;
			}
			return instance.globalAbilityModifier.effects.duration + GetAbilityModifier(abilityID).effects.duration;
		}

		public static float GetAbilityDamage(int abilityID)
		{
			if (instance == null)
			{
				return 0f;
			}
			return instance.globalAbilityModifier.effects.damageMin + GetAbilityModifier(abilityID).effects.damageMin;
		}

		public static float GetAbilityStunChance(int abilityID)
		{
			if (instance == null)
			{
				return 0f;
			}
			return instance.globalAbilityModifier.effects.stunChance + GetAbilityModifier(abilityID).effects.stunChance;
		}

		public static float GetAbilityDamageBuff(int abilityID)
		{
			if (instance == null)
			{
				return 0f;
			}
			return instance.globalAbilityModifier.effects.damageBuff + GetAbilityModifier(abilityID).effects.damageBuff;
		}

		public static float GetAbilityRangeBuff(int abilityID)
		{
			if (instance == null)
			{
				return 0f;
			}
			return instance.globalAbilityModifier.effects.rangeBuff + GetAbilityModifier(abilityID).effects.rangeBuff;
		}

		public static float GetAbilityCooldownBuff(int abilityID)
		{
			if (instance == null)
			{
				return 0f;
			}
			return instance.globalAbilityModifier.effects.cooldownBuff + GetAbilityModifier(abilityID).effects.cooldownBuff;
		}

		public static float GetAbilityHPGain(int abilityID)
		{
			if (instance == null)
			{
				return 0f;
			}
			return instance.globalAbilityModifier.effects.HPGainMin + GetAbilityModifier(abilityID).effects.HPGainMin;
		}

		public static Slow GetAbilitySlowMultiplier(int prefabID)
		{
			if (instance == null)
			{
				return new Slow();
			}
			Slow slow = instance.globalAbilityModifier.effects.slow;
			Slow slow2 = GetAbilityModifier(prefabID).effects.slow;
			return new Slow(slow.slowMultiplier + slow2.slowMultiplier, slow.duration + slow2.duration);
		}

		public static Dot GetAbilityDotMultiplier(int prefabID)
		{
			if (instance == null)
			{
				return new Dot();
			}
			Dot dot = instance.globalAbilityModifier.effects.dot;
			Dot dot2 = GetAbilityModifier(prefabID).effects.dot;
			return new Dot(dot.duration + dot2.duration, dot.interval + dot2.interval, dot.value + dot2.value);
		}
	}
}
