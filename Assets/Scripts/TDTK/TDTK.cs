using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace TDTK
{
	public class TDTK : MonoBehaviour
	{
		public delegate void GameMessageHandler(string msg);

		public delegate void LifeHandler(int value);

		public delegate void FastForwardHandler(bool ff);

		public delegate void GameOverHandler(bool playerWon);

		public delegate void NewWaveHandler(int waveID);

		public delegate void WaveClearedHandler(int waveID);

		public delegate void EnableSpawnHandler();

		public delegate void SpawnTimerHandler(float time);

		public delegate void ResourceHandler(List<int> changedValueList);

		public delegate void DragNDropHandler(bool flag);

		public delegate void NewBuildableTowerHandler(UnitTower tower);

		public delegate void NewUnitHandler(Unit unit);

		public delegate void UnitDamagedHandler(Unit unit);

		public delegate void CreepDestroyedHandler(UnitCreep creep);

		public delegate void CreepDestinationHandler(UnitCreep creep);

		public delegate void TowerConstructingHandler(UnitTower tower);

		public delegate void TowerConstructedHandler(UnitTower tower);

		public delegate void TowerUpgradingHandler(UnitTower tower);

		public delegate void TowerUpgradedHandler(UnitTower tower);

		public delegate void TowerSoldHandler(UnitTower tower);

		public delegate void TowerDestroyedHandler(UnitTower tower);

		public delegate void NewAbilityHandler(Ability ability);

		public delegate void AbilityActivatedHandler(Ability ability);

		public delegate void AbilityTargetSelectModeHandler(bool flag);

		public delegate void AbilityReadyHandler(Ability ability);

		public delegate void EnergyFullHandler();

		public delegate void FPSModeHandler(bool flag);

		public delegate void FPSShootHandler();

		public delegate void FPSReloadHandler(bool flag);

		public delegate void FPSSwitchWeaponHandler();

		public delegate void OnFPSSwitchCamHandler();

		public delegate void PerkPurchasedHandler(Perk perk);

		public delegate void PerkPointHandler();

		private static List<UnitTower> towerDBList = new List<UnitTower>();

		private static List<Ability> abilityDBList = new List<Ability>();

		private static List<Perk> perkDBList = new List<Perk>();

		private static List<FPSWeapon> fpsWeaponDBList = new List<FPSWeapon>();

		private static bool init = false;

		public static event GameMessageHandler onGameMessageE;

		public static event LifeHandler onLifeE;

		public static event FastForwardHandler onFastForwardE;

		public static event GameOverHandler onGameOverE;

		public static event NewWaveHandler onNewWaveE;

		public static event WaveClearedHandler onWaveClearedE;

		public static event EnableSpawnHandler onEnableSpawnE;

		public static event SpawnTimerHandler onSpawnTimerE;

		public static event ResourceHandler onResourceE;

		public static event DragNDropHandler onDragNDropE;

		public static event NewBuildableTowerHandler onNewBuildableTowerE;

		public static event NewUnitHandler onNewUnitE;

		public static event UnitDamagedHandler onUnitDamagedE;

		public static event CreepDestroyedHandler onCreepDestroyedE;

		public static event CreepDestinationHandler onCreepDestinationE;

		public static event TowerConstructingHandler onTowerConstructingE;

		public static event TowerConstructedHandler onTowerConstructedE;

		public static event TowerUpgradingHandler onTowerUpgradingE;

		public static event TowerUpgradedHandler onTowerUpgradedE;

		public static event TowerSoldHandler onTowerSoldE;

		public static event TowerDestroyedHandler onTowerDestroyedE;

		public static event NewAbilityHandler onNewAbilityE;

		public static event AbilityActivatedHandler onAbilityActivatedE;

		public static event AbilityTargetSelectModeHandler onAbilitySelectingTargetE;

		public static event AbilityReadyHandler onAbilityReadyE;

		public static event EnergyFullHandler onEnergyFullE;

		public static event FPSModeHandler onFPSModeE;

		public static event FPSShootHandler onFPSShootE;

		public static event FPSReloadHandler onFPSReloadE;

		public static event FPSSwitchWeaponHandler onFPSSwitchWeaponE;

		public static event OnFPSSwitchCamHandler onFPSSwitchCameraE;

		public static event PerkPurchasedHandler onPerkPurchasedE;

		public static event PerkPointHandler onPerkPointE;

		public static int GetLayerCreep()
		{
			return 31;
		}

		public static int GetLayerCreepF()
		{
			return 30;
		}

		public static int GetLayerTower()
		{
			return 29;
		}

		public static int GetLayerShootObject()
		{
			return 28;
		}

		public static int GetLayerPlatform()
		{
			return 27;
		}

		public static int GetLayerTerrain()
		{
			return 26;
		}

		public static int GetLayerUI()
		{
			return 5;
		}

		public static void OnGameMessage(string msg)
		{
			if (TDTK.onGameMessageE != null)
			{
				TDTK.onGameMessageE(msg);
			}
		}

		public static void OnLife(int valueChanged)
		{
			if (TDTK.onLifeE != null)
			{
				TDTK.onLifeE(valueChanged);
			}
		}

		public static void OnFastForward(bool ff)
		{
			if (TDTK.onFastForwardE != null)
			{
				TDTK.onFastForwardE(ff);
			}
		}

		public static void OnGameOver(bool playerWon)
		{
			if (TDTK.onGameOverE != null)
			{
				TDTK.onGameOverE(playerWon);
			}
		}

		public static void OnNewWave(int waveID)
		{
			if (TDTK.onNewWaveE != null)
			{
				TDTK.onNewWaveE(waveID);
			}
		}

		public static void OnWaveCleared(int waveID)
		{
			if (TDTK.onWaveClearedE != null)
			{
				TDTK.onWaveClearedE(waveID);
			}
		}

		public static void OnEnableSpawn()
		{
			if (TDTK.onEnableSpawnE != null)
			{
				TDTK.onEnableSpawnE();
			}
		}

		public static void OnSpawnTimer(float time)
		{
			if (TDTK.onSpawnTimerE != null)
			{
				TDTK.onSpawnTimerE(time);
			}
		}

		public static void OnResource(List<int> valueChangedList)
		{
			if (TDTK.onResourceE != null)
			{
				TDTK.onResourceE(valueChangedList);
			}
		}

		public static void OnDragNDrop(bool flag)
		{
			if (TDTK.onDragNDropE != null)
			{
				TDTK.onDragNDropE(flag);
			}
		}

		public static void OnNewBuildableTower(UnitTower tower)
		{
			if (TDTK.onNewBuildableTowerE != null)
			{
				TDTK.onNewBuildableTowerE(tower);
			}
		}

		public static void OnNewUnit(Unit unit)
		{
			if (TDTK.onNewUnitE != null)
			{
				TDTK.onNewUnitE(unit);
			}
		}

		public static void OnUnitDamaged(Unit unit)
		{
			if (TDTK.onUnitDamagedE != null)
			{
				TDTK.onUnitDamagedE(unit);
			}
		}

		public static void OnUnitCreepDestroyed(UnitCreep unit)
		{
			if (TDTK.onCreepDestroyedE != null)
			{
				TDTK.onCreepDestroyedE(unit);
			}
		}

		public static void OnCreepDestination(UnitCreep creep)
		{
			if (TDTK.onCreepDestinationE != null)
			{
				TDTK.onCreepDestinationE(creep);
			}
		}

		public static void OnTowerConstructing(UnitTower tower)
		{
			if (TDTK.onTowerConstructingE != null)
			{
				TDTK.onTowerConstructingE(tower);
			}
		}

		public static void OnTowerConstructed(UnitTower tower)
		{
			if (TDTK.onTowerConstructedE != null)
			{
				TDTK.onTowerConstructedE(tower);
			}
		}

		public static void OnTowerUpgrading(UnitTower tower)
		{
			if (TDTK.onTowerUpgradingE != null)
			{
				TDTK.onTowerUpgradingE(tower);
			}
		}

		public static void OnTowerUpgraded(UnitTower tower)
		{
			if (TDTK.onTowerUpgradedE != null)
			{
				TDTK.onTowerUpgradedE(tower);
			}
		}

		public static void OnTowerSold(UnitTower tower)
		{
			if (TDTK.onTowerSoldE != null)
			{
				TDTK.onTowerSoldE(tower);
			}
		}

		public static void OnUnitTowerDestroyed(UnitTower unit)
		{
			if (TDTK.onTowerDestroyedE != null)
			{
				TDTK.onTowerDestroyedE(unit);
			}
		}

		public static void OnNewAbility(Ability ability)
		{
			if (TDTK.onNewAbilityE != null)
			{
				TDTK.onNewAbilityE(ability);
			}
		}

		public static void OnAbilityActivated(Ability ability)
		{
			if (TDTK.onAbilityActivatedE != null)
			{
				TDTK.onAbilityActivatedE(ability);
			}
		}

		public static void OnAbilityTargetSelectModeE(bool flag)
		{
			if (TDTK.onAbilitySelectingTargetE != null)
			{
				TDTK.onAbilitySelectingTargetE(flag);
			}
		}

		public static void OnAbilityReady(Ability ability)
		{
			if (TDTK.onAbilityReadyE != null)
			{
				TDTK.onAbilityReadyE(ability);
			}
		}

		public static void OnEnergyFull()
		{
			if (TDTK.onEnergyFullE != null)
			{
				TDTK.onEnergyFullE();
			}
		}

		public static void OnFPSMode(bool flag)
		{
			if (TDTK.onFPSModeE != null)
			{
				TDTK.onFPSModeE(flag);
			}
		}

		public static void OnFPSShoot()
		{
			if (TDTK.onFPSShootE != null)
			{
				TDTK.onFPSShootE();
			}
		}

		public static void OnFPSReload(bool flag)
		{
			if (TDTK.onFPSReloadE != null)
			{
				TDTK.onFPSReloadE(flag);
			}
		}

		public static void OnFPSSwitchWeapon()
		{
			if (TDTK.onFPSSwitchWeaponE != null)
			{
				TDTK.onFPSSwitchWeaponE();
			}
		}

		public static void OnFPSSwitchCamera()
		{
			if (TDTK.onFPSSwitchCameraE != null)
			{
				TDTK.onFPSSwitchCameraE();
			}
		}

		public static void OnPerkPurchased(Perk perk)
		{
			if (TDTK.onPerkPurchasedE != null)
			{
				TDTK.onPerkPurchasedE(perk);
			}
		}

		public static void OnPerkPoint()
		{
			if (TDTK.onPerkPointE != null)
			{
				TDTK.onPerkPointE();
			}
		}

		public static List<UnitTower> GetTowerDBList()
		{
			return new List<UnitTower>(towerDBList);
		}

		public static List<Ability> GetAbilityDBList()
		{
			return new List<Ability>(abilityDBList);
		}

		public static List<Perk> GetPerkDBList()
		{
			return new List<Perk>(perkDBList);
		}

		public static List<FPSWeapon> GetFpsWeaponDBList()
		{
			return new List<FPSWeapon>(fpsWeaponDBList);
		}

		public static UnitTower GetDBTower(int prefabID)
		{
			for (int i = 0; i < towerDBList.Count; i++)
			{
				if (towerDBList[i].prefabID == prefabID)
				{
					return towerDBList[i];
				}
			}
			return null;
		}

		public static Ability GetDBAbility(int prefabID)
		{
			for (int i = 0; i < abilityDBList.Count; i++)
			{
				if (abilityDBList[i].ID == prefabID)
				{
					return abilityDBList[i];
				}
			}
			return null;
		}

		public static Perk GetDBPerk(int prefabID)
		{
			for (int i = 0; i < perkDBList.Count; i++)
			{
				if (perkDBList[i].ID == prefabID)
				{
					return perkDBList[i];
				}
			}
			return null;
		}

		public static FPSWeapon GetDBFpsWeapon(int prefabID)
		{
			for (int i = 0; i < fpsWeaponDBList.Count; i++)
			{
				if (fpsWeaponDBList[i].prefabID == prefabID)
				{
					return fpsWeaponDBList[i];
				}
			}
			return null;
		}

		public static void InitDB()
		{
			if (!init)
			{
				init = true;
				towerDBList = TowerDB.Load();
				abilityDBList = AbilityDB.Load();
				perkDBList = PerkDB.Load();
				fpsWeaponDBList = FPSWeaponDB.Load();
			}
		}

		public static List<Unit> GetUnitInRange(Vector3 pos, float range, LayerMask mask)
		{
			return GetUnitInRange(pos, range, 0f, mask);
		}

		public static List<Unit> GetUnitInRange(Vector3 pos, float range, float minRange, LayerMask mask)
		{
			List<Unit> list = new List<Unit>();
			Collider[] array = Physics.OverlapSphere(pos, range, mask);
			for (int i = 0; i < array.Length; i++)
			{
				Unit component = array[i].GetComponent<Unit>();
				if (!(component == null) && !component.IsDestroyed() && !(Vector3.Distance(pos, component.thisT.position) > range))
				{
					list.Add(component);
				}
			}
			if (minRange > 0f)
			{
				for (int j = 0; j < list.Count; j++)
				{
					if (!(Vector3.Distance(pos, list[j].thisT.position) > minRange))
					{
						list.RemoveAt(j);
						j--;
					}
				}
			}
			return list;
		}

		public static Vector3 GetTouchPosition(int pointerID)
		{
			for (int i = 0; i < UnityEngine.Input.touchCount; i++)
			{
				if (Input.touches[i].fingerId == pointerID)
				{
					return Input.touches[i].position;
				}
			}
			return new Vector3(0f, -50f, 0f);
		}

		public static bool IsTouchStarting(int pointerID)
		{
			for (int i = 0; i < UnityEngine.Input.touchCount; i++)
			{
				if (Input.touches[i].fingerId == pointerID && Input.touches[i].phase == TouchPhase.Began)
				{
					return true;
				}
			}
			return false;
		}

		public static bool IsTouchEnding(int pointerID)
		{
			for (int i = 0; i < UnityEngine.Input.touchCount; i++)
			{
				if (Input.touches[i].fingerId == pointerID && Input.touches[i].phase == TouchPhase.Ended)
				{
					return true;
				}
			}
			return false;
		}
	}
}
