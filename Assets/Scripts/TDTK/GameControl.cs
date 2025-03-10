using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TDTK
{
	[RequireComponent(typeof(ResourceManager))]
	[RequireComponent(typeof(DamageTable))]
	public class GameControl : MonoBehaviour
	{
		private bool gameStarted;

		public _GameState gameState;

		public float ffSpeed = 2.5f;

		private bool fastforward;

		public bool playerWon;

		public int levelID = 1;

		public bool capLife;

		public int playerLifeCap;

		public int playerLife = 10;

		public bool enableLifeGen;

		public float lifeRegenRate = 0.1f;

		public float sellTowerRefundRatio = 0.5f;

		public bool resetTargetAfterShoot = true;

		public string nextScene = string.Empty;

		public string mainMenu = string.Empty;

		private float timeStep = 0.015f;

		public static GameControl instance;

		public Transform thisT;

		public UnitTower selectedTower;

		public static void StartGame()
		{
			instance.gameStarted = true;
		}

		public static bool IsGameStarted()
		{
			return instance.gameStarted;
		}

		public static _GameState GetGameState()
		{
			return instance.gameState;
		}

		public static bool IsGameOver()
		{
			return (instance.gameState == _GameState.Over) ? true : false;
		}

		public static bool IsGamePlaying()
		{
			return (instance.gameState == _GameState.Play) ? true : false;
		}

		public static bool IsGamePaused()
		{
			return (instance.gameState == _GameState.Pause) ? true : false;
		}

		public static void FastForwardOn()
		{
			FastForward(flag: true);
		}

		public static void FastForwardOff()
		{
			FastForward(flag: false);
		}

		public static void FastForward(bool flag)
		{
			Time.timeScale = ((!flag) ? 1f : instance.ffSpeed);
			instance.fastforward = flag;
			TDTK.OnFastForward(flag);
		}

		public static bool IsFastForwardOn()
		{
			return instance.fastforward;
		}

		public static bool HasPlayerWon()
		{
			return instance.playerWon;
		}

		public static int GetLevelID()
		{
			return instance.levelID;
		}

		public static int GetPlayerLife()
		{
			return instance.playerLife;
		}

		public static int GetPlayerLifeCap()
		{
			return (!instance.capLife) ? (-1) : (instance.playerLifeCap + PerkManager.GetLifeCapModifier());
		}

		public static bool ResetTargetAfterShoot()
		{
			return instance.resetTargetAfterShoot;
		}

		public static void LoadNextScene()
		{
			Load(instance.nextScene);
		}

		public static void LoadMainMenu()
		{
			Load(instance.mainMenu);
		}

		public static void Load(string levelName)
		{
			if (levelName == string.Empty)
			{
				UnityEngine.Debug.LogWarning("Trying to load unspecificed scene", instance);
				return;
			}
			Time.timeScale = 1f;
			SceneManager.LoadScene(levelName);
		}

		public static void RestartScene()
		{
			ResourceManager.OnRestartLevel();
			Time.timeScale = 1f;
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		}

		private void Awake()
		{
			Time.fixedDeltaTime = timeStep;
			Screen.sleepTimeout = -1;
			instance = this;
			thisT = base.transform;
			ObjectPoolManager.Init();
			NodeGenerator nodeGenerator = (NodeGenerator)UnityEngine.Object.FindObjectOfType(typeof(NodeGenerator));
			if (nodeGenerator != null)
			{
				nodeGenerator.Awake();
			}
			PathFinder pathFinder = (PathFinder)UnityEngine.Object.FindObjectOfType(typeof(PathFinder));
			if (pathFinder != null)
			{
				pathFinder.Awake();
			}
			PathTD[] array = UnityEngine.Object.FindObjectsOfType(typeof(PathTD)) as PathTD[];
			if (array.Length > 0)
			{
				for (int i = 0; i < array.Length; i++)
				{
					array[i].Init();
				}
			}
			TDTK.InitDB();
			ResourceManager resourceManager = (ResourceManager)UnityEngine.Object.FindObjectOfType(typeof(ResourceManager));
			if (resourceManager != null)
			{
				resourceManager.Init();
			}
			PerkManager perkManager = (PerkManager)UnityEngine.Object.FindObjectOfType(typeof(PerkManager));
			if (perkManager != null)
			{
				perkManager.Init();
			}
			BuildManager buildManager = (BuildManager)UnityEngine.Object.FindObjectOfType(typeof(BuildManager));
			if (buildManager != null)
			{
				buildManager.Init();
			}
			AbilityManager abilityManager = (AbilityManager)UnityEngine.Object.FindObjectOfType(typeof(AbilityManager));
			if (abilityManager != null)
			{
				abilityManager.Init();
			}
			FPSControl fPSControl = (FPSControl)UnityEngine.Object.FindObjectOfType(typeof(FPSControl));
			if (fPSControl != null)
			{
				fPSControl.Init();
			}
			IndicatorControl indicatorControl = (IndicatorControl)UnityEngine.Object.FindObjectOfType(typeof(IndicatorControl));
			if (indicatorControl != null)
			{
				indicatorControl.Init();
			}
			Time.timeScale = 1f;
		}

		private void Start()
		{
			UnitTower[] array = UnityEngine.Object.FindObjectsOfType(typeof(UnitTower)) as UnitTower[];
			for (int i = 0; i < array.Length; i++)
			{
				BuildManager.PreBuildTower(array[i]);
			}
			int layerShootObject = TDTK.GetLayerShootObject();
			Physics.IgnoreLayerCollision(layerShootObject, layerShootObject, ignore: true);
			if (capLife)
			{
				playerLife = Mathf.Min(playerLife, GetPlayerLifeCap());
			}
			if (enableLifeGen)
			{
				StartCoroutine(LifeRegenRoutine());
			}
		}

		public static void OnCreepReachDestination(UnitCreep unit)
		{
			instance._OnCreepReachDestination(unit);
		}

		public void _OnCreepReachDestination(UnitCreep unit)
		{
			playerLife = Mathf.Max(0, playerLife - unit.lifeCost);
			TDTK.OnLife(-unit.lifeCost);
			if (playerLife <= 0)
			{
				GameOver();
			}
		}

		public static void GameOver(bool won = false)
		{
			if (instance.gameState != _GameState.Over)
			{
				instance.playerWon = won;
				ResourceManager.OnGameOver(won);
				instance.gameState = _GameState.Over;
				TDTK.OnGameOver(won);
			}
		}

		private IEnumerator LifeRegenRoutine()
		{
			float temp = 0f;
			while (true)
			{
				yield return new WaitForSeconds(1f);
				temp += lifeRegenRate + PerkManager.GetLifeRegenModifier();
				int value = 0;
				while (temp >= 1f)
				{
					value++;
					temp -= 1f;
				}
				if (value > 0)
				{
					_GainLife(value);
				}
			}
		}

		public static void GainLife(int value)
		{
			instance._GainLife(value);
		}

		public void _GainLife(int value)
		{
			playerLife += value;
			if (capLife)
			{
				playerLife = Mathf.Min(playerLife, GetPlayerLifeCap());
			}
			TDTK.OnLife(value);
		}

		public static UnitTower GetSelectedTower()
		{
			return instance.selectedTower;
		}

		public static UnitTower Select(Vector3 pointer)
		{
			LayerMask mask = 1 << TDTK.GetLayerTower();
			Ray ray = Camera.main.ScreenPointToRay(pointer);
			if (!Physics.Raycast(ray, out RaycastHit hitInfo, float.PositiveInfinity, mask))
			{
				return null;
			}
			return hitInfo.transform.GetComponent<UnitTower>();
		}

		public static void ClearSelectedTower()
		{
			SelectTower();
		}

		public static void SelectTower(UnitTower tower = null)
		{
			instance._SelectTower(tower);
		}

		public void _SelectTower(UnitTower tower = null)
		{
			if (tower == null)
			{
				IndicatorControl.ClearRangeIndicator();
			}
			else
			{
				IndicatorControl.ShowTowerRangeIndicator(tower);
			}
		}

		public static void TowerScanAngleChanged(UnitTower tower)
		{
			IndicatorControl.TowerScanAngleChanged(tower);
		}

		public static void PauseGame()
		{
			FastForwardOff();
			instance.gameState = _GameState.Pause;
			Time.timeScale = 0f;
		}

		public static void ResumeGame()
		{
			instance.gameState = _GameState.Play;
			Time.timeScale = 1f;
		}

		public static float GetSellTowerRefundRatio()
		{
			return instance.sellTowerRefundRatio;
		}
	}
}
