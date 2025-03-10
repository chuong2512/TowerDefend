using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TDTK
{
	public class AbilityManager : MonoBehaviour
	{
		public List<int> unavailableIDList = new List<int>();

		[HideInInspector]
		public List<int> availableIDList = new List<int>();

		public List<Ability> abilityList = new List<Ability>();

		public Transform defaultIndicator;

		private bool isSelectingTarget;

		public Transform currentIndicator;

		public bool startWithFullEnergy;

		public bool onlyChargeOnSpawn;

		public float energyRate = 2f;

		public float fullEnergy = 100f;

		public float energy;

		private Transform thisT;

		private static AbilityManager instance;

		private LayerMask maskAOE;

		public static List<Ability> GetAbilityList()
		{
			return instance.abilityList;
		}

		public static int GetAbilityCount()
		{
			return instance.abilityList.Count;
		}

		public static bool IsSelectingTarget()
		{
			return !(instance == null) && instance.isSelectingTarget;
		}

		public static AbilityManager GetInstance()
		{
			return instance;
		}

		public static bool IsOn()
		{
			return (!(instance == null)) ? true : false;
		}

		public void Init()
		{
			instance = this;
			thisT = base.transform;
			if (startWithFullEnergy)
			{
				energy = fullEnergy;
			}
			List<Ability> abilityDBList = TDTK.GetAbilityDBList();
			availableIDList = new List<int>();
			abilityList = new List<Ability>();
			for (int i = 0; i < abilityDBList.Count; i++)
			{
				if (!abilityDBList[i].disableInAbilityManager && !unavailableIDList.Contains(abilityDBList[i].ID))
				{
					abilityList.Add(abilityDBList[i].Clone());
					availableIDList.Add(abilityDBList[i].ID);
				}
			}
			List<Ability> unlockedAbilityList = PerkManager.GetUnlockedAbilityList();
			for (int j = 0; j < unlockedAbilityList.Count; j++)
			{
				abilityList.Add(unlockedAbilityList[j].Clone());
			}
			for (int k = 0; k < abilityList.Count; k++)
			{
				abilityList[k].Init();
			}
			if (defaultIndicator != null)
			{
				defaultIndicator = UnityEngine.Object.Instantiate(defaultIndicator);
				defaultIndicator.parent = thisT;
				defaultIndicator.gameObject.SetActive(value: false);
			}
			maskAOE = 1 << TDTK.GetLayerPlatform();
			int layerTerrain = TDTK.GetLayerTerrain();
			if (layerTerrain >= 0)
			{
				maskAOE = ((int)maskAOE | (1 << layerTerrain));
			}
		}

		public static void AddNewAbility(Ability ab)
		{
			if (instance != null)
			{
				instance._AddNewAbility(ab);
			}
		}

		public void _AddNewAbility(Ability ab)
		{
			for (int i = 0; i < abilityList.Count; i++)
			{
				if (ab.ID == abilityList[i].ID)
				{
					return;
				}
			}
			Ability ability = ab.Clone();
			ability.Init();
			abilityList.Add(ability);
			TDTK.OnNewAbility(ability);
		}

		private void FixedUpdate()
		{
			if (((onlyChargeOnSpawn && GameControl.IsGameStarted()) || !onlyChargeOnSpawn) && energy < GetEnergyFull())
			{
				float num = Time.fixedDeltaTime * GetEnergyRate();
				energy += num;
				energy = Mathf.Min(energy, GetEnergyFull());
				if (energy == GetEnergyFull())
				{
					TDTK.OnEnergyFull();
				}
			}
		}

		private IEnumerator SelectAbilityTargetRoutine(Ability ability, int pointerID = -1)
		{
			yield return null;
			Vector3 zero = Vector3.zero;
			Unit targetUnit = null;
			LayerMask mask = maskAOE;
			if (ability.singleUnitTargeting)
			{
				if (ability.targetType == Ability._TargetType.Hybrid)
				{
					mask = ((int)mask | ((1 << TDTK.GetLayerTower()) | (1 << TDTK.GetLayerCreep())));
				}
				else if (ability.targetType == Ability._TargetType.Friendly)
				{
					mask = ((int)mask | (1 << TDTK.GetLayerTower()));
				}
				else if (ability.targetType == Ability._TargetType.Hostile)
				{
					mask = ((int)mask | (1 << TDTK.GetLayerCreep()));
				}
			}
			Transform indicator = ability.indicator;
			if (indicator == null)
			{
				indicator = defaultIndicator;
				float num = (!ability.singleUnitTargeting) ? (ability.GetAOERadius() * 2f) : BuildManager.GetGridSize();
				indicator.localScale = new Vector3(num, num, num);
			}
			isSelectingTarget = true;
			TDTK.OnAbilityTargetSelectModeE(flag: true);
			if (pointerID >= 0)
			{
				while (!TDTK.IsTouchStarting(pointerID))
				{
					yield return null;
				}
			}
			bool cursorOnUI = true;
			while (isSelectingTarget && !Input.GetKeyDown(KeyCode.Escape))
			{
				bool invalidCursor = false;
				bool invalidTarget = false;
				Vector3 cursorPos = (pointerID >= 0) ? TDTK.GetTouchPosition(pointerID) : UnityEngine.Input.mousePosition;
				if (cursorPos.magnitude < 0f)
				{
					invalidCursor = true;
				}
				if (!invalidCursor && !cursorOnUI)
				{
					Ray ray = Camera.main.ScreenPointToRay(cursorPos);
					if (Physics.Raycast(ray, out RaycastHit hitInfo, float.PositiveInfinity, mask))
					{
						indicator.position = hitInfo.point;
						targetUnit = null;
						if (ability.singleUnitTargeting)
						{
							targetUnit = hitInfo.transform.GetComponent<Unit>();
							if (targetUnit != null)
							{
								indicator.position = targetUnit.thisT.position;
							}
							else
							{
								invalidTarget = true;
							}
						}
					}
				}
				indicator.gameObject.SetActive(!invalidCursor);
				if (pointerID == -1)
				{
					if (Input.GetMouseButtonDown(0))
					{
						if (!cursorOnUI)
						{
							if (!invalidTarget)
							{
								ActivateAbility(ability, indicator.position, targetUnit);
							}
							else
							{
								TDTK.OnGameMessage("Invalid target for ability");
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
						if (!invalidTarget)
						{
							ActivateAbility(ability, indicator.position, targetUnit);
						}
						else
						{
							TDTK.OnGameMessage("Invalid target for ability");
						}
					}
					break;
				}
				cursorOnUI = UI.IsCursorOnUI(pointerID);
				yield return null;
			}
			yield return null;
			indicator.gameObject.SetActive(value: false);
			isSelectingTarget = false;
			TDTK.OnAbilityTargetSelectModeE(flag: false);
		}

		public static void ExitSelectingTargetMode()
		{
			instance.isSelectingTarget = false;
		}

		public static string SelectAbility(int ID, int pointerID = -1)
		{
			return instance._SelectAbility(ID, pointerID);
		}

		public string _SelectAbility(int ID, int pointerID = -1)
		{
			Ability ability = abilityList[ID];
			UnityEngine.Debug.Log(ability.name + "   " + ability.requireTargetSelection);
			string text = ability.IsAvailable();
			if (text != string.Empty)
			{
				return text;
			}
			if (!ability.requireTargetSelection)
			{
				ActivateAbility(ability);
			}
			else if (UnityEngine.Input.touchCount == 0)
			{
				StartCoroutine(SelectAbilityTargetRoutine(ability));
			}
			else
			{
				StartCoroutine(SelectAbilityTargetRoutine(ability, 0));
			}
			return string.Empty;
		}

		public void ActivateAbility(Ability ab, Vector3 pos = default(Vector3), Unit unit = null)
		{
			energy -= ab.GetCost();
			ab.Activate(pos);
			if (ab.useDefaultEffect)
			{
				StartCoroutine(ApplyAbilityEffect(ab, pos, unit));
			}
			TDTK.OnAbilityActivated(ab);
		}

		private IEnumerator ApplyAbilityEffect(Ability ab, Vector3 pos, Unit tgtUnit = null)
		{
			yield return new WaitForSeconds(ab.effectDelay);
			List<Unit> creepList = new List<Unit>();
			List<Unit> towerList = new List<Unit>();
			if (tgtUnit == null)
			{
				LayerMask mask = (1 << TDTK.GetLayerTower()) | (1 << TDTK.GetLayerCreep()) | (1 << TDTK.GetLayerCreepF());
				float radius = (!ab.requireTargetSelection) ? float.PositiveInfinity : ab.GetAOERadius();
				Collider[] array = Physics.OverlapSphere(pos, radius, mask);
				if (array.Length > 0)
				{
					for (int i = 0; i < array.Length; i++)
					{
						int layer = array[i].gameObject.layer;
						if (layer == TDTK.GetLayerCreep() || layer == TDTK.GetLayerCreepF())
						{
							creepList.Add(array[i].gameObject.GetComponent<UnitCreep>());
						}
						if (layer == TDTK.GetLayerTower())
						{
							towerList.Add(array[i].gameObject.GetComponent<UnitTower>());
						}
					}
				}
			}
			else
			{
				creepList.Add(tgtUnit);
				towerList.Add(tgtUnit);
			}
			AbilityEffect eff = ab.GetActiveEffect();
			for (int j = 0; j < creepList.Count; j++)
			{
				if (eff.damageMax > 0f)
				{
					creepList[j].ApplyDamage(UnityEngine.Random.Range(eff.damageMin, eff.damageMax));
				}
				if (eff.stunChance > 0f && eff.duration > 0f && UnityEngine.Random.Range(0f, 1f) < eff.stunChance)
				{
					creepList[j].ApplyStun(eff.duration);
				}
				if (eff.slow.IsValid())
				{
					creepList[j].ApplySlow(eff.slow);
				}
				if (eff.dot.GetTotalDamage() > 0f)
				{
					creepList[j].ApplyDot(eff.dot);
				}
			}
			for (int k = 0; k < towerList.Count; k++)
			{
				if (eff.duration > 0f)
				{
					if (eff.damageBuff > 0f)
					{
						towerList[k].ABBuffDamage(eff.damageBuff, eff.duration);
					}
					if (eff.rangeBuff > 0f)
					{
						towerList[k].ABBuffRange(eff.rangeBuff, eff.duration);
					}
					if (eff.cooldownBuff > 0f)
					{
						towerList[k].ABBuffCooldown(eff.cooldownBuff, eff.duration);
					}
				}
				if (eff.HPGainMax > 0f)
				{
					towerList[k].RestoreHP(UnityEngine.Random.Range(eff.HPGainMin, eff.HPGainMax));
				}
			}
		}

		public static void GainEnergy(int value)
		{
			if (instance != null)
			{
				instance._GainEnergy(value);
			}
		}

		public void _GainEnergy(int value)
		{
			energy += value;
			energy = Mathf.Min(energy, GetEnergyFull());
		}

		public static float GetAbilityCurrentCD(int index)
		{
			return instance.abilityList[index].currentCD;
		}

		public static float GetEnergyFull()
		{
			return instance.fullEnergy + PerkManager.GetEnergyCapModifier();
		}

		public static float GetEnergy()
		{
			return instance.energy;
		}

		private float GetEnergyRate()
		{
			return energyRate + PerkManager.GetEnergyRegenModifier();
		}

		public static int GetAbilityIndex(Ability ability)
		{
			for (int i = 0; i < instance.abilityList.Count; i++)
			{
				if (ability == instance.abilityList[i])
				{
					return i;
				}
			}
			return -1;
		}
	}
}
