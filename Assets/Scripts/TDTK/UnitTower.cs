using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TDTK
{
	public class UnitTower : Unit
	{
		private enum _Construction
		{
			None,
			Constructing,
			Deconstructing
		}

		[Header("Tower Setting")]
		public _TowerType type;

		public _TargetMode targetMode;

		[Space(8f)]
		public bool disableInBuildManager;

		public bool canBeSold = true;

		[Space(8f)]
		public bool disableFPS;

		public int FPSWeaponID = -1;

		[Header("Upgrade Setting")]
		public List<UnitTower> nextLevelTowerList = new List<UnitTower>();

		[HideInInspector]
		public UnitTower prevLevelTower;

		[HideInInspector]
		public List<int> value = new List<int>();

		[Header("Visual Effect (Building)")]
		public bool hideWhenBuilding;

		[Space(8f)]
		public GameObject buildingEffect;

		public bool destroyBuildingEffect = true;

		public float destroyBuildingDuration = 1.5f;

		[Space(8f)]
		public GameObject builtEffect;

		public bool destroyBuiltEffect = true;

		public float destroyBuiltDuration = 1.5f;

		private _Construction construction;

		[HideInInspector]
		public PlatformTD occupiedPlatform;

		[HideInInspector]
		public NodeTD occupiedNode;

		private float buildProgress;

		private bool isSampleTower;

		private UnitTower srcTower;

		private static bool inDragNDropRoutine;

		private int level = 1;

		public override bool IsInConstruction()
		{
			return (construction != 0) ? true : false;
		}

		public override void Awake()
		{
			isTower = true;
			base.gameObject.layer = TDTK.GetLayerTower();
			base.Awake();
			for (int i = 0; i < nextLevelTowerList.Count; i++)
			{
				if (nextLevelTowerList[i] == null)
				{
					nextLevelTowerList.RemoveAt(i);
					i--;
				}
			}
			if (stats.Count == 0)
			{
				stats.Add(new UnitStat());
			}
		}

		public override void Start()
		{
			base.Start();
		}

		public void InitTower(int ID)
		{
			Init();
			instanceID = ID;
			value = stats[currentActiveStat].cost;
			int resourceCount = ResourceManager.GetResourceCount();
			for (int i = 0; i < stats.Count; i++)
			{
				UnitStat unitStat = stats[i];
				unitStat.slow.effectID = instanceID;
				unitStat.dot.effectID = instanceID;
				unitStat.buff.effectID = instanceID;
				if (unitStat.rscGain.Count != resourceCount)
				{
					while (unitStat.rscGain.Count < resourceCount)
					{
						unitStat.rscGain.Add(0);
					}
					while (unitStat.rscGain.Count > resourceCount)
					{
						unitStat.rscGain.RemoveAt(unitStat.rscGain.Count - 1);
					}
				}
				if (unitStat.cost.Count != resourceCount)
				{
					while (unitStat.cost.Count < resourceCount)
					{
						unitStat.cost.Add(0);
					}
					while (unitStat.cost.Count > resourceCount)
					{
						unitStat.cost.RemoveAt(unitStat.cost.Count - 1);
					}
				}
			}
			if (type == _TowerType.Turret)
			{
				SetupTargetLayerMask();
				StartCoroutine(ScanForTargetRoutine());
				StartCoroutine(TurretRoutine());
			}
			if (type == _TowerType.AOE)
			{
				SetupTargetLayerMask();
				StartCoroutine(AOETowerRoutine());
			}
			if (type == _TowerType.Support)
			{
				maskTarget = 1 << TDTK.GetLayerTower();
				StartCoroutine(SupportRoutine());
			}
			if (type == _TowerType.Resource)
			{
				StartCoroutine(ResourceTowerRoutine());
			}
			if (type == _TowerType.Mine)
			{
				StartCoroutine(MineRoutine());
			}
		}

		private void SetupTargetLayerMask()
		{
			if (targetMode == _TargetMode.Hybrid)
			{
				LayerMask mask = 1 << TDTK.GetLayerCreep();
				LayerMask mask2 = 1 << TDTK.GetLayerCreepF();
				maskTarget = ((int)mask | (int)mask2);
			}
			else if (targetMode == _TargetMode.Air)
			{
				maskTarget = 1 << TDTK.GetLayerCreepF();
			}
			else if (targetMode == _TargetMode.Ground)
			{
				maskTarget = 1 << TDTK.GetLayerCreep();
			}
		}

		public void SetPlatform(PlatformTD platform, NodeTD node)
		{
			occupiedPlatform = platform;
			occupiedNode = node;
		}

		public override void IterateTargetPriority(int i = 1)
		{
			base.IterateTargetPriority(i);
		}

		public override void ChangeScanAngle(int angle)
		{
			base.ChangeScanAngle(angle);
			GameControl.TowerScanAngleChanged(this);
		}

		public void UnBuild()
		{
			StartCoroutine(Building(isUpgrade: false, stats[currentActiveStat].unBuildDuration, reverse: true));
		}

		public void Build(bool isUpgrade = false)
		{
			StartCoroutine(Building(isUpgrade, stats[currentActiveStat].buildDuration));
		}

		private IEnumerator Building(bool isUpgrade, float duration, bool reverse = false)
		{
			construction = ((!reverse) ? _Construction.Constructing : _Construction.Deconstructing);
			float builtTime = 0f;
			buildProgress = 0f;
			if (hideWhenBuilding)
			{
				Utility.DisableAllChildRendererRecursively(thisT);
			}
			if (!isUpgrade)
			{
				TDTK.OnTowerConstructing(this);
			}
			else
			{
				TDTK.OnTowerUpgrading(this);
			}
			if (buildingEffect != null)
			{
				if (!destroyBuildingEffect)
				{
					ObjectPoolManager.Spawn(buildingEffect, thisT.position, thisT.rotation);
				}
				else
				{
					ObjectPoolManager.Spawn(buildingEffect, thisT.position, thisT.rotation, destroyBuildingDuration);
				}
			}
			yield return null;
			if (!reverse)
			{
				PlayAnimConstruct();
			}
			else
			{
				PlayAnimDeconstruct();
			}
			do
			{
				yield return null;
				builtTime += Time.deltaTime;
				if (!reverse)
				{
					buildProgress = builtTime / duration;
				}
				else
				{
					buildProgress = (duration - builtTime) / duration;
				}
			}
			while (!(builtTime > duration));
			construction = _Construction.None;
			buildProgress = 1f;
			if (!reverse)
			{
				Utility.EnbleAllChildRendererRecursively(thisT);
				if (!isUpgrade)
				{
					TDTK.OnTowerConstructed(this);
				}
				else
				{
					TDTK.OnTowerUpgraded(this);
				}
				if (builtEffect != null)
				{
					if (!destroyBuiltEffect)
					{
						ObjectPoolManager.Spawn(builtEffect, thisT.position, thisT.rotation);
					}
					else
					{
						ObjectPoolManager.Spawn(builtEffect, thisT.position, thisT.rotation, destroyBuiltDuration);
					}
				}
			}
			if (reverse)
			{
				ResourceManager.GainResource(GetValue());
				TDTK.OnTowerSold(this);
				RemoveFromGame();
			}
		}

		public float GetBuildProgress()
		{
			if (construction != 0)
			{
				return buildProgress;
			}
			return 0f;
		}

		public void Sell()
		{
			if (canBeSold)
			{
				UnBuild();
			}
		}

		public bool IsSampleTower()
		{
			return isSampleTower;
		}

		public void SetAsSampleTower(UnitTower tower)
		{
			isSampleTower = true;
			srcTower = tower;
			thisT.position = new Vector3(0f, 99999f, 0f);
			int resourceCount = ResourceManager.GetResourceCount();
			for (int i = 0; i < stats.Count; i++)
			{
				if (stats[i].cost.Count != resourceCount)
				{
					while (stats[i].cost.Count < resourceCount)
					{
						stats[i].cost.Add(0);
					}
					while (stats[i].cost.Count > resourceCount)
					{
						stats[i].cost.RemoveAt(stats[i].cost.Count - 1);
					}
				}
			}
		}

		public IEnumerator DragNDropRoutine(int pointerID = -1)
		{
			GameControl.SelectTower(this);
			yield return null;
			Vector3 zero = Vector3.zero;
			TDTK.OnDragNDrop(flag: true);
			inDragNDropRoutine = true;
			while (inDragNDropRoutine && !Input.GetKeyDown(KeyCode.Escape))
			{
				bool invalidCursor = false;
				Vector3 cursorPos = (pointerID >= 0) ? TDTK.GetTouchPosition(pointerID) : UnityEngine.Input.mousePosition;
				if (cursorPos.magnitude < 0f)
				{
					invalidCursor = true;
				}
				BuildInfo buildInfo = null;
				if (!invalidCursor)
				{
					buildInfo = BuildManager.CheckBuildPoint(cursorPos, prefabID);
					if (buildInfo.status == _TileStatus.NoPlatform)
					{
						Ray ray = Camera.main.ScreenPointToRay(cursorPos);
						if (Physics.Raycast(ray, out RaycastHit hitInfo, float.PositiveInfinity))
						{
							thisT.position = hitInfo.point;
						}
						else
						{
							thisT.position = ray.GetPoint(30f);
						}
					}
					else
					{
						thisT.position = buildInfo.position;
						thisT.rotation = buildInfo.platform.thisT.rotation;
					}
					IndicatorControl.SetBuildTileIndicator(buildInfo);
				}
				bool cursorOnUI = UI.IsCursorOnUI(pointerID);
				if (pointerID < 0)
				{
					if (Input.GetMouseButtonDown(0))
					{
						if (!cursorOnUI)
						{
							string text = BuildManager._BuildTower(srcTower, buildInfo);
							if (text != string.Empty)
							{
								TDTK.OnGameMessage(text);
							}
						}
						break;
					}
					if (Input.GetMouseButtonDown(1))
					{
						break;
					}
				}
				else if (TDTK.IsTouchEnding(pointerID))
				{
					if (!cursorOnUI)
					{
						string text2 = BuildManager._BuildTower(srcTower, buildInfo);
						if (text2 != string.Empty)
						{
							TDTK.OnGameMessage(text2);
						}
					}
					break;
				}
				yield return null;
			}
			inDragNDropRoutine = false;
			TDTK.OnDragNDrop(flag: false);
			IndicatorControl.SetDragNDropPhase(flag: false);
			thisObj.SetActive(value: false);
		}

		public static void ExitDragNDrop()
		{
			inDragNDropRoutine = false;
		}

		public static bool InDragNDrop()
		{
			return inDragNDropRoutine;
		}

		public override void Update()
		{
			base.Update();
		}

		public override void FixedUpdate()
		{
			base.FixedUpdate();
		}

		private IEnumerator AOETowerRoutine()
		{
			while (true)
			{
				yield return new WaitForSeconds(GetCooldown());
				while (stunned || IsInConstruction())
				{
					yield return null;
				}
				List<Unit> targetList = TDTK.GetUnitInRange(thisT.position, GetRange(), maskTarget);
				for (int i = 0; i < targetList.Count; i++)
				{
					AttackInstance attackInstance = new AttackInstance(this, targetList[i]);
					attackInstance.Process();
					targetList[i].ApplyEffect(attackInstance);
				}
				SpawnEffectObject();
			}
		}

		private IEnumerator ResourceTowerRoutine()
		{
			while (true)
			{
				yield return new WaitForSeconds(GetCooldown());
				while (stunned || IsInConstruction())
				{
					yield return null;
				}
				SpawnEffectObject();
				ResourceManager.GainResource(GetResourceGain(), PerkManager.GetRscTowerGain());
			}
		}

		private IEnumerator MineRoutine()
		{
			LayerMask maskTarget = 1 << TDTK.GetLayerCreep();
			while (true)
			{
				if (!destroyed && !IsInConstruction())
				{
					Collider[] array = Physics.OverlapSphere(thisT.position, GetRange(), maskTarget);
					if (array.Length > 0)
					{
						List<Unit> unitInRange = TDTK.GetUnitInRange(thisT.position, GetAOERadius(), maskTarget);
						for (int i = 0; i < unitInRange.Count; i++)
						{
							AttackInstance attackInstance = new AttackInstance(this, unitInRange[i]);
							attackInstance.Process();
							unitInRange[i].ApplyEffect(attackInstance);
						}
						SpawnEffectObject();
						Destroyed();
					}
				}
				yield return new WaitForSeconds(0.1f);
			}
		}

		public int GetLevel()
		{
			return level;
		}

		public void SetLevel(int lvl)
		{
			level = lvl;
		}

		public int ReadyToBeUpgrade()
		{
			if (currentActiveStat < stats.Count - 1)
			{
				return 1;
			}
			if (nextLevelTowerList.Count > 0)
			{
				if (nextLevelTowerList.Count >= 2 && nextLevelTowerList[1] != null)
				{
					return 2;
				}
				if (nextLevelTowerList.Count >= 1 && nextLevelTowerList[0] != null)
				{
					return 1;
				}
			}
			return 0;
		}

		public string Upgrade(int ID = 0)
		{
			if (nextLevelTowerList.Count == 0 && currentActiveStat >= stats.Count - 1)
			{
				return "Tower is at maximum level!";
			}
			List<int> cost = GetCost(ID);
			if (ResourceManager.HasSufficientResource(cost) >= 0)
			{
				return "Insufficient Resource";
			}
			ResourceManager.SpendResource(cost);
			if (currentActiveStat < stats.Count - 1)
			{
				return UpgradeToNextStat();
			}
			if (nextLevelTowerList.Count > 0)
			{
				return UpgradeToNextTower(ID);
			}
			return string.Empty;
		}

		public string UpgradeToNextStat()
		{
			level++;
			currentActiveStat++;
			AddValue(stats[currentActiveStat].cost);
			Build(isUpgrade: true);
			return string.Empty;
		}

		public string UpgradeToNextTower(int ID = 0)
		{
			UnitTower unitTower = nextLevelTowerList[Mathf.Clamp(ID, 0, nextLevelTowerList.Count)];
			GameObject gameObject = Object.Instantiate(unitTower.gameObject, thisT.position, thisT.rotation);
			UnitTower component = gameObject.GetComponent<UnitTower>();
			component.InitTower(instanceID);
			component.SetPlatform(occupiedPlatform, occupiedNode);
			component.AddValue(value);
			component.SetLevel(level + 1);
			component.Build(isUpgrade: true);
			GameControl.SelectTower(component);
			UnityEngine.Object.Destroy(thisObj);
			return string.Empty;
		}

		public List<int> GetCost(int ID = 0)
		{
			List<int> list = new List<int>();
			float num = 1f;
			if (isSampleTower)
			{
				num = GetBuildCostMultiplier();
				list = new List<int>(stats[currentActiveStat].cost);
			}
			else
			{
				num = GetUpgradeCostMultiplier();
				if (currentActiveStat < stats.Count - 1)
				{
					list = new List<int>(stats[currentActiveStat + 1].cost);
				}
				else if (ID < nextLevelTowerList.Count && nextLevelTowerList[ID] != null)
				{
					list = new List<int>(nextLevelTowerList[ID].stats[0].cost);
				}
				else
				{
					UnityEngine.Debug.Log("no next level tower?");
				}
			}
			for (int i = 0; i < list.Count; i++)
			{
				list[i] = (int)Mathf.Round((float)list[i] * num);
			}
			return list;
		}

		private float GetBuildCostMultiplier()
		{
			return 1f - PerkManager.GetTowerBuildCost(prefabID);
		}

		private float GetUpgradeCostMultiplier()
		{
			return 1f - PerkManager.GetTowerUpgradeCost(prefabID);
		}

		public List<int> GetValue()
		{
			List<int> list = new List<int>();
			for (int i = 0; i < value.Count; i++)
			{
				list.Add((int)((float)value[i] * GameControl.GetSellTowerRefundRatio()));
			}
			return list;
		}

		public void AddValue(List<int> list)
		{
			for (int i = 0; i < value.Count; i++)
			{
				List<int> list2;
				int index;
				(list2 = value)[index = i] = list2[index] + list[i];
			}
		}

		public bool DealDamage()
		{
			if (type == _TowerType.Turret || type == _TowerType.AOE || type == _TowerType.Mine)
			{
				return true;
			}
			return false;
		}

		public override void Destroyed(float delay = 0f)
		{
			if (!destroyed)
			{
				destroyed = true;
				TDTK.OnUnitTowerDestroyed(this);
				RemoveFromGame();
			}
		}

		public void RemoveFromGame()
		{
			IndicatorControl.TowerRemoved(this);
			destroyed = true;
			if (occupiedPlatform != null)
			{
				occupiedPlatform.UnbuildTower(occupiedNode);
			}
			base.Destroyed(PlayAnimDestroyed());
		}

		public string GetDespStats()
		{
			if (stats[currentActiveStat].useCustomDesp)
			{
				return stats[currentActiveStat].desp;
			}
			string text = string.Empty;
			if (type == _TowerType.Turret || type == _TowerType.AOE || type == _TowerType.Mine)
			{
				float damageMin = GetDamageMin();
				float damageMax = GetDamageMax();
				if (damageMax > 0f)
				{
					float num = (damageMax + damageMin) / 2f * (1f / GetCooldown());
					int num2 = (int)num;
					if (damageMin == damageMax)
					{
						string text2 = text;
						text = text2 + "Damage:  " + damageMax.ToString("f0") + "\nDamage Per Second:  " + num2 + " dps";
					}
					else
					{
						string text2 = text;
						text = text2 + "Damage:  " + damageMin.ToString("f0") + "-" + damageMax.ToString("f0") + "\nDamage Per Second:  " + num2 + " dps";
					}
				}
				float aOERadius = GetAOERadius();
				if (aOERadius > 0f)
				{
					text += " (AOE)";
				}
				if (type != _TowerType.Mine)
				{
					float cooldown = GetCooldown();
					if (cooldown > 0f)
					{
						text = text + "\nCooldown:\t " + cooldown.ToString("f1") + "s";
					}
				}
				if (text != string.Empty)
				{
					text += "\n";
				}
				if (GetCritChance() > 0f)
				{
					text = text + "\n" + (GetCritChance() * 100f).ToString("f0") + "% Chance to X2 Damage";
				}
				Stun stun = GetStun();
				if (stun.IsValid())
				{
					text = text + "\n" + (stun.chance * 100f).ToString("f0") + "% Chance to stuns target";
				}
				Slow slow = GetSlow();
				if (slow.IsValid())
				{
					string text2 = text;
					text = text2 + "\nSlows target  " + (100f - slow.slowMultiplier * 100f) + "%  for " + slow.duration.ToString("f1") + " seconds";
				}
				Dot dot = GetDot();
				float totalDamage = dot.GetTotalDamage();
				if (totalDamage > 0f)
				{
					string text2 = text;
					text = text2 + "\nDamage target by " + totalDamage.ToString("f0") + " over " + dot.duration.ToString("f0") + "s";
				}
				if (DamageShieldOnly())
				{
					text += "\nDamage target's shield only";
				}
				if (GetShieldBreak() > 0f)
				{
					text = text + "\n" + (GetShieldBreak() * 100f).ToString("f0") + "% Chance to break shield";
				}
				if (GetShieldPierce() > 0f)
				{
					text = text + "\n" + (GetShieldPierce() * 100f).ToString("f0") + "% Chance to pierce shield";
				}
				InstantKill instantKill = GetInstantKill();
				if (instantKill.IsValid())
				{
					text = text + "\n" + (instantKill.chance * 100f).ToString("f0") + "% Chance to kill target instantly";
				}
			}
			else if (type == _TowerType.Support)
			{
				Buff buff = GetBuff();
				if (buff.damageBuff > 0f)
				{
					text = text + "Damage Buff: " + (buff.damageBuff * 100f).ToString("f0") + "%";
				}
				if (buff.cooldownBuff > 0f)
				{
					text = text + "\nCooldown Buff: " + (buff.cooldownBuff * 100f).ToString("f0") + "%";
				}
				if (buff.rangeBuff > 0f)
				{
					text = text + "\nRange Buff: " + (buff.rangeBuff * 100f).ToString("f0") + "%";
				}
				if (buff.criticalBuff > 0f)
				{
					text = text + "\nRange Buff: " + (buff.criticalBuff * 100f).ToString("f0") + "%";
				}
				if (buff.hitBuff > 0f)
				{
					text = text + "\nHit Buff: " + (buff.hitBuff * 100f).ToString("f0") + "%";
				}
				if (buff.dodgeBuff > 0f)
				{
					text = text + "\nDodge Buff: " + (buff.dodgeBuff * 100f).ToString("f0") + "%";
				}
				if (text != string.Empty)
				{
					text += "\n";
				}
				if (buff.regenHP > 0f)
				{
					float num3 = buff.regenHP;
					float num4 = 1f;
					if (buff.regenHP < 1f)
					{
						num3 = 1f;
						num4 = 1f / buff.regenHP;
					}
					string text2 = text;
					text = text2 + "\nRegen " + num3.ToString("f0") + "HP every " + num4.ToString("f0") + "s";
				}
			}
			else if (type == _TowerType.Resource)
			{
				text += "Regenerate resource overtime";
			}
			return text;
		}
	}
}
