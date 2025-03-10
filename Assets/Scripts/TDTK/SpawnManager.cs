using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TDTK
{
	public class SpawnManager : MonoBehaviour
	{
		public enum _SpawnMode
		{
			Continous,
			WaveCleared,
			Round
		}

		public enum _SpawnLimit
		{
			Finite,
			Infinite
		}

		public _SpawnMode spawnMode;

		public _SpawnLimit spawnLimit;

		public bool allowSkip;

		public bool autoStart;

		public float autoStartDelay = 5f;

		public bool procedurallyGenerateWave;

		public PathTD defaultPath;

		private int currentWaveID = -1;

		public bool spawning;

		public int activeUnitCount;

		public int totalSpawnCount;

		public int waveClearedCount;

		public List<Wave> waveList = new List<Wave>();

		public WaveGenerator waveGenerator;

		public static SpawnManager instance;

		private float spawnCD;

		public static bool AutoStart()
		{
			return !(instance == null) && instance.autoStart;
		}

		public static float GetAutoStartDelay()
		{
			return instance.autoStartDelay;
		}

		private void Awake()
		{
			instance = this;
		}

		private void Start()
		{
			if (defaultPath == null)
			{
				UnityEngine.Debug.Log("DefaultPath on SpawnManager not assigned, auto search for one");
				defaultPath = (PathTD)UnityEngine.Object.FindObjectOfType(typeof(PathTD));
			}
			if (spawnLimit == _SpawnLimit.Infinite || procedurallyGenerateWave)
			{
				waveGenerator.Init();
				if (defaultPath != null && waveGenerator.pathList.Count == 0)
				{
					waveGenerator.pathList.Add(defaultPath);
				}
			}
			if (spawnLimit == _SpawnLimit.Finite && procedurallyGenerateWave)
			{
				for (int i = 0; i < waveList.Count; i++)
				{
					waveList[i] = waveGenerator.Generate(i);
				}
			}
			if (spawnLimit == _SpawnLimit.Infinite)
			{
				waveList = new List<Wave>();
			}
			if (spawnLimit == _SpawnLimit.Finite)
			{
				for (int j = 0; j < waveList.Count; j++)
				{
					waveList[j].waveID = j;
				}
			}
			if (autoStart)
			{
				StartCoroutine(AutoStartRoutine());
			}
		}

		private IEnumerator AutoStartRoutine()
		{
			while (autoStartDelay > 0f)
			{
				if (IsSpawningStarted())
				{
					yield break;
				}
				autoStartDelay -= Time.deltaTime;
				spawnCD = autoStartDelay;
				yield return null;
			}
			if (!IsSpawningStarted())
			{
				_Spawn();
			}
		}

		public static void OnUnitDestroyed(UnitCreep unit)
		{
			instance.OnUnitCleared(unit);
		}

		public static void OnCreepReachDestination(UnitCreep creep)
		{
			if (!creep.IsDestroyed())
			{
				instance.OnUnitCleared(creep);
			}
		}

		private void OnUnitCleared(UnitCreep creep)
		{
			if (GameControl.IsGameOver())
			{
				return;
			}
			int waveID = creep.waveID;
			activeUnitCount--;
			Wave wave = null;
			if (spawnLimit == _SpawnLimit.Finite)
			{
				wave = waveList[waveID];
			}
			else if (spawnLimit == _SpawnLimit.Infinite)
			{
				for (int i = 0; i < waveList.Count; i++)
				{
					if (waveList[i].waveID == waveID)
					{
						wave = waveList[i];
						break;
					}
				}
				if (wave == null)
				{
					UnityEngine.Debug.Log("error!");
					return;
				}
			}
			wave.activeUnitCount--;
			if (wave.spawned && wave.activeUnitCount == 0)
			{
				wave.cleared = true;
				waveClearedCount++;
				TDTK.OnWaveCleared(waveID);
				UnityEngine.Debug.Log("wave" + (waveID + 1) + " is cleared");
				ResourceManager.GainResource(wave.rscGainList, PerkManager.GetRscWaveCleared());
				GameControl.GainLife(wave.lifeGain + PerkManager.GetLifeWaveClearedModifier());
				AbilityManager.GainEnergy(wave.energyGain + (int)PerkManager.GetEnergyWaveClearedModifier());
				if (spawnLimit == _SpawnLimit.Infinite)
				{
					waveList.Remove(wave);
				}
				if (IsAllWaveCleared())
				{
					GameControl.GameOver(won: true);
				}
				else if (spawnMode == _SpawnMode.Round)
				{
					TDTK.OnEnableSpawn();
				}
			}
			if (!IsAllWaveCleared() && activeUnitCount == 0 && !spawning && spawnMode == _SpawnMode.WaveCleared)
			{
				SpawnWaveFinite();
			}
		}

		public static int AddDestroyedSpawn(UnitCreep unit)
		{
			return instance._AddDestroyedSpawn(unit);
		}

		public int _AddDestroyedSpawn(UnitCreep unit)
		{
			activeUnitCount++;
			if (spawnLimit == _SpawnLimit.Finite)
			{
				waveList[unit.waveID].activeUnitCount++;
			}
			else if (spawnLimit == _SpawnLimit.Infinite)
			{
				for (int i = 0; i < waveList.Count; i++)
				{
					if (waveList[i].waveID == unit.waveID)
					{
						waveList[i].activeUnitCount++;
						break;
					}
				}
			}
			return ++totalSpawnCount;
		}

		public static void Spawn()
		{
			instance._Spawn();
		}

		public void _Spawn()
		{
			if (GameControl.IsGameOver())
			{
				return;
			}
			if (IsSpawningStarted())
			{
				if (spawnMode == _SpawnMode.Round)
				{
					if (spawnLimit == _SpawnLimit.Infinite)
					{
						if (waveList.Count != 0)
						{
							return;
						}
					}
					else if (!waveList[currentWaveID].cleared)
					{
						return;
					}
				}
				else if (!allowSkip)
				{
					return;
				}
				spawnCD = SpawnWaveFinite();
			}
			else
			{
				if (spawnMode != 0)
				{
					SpawnWaveFinite();
				}
				else
				{
					StartCoroutine(ContinousSpawnRoutine());
				}
				GameControl.StartGame();
			}
		}

		private IEnumerator ContinousSpawnRoutine()
		{
			while (!GameControl.IsGameOver())
			{
				spawnCD = SpawnWaveFinite();
				if (spawnLimit == _SpawnLimit.Finite && currentWaveID >= waveList.Count)
				{
					break;
				}
				yield return null;
				while (spawnCD > 0f)
				{
					spawnCD -= Time.deltaTime;
					yield return null;
				}
			}
		}

		private float SpawnWaveFinite()
		{
			if (spawning)
			{
				return 0f;
			}
			spawning = true;
			currentWaveID++;
			if (spawnLimit == _SpawnLimit.Finite && currentWaveID >= waveList.Count)
			{
				return 0f;
			}
			UnityEngine.Debug.Log("spawning wave" + (currentWaveID + 1));
			TDTK.OnNewWave(currentWaveID + 1);
			Wave wave = null;
			if (spawnLimit == _SpawnLimit.Finite)
			{
				wave = waveList[currentWaveID];
			}
			else if (spawnLimit == _SpawnLimit.Infinite)
			{
				wave = waveGenerator.Generate(currentWaveID);
				wave.waveID = currentWaveID;
				waveList.Add(wave);
			}
			if (spawnMode == _SpawnMode.Continous && ((spawnLimit == _SpawnLimit.Finite && currentWaveID < waveList.Count - 1) || spawnLimit == _SpawnLimit.Infinite))
			{
				TDTK.OnSpawnTimer(wave.duration);
			}
			for (int i = 0; i < wave.subWaveList.Count; i++)
			{
				StartCoroutine(SpawnSubWave(wave.subWaveList[i], wave));
			}
			return wave.duration;
		}

		private IEnumerator SpawnSubWave(SubWave subWave, Wave parentWave)
		{
			if (subWave.unit == null)
			{
				UnityEngine.Debug.LogWarning("No creep prefab has been assigned to sub-wave", this);
				yield break;
			}
			yield return new WaitForSeconds(subWave.delay);
			PathTD path = defaultPath;
			if (subWave.path != null)
			{
				path = subWave.path;
			}
			Vector3 pos = path.GetSpawnPoint();
			Quaternion rot = path.GetSpawnDirection();
			int spawnCount = 0;
			if (subWave.unitC == null)
			{
				subWave.unitC = subWave.unit.GetComponent<UnitCreep>();
			}
			while (spawnCount < subWave.count)
			{
				if (subWave.unit == null)
				{
					UnityEngine.Debug.LogWarning("no creep has been assigned to subwave", this);
					break;
				}
				GameObject obj = ObjectPoolManager.Spawn(subWave.unit, pos, rot);
				UnitCreep unit = obj.GetComponent<UnitCreep>();
				if (subWave.overrideHP > 0f)
				{
					unit.defaultHP = subWave.overrideHP;
				}
				else
				{
					unit.defaultHP = subWave.unitC.defaultHP;
				}
				if (subWave.overrideShield > 0f)
				{
					unit.defaultShield = subWave.overrideShield;
				}
				else
				{
					unit.defaultShield = subWave.unitC.defaultShield;
				}
				if (subWave.overrideMoveSpd > 0f)
				{
					unit.moveSpeed = subWave.overrideMoveSpd;
				}
				else
				{
					unit.moveSpeed = subWave.unitC.moveSpeed;
				}
				unit.Init(path, totalSpawnCount, parentWave.waveID);
				UnityEngine.Debug.Log(unit.name + "-" + path + "-" + totalSpawnCount + "-" + parentWave.waveID);
				totalSpawnCount++;
				activeUnitCount++;
				parentWave.activeUnitCount++;
				spawnCount++;
				if (spawnCount == subWave.count)
				{
					break;
				}
				yield return new WaitForSeconds(subWave.interval);
			}
			parentWave.subWaveSpawnedCount++;
			if (parentWave.subWaveSpawnedCount == parentWave.subWaveList.Count)
			{
				parentWave.spawned = true;
				spawning = false;
				UnityEngine.Debug.Log("wave " + (parentWave.waveID + 1) + " has done spawning");
				yield return new WaitForSeconds(0.5f);
				if ((currentWaveID <= waveList.Count - 2 || spawnLimit == _SpawnLimit.Infinite) && (spawnMode == _SpawnMode.WaveCleared || spawnMode == _SpawnMode.Continous) && allowSkip)
				{
					TDTK.OnEnableSpawn();
				}
			}
		}

		public bool IsSpawningStarted()
		{
			return (currentWaveID >= 0) ? true : false;
		}

		public static bool IsAllWaveCleared()
		{
			if (instance.spawnLimit == _SpawnLimit.Infinite)
			{
				return false;
			}
			if (instance.waveClearedCount >= instance.waveList.Count)
			{
				return true;
			}
			return false;
		}

		public static int GetTotalWaveCount()
		{
			if (instance == null || instance.spawnLimit == _SpawnLimit.Infinite)
			{
				return -1;
			}
			return instance.waveList.Count;
		}

		public static float GetTimeToNextSpawn()
		{
			return instance._GetTimeToNextSpawn();
		}

		public float _GetTimeToNextSpawn()
		{
			if (spawnMode == _SpawnMode.Round || spawnMode == _SpawnMode.WaveCleared)
			{
				return -1f;
			}
			if (spawnLimit == _SpawnLimit.Finite && currentWaveID >= waveList.Count - 1)
			{
				return -1f;
			}
			return spawnCD;
		}

		public static int GetCurrentWaveID()
		{
			return (!(instance == null)) ? instance.currentWaveID : 0;
		}

		public static int GetActiveUnitCount()
		{
			return instance.activeUnitCount;
		}
	}
}
