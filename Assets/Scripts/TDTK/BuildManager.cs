using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace TDTK
{
	public class BuildManager : MonoBehaviour
	{
		public delegate void NewBuild();

		public _BuildMode buildMode;

		public float gridSize = 1.5f;

		[Tooltip("When checked, player cannot build tower when there are active creep in the scene")]
		public bool disableBuildWhenInPlay;

		public bool autoAdjustTextureToGrid = true;

		private List<PlatformTD> buildPlatforms = new List<PlatformTD>();

		public List<int> unavailableTowerIDList = new List<int>();

		[HideInInspector]
		public List<int> availableTowerIDList = new List<int>();

		private List<UnitTower> towerList = new List<UnitTower>();

		private int towerCount;

		private static BuildManager instance;

		public LayerMask maskPlatform;

		public LayerMask maskAll;

		public LayerMask maskIndicator;

		private List<UnitTower> sampleTowerList = new List<UnitTower>();

		private int currentSampleID = -1;

		public static event NewBuild OnbuildTower;

		public static bool UseDragNDrop()
		{
			return (instance.buildMode != 0) ? true : false;
		}

		public static float GetGridSize()
		{
			return instance.gridSize;
		}

		public static int GetTowerCount()
		{
			return instance.towerCount;
		}

		public static BuildManager GetInstance()
		{
			return instance;
		}

		public void Init()
		{
			instance = this;
			gridSize = Mathf.Max(0.25f, gridSize);
			InitTower();
			InitPlatform();
		}

		private void Start()
		{
			SetupLayerMask();
			InitiateSampleTowerList();
		}

		public void InitTower()
		{
			List<UnitTower> towerDBList = TDTK.GetTowerDBList();
			availableTowerIDList = new List<int>();
			towerList = new List<UnitTower>();
			for (int i = 0; i < towerDBList.Count; i++)
			{
				if (!(towerDBList[i] == null) && !towerDBList[i].disableInBuildManager && !unavailableTowerIDList.Contains(towerDBList[i].prefabID))
				{
					towerList.Add(towerDBList[i]);
					availableTowerIDList.Add(towerDBList[i].prefabID);
				}
			}
			List<UnitTower> unlockedTowerList = PerkManager.GetUnlockedTowerList();
			for (int j = 0; j < unlockedTowerList.Count; j++)
			{
				towerList.Add(unlockedTowerList[j]);
			}
		}

		private void InitPlatform()
		{
			buildPlatforms = new List<PlatformTD>();
			PlatformTD[] array = UnityEngine.Object.FindObjectsOfType(typeof(PlatformTD)) as PlatformTD[];
			for (int i = 0; i < array.Length; i++)
			{
				buildPlatforms.Add(array[i]);
			}
			for (int j = 0; j < buildPlatforms.Count; j++)
			{
				buildPlatforms[j].Init(gridSize, autoAdjustTextureToGrid, towerList);
			}
		}

		private void SetupLayerMask()
		{
			maskPlatform = 1 << TDTK.GetLayerPlatform();
			maskAll = 1 << TDTK.GetLayerPlatform();
			int layerTerrain = TDTK.GetLayerTerrain();
			if (layerTerrain >= 0)
			{
				maskAll = ((int)maskAll | (1 << layerTerrain));
			}
			maskIndicator = ((1 << TDTK.GetLayerPlatform()) | (1 << TDTK.GetLayerTerrain()) | (1 << TDTK.GetLayerTower()));
			maskIndicator = ((int)maskIndicator | ((1 << TDTK.GetLayerCreep()) | (1 << TDTK.GetLayerCreepF()) | (1 << TDTK.GetLayerShootObject())));
		}

		public static void AddNewTower(UnitTower newTower)
		{
			if (instance != null)
			{
				instance._AddNewTower(newTower);
			}
		}

		public void _AddNewTower(UnitTower newTower)
		{
			if (!towerList.Contains(newTower))
			{
				towerList.Add(newTower);
				availableTowerIDList.Add(newTower.prefabID);
				AddNewSampleTower(newTower);
				for (int i = 0; i < buildPlatforms.Count; i++)
				{
					buildPlatforms[i].availableTowerIDList.Add(newTower.prefabID);
				}
				TDTK.OnNewBuildableTower(newTower);
			}
		}

		public static void SetTileIndicator(Vector3 cursor)
		{
			instance._SetTileIndicator(cursor);
		}

		public void _SetTileIndicator(Vector3 cursor)
		{
			Ray ray = Camera.main.ScreenPointToRay(cursor);
			if (Physics.Raycast(ray, out RaycastHit hitInfo, float.PositiveInfinity, maskPlatform))
			{
				for (int i = 0; i < buildPlatforms.Count; i++)
				{
					if (!(hitInfo.transform != buildPlatforms[i].thisT))
					{
						Vector3 tilePos = GetTilePos(buildPlatforms[i], hitInfo.point);
						Collider[] array = Physics.OverlapSphere(tilePos, gridSize / 2f * 0.9f, ~(int)maskIndicator);
						if (array.Length == 0)
						{
							IndicatorControl.SetIndicatorCursor(tilePos, buildPlatforms[i].thisT.rotation);
							return;
						}
						break;
					}
				}
			}
			IndicatorControl.ClearIndicatorCursor();
		}

		public static Vector3 GetTilePos(PlatformTD platform, Vector3 hitPos)
		{
			return instance._GetTilePos(platform, hitPos);
		}

		public Vector3 _GetTilePos(PlatformTD platform, Vector3 hitPos)
		{
			Vector3 point = hitPos - platform.thisT.position;
			Vector3 eulerAngles = platform.thisT.rotation.eulerAngles;
			point = Quaternion.Euler(0f, 0f - eulerAngles.y, 0f) * point;
			float num = (platform.size.x % 2f != 0f) ? 0f : (gridSize / 2f);
			float num2 = (platform.size.y % 2f != 0f) ? 0f : (gridSize / 2f);
			float x = Mathf.Round((num + point.x) / gridSize) * gridSize - num;
			float z = Mathf.Round((num2 + point.z) / gridSize) * gridSize - num2;
			return platform.thisT.position + platform.thisT.TransformDirection(new Vector3(x, 0f, z));
		}

		public static BuildInfo CheckBuildPoint(Vector3 pointer, int towerID = -1)
		{
			return instance._CheckBuildPoint(pointer, towerID);
		}

		public BuildInfo _CheckBuildPoint(Vector3 pointer, int towerID)
		{
			BuildInfo buildInfo = new BuildInfo();
			if (disableBuildWhenInPlay && SpawnManager.GetActiveUnitCount() > 0)
			{
				buildInfo.status = _TileStatus.NotInBuildPhase;
				return buildInfo;
			}
			Camera main = Camera.main;
			if (main != null)
			{
				Ray ray = main.ScreenPointToRay(pointer);
				if (Physics.Raycast(ray, out RaycastHit hitInfo, float.PositiveInfinity, maskPlatform))
				{
					for (int i = 0; i < buildPlatforms.Count; i++)
					{
						if (!(hitInfo.transform == buildPlatforms[i].thisT))
						{
							continue;
						}
						PlatformTD platformTD = buildPlatforms[i];
						Vector3 vector = buildInfo.position = GetTilePos(platformTD, hitInfo.point);
						buildInfo.platform = platformTD;
						if (towerID >= 0 && !platformTD.availableTowerIDList.Contains(towerID))
						{
							buildInfo.status = _TileStatus.Unavailable;
						}
						if (buildInfo.status != _TileStatus.Available)
						{
							break;
						}
						Collider[] array = Physics.OverlapSphere(vector, gridSize / 2f * 0.9f, ~(int)maskAll);
						if (array.Length > 0)
						{
							buildInfo.status = _TileStatus.NoPlatform;
						}
						else
						{
							buildInfo.status = _TileStatus.Available;
						}
						if (buildInfo.status != _TileStatus.Available)
						{
							break;
						}
						if (platformTD.IsWalkable() && platformTD.CheckForBlock(vector))
						{
							buildInfo.status = _TileStatus.Blocked;
						}
						buildInfo.availableTowerIDList = new List<int>();
						for (int j = 0; j < platformTD.availableTowerIDList.Count; j++)
						{
							for (int k = 0; k < towerList.Count; k++)
							{
								if (platformTD.availableTowerIDList[j] == towerList[k].prefabID)
								{
									buildInfo.availableTowerIDList.Add(k);
									break;
								}
							}
						}
						break;
					}
				}
				else
				{
					buildInfo.status = _TileStatus.NoPlatform;
				}
			}
			else
			{
				buildInfo.status = _TileStatus.NoPlatform;
			}
			if (buildInfo.status == _TileStatus.Blocked)
			{
				if (towerID >= 0 && GetTowerPrefab(towerID).type == _TowerType.Mine)
				{
					buildInfo.status = _TileStatus.Available;
				}
				if (towerID < 0)
				{
					bool flag = false;
					for (int l = 0; l < buildInfo.availableTowerIDList.Count; l++)
					{
						if (towerList[buildInfo.availableTowerIDList[l]].type == _TowerType.Mine)
						{
							flag = true;
							continue;
						}
						buildInfo.availableTowerIDList.RemoveAt(l);
						l--;
					}
					if (flag)
					{
						buildInfo.status = _TileStatus.Available;
					}
				}
			}
			if (!UseDragNDrop())
			{
				if (buildInfo.status != _TileStatus.Available)
				{
					IndicatorControl.ClearBuildTileIndicator();
				}
				else
				{
					IndicatorControl.SetBuildTileIndicator(buildInfo);
				}
			}
			return buildInfo;
		}

		public string StartDragNDrop(int ID, int pointerID = -1)
		{
			UnitTower sampleTower = GetSampleTower(ID);
			if (sampleTower.type == _TowerType.Resource && !GameControl.IsGameStarted())
			{
				return "Cant Build Tower before spawn start";
			}
			IndicatorControl.SetDragNDropPhase(flag: true);
			List<int> cost = sampleTower.GetCost();
			int num = ResourceManager.HasSufficientResource(cost);
			if (num == -1)
			{
				sampleTower.thisT.position = new Vector3(9999f, 9999f, 9999f);
				sampleTower.thisObj.SetActive(value: true);
				UnitTower unitTower = sampleTower;
				unitTower.StartCoroutine(unitTower.DragNDropRoutine(pointerID));
				return string.Empty;
			}
			return "Insufficient Resource   " + num;
		}

		public static bool InDragNDrop()
		{
			return UseDragNDrop() && UnitTower.InDragNDrop();
		}

		public static void ExitDragNDrop()
		{
			UnitTower.InDragNDrop();
		}

		public static string BuildTower(int ID, BuildInfo bInfo, int pointerID = -1)
		{
			if (UseDragNDrop())
			{
				return instance.StartDragNDrop(ID, pointerID);
			}
			ClearSampleTower();
			return _BuildTower(instance.towerList[ID], bInfo);
		}

		public static string _BuildTower(UnitTower tower, BuildInfo bInfo)
		{
			if (bInfo == null)
			{
				if (!UseDragNDrop())
				{
					return "Select a Build Point First";
				}
				return "Invalid build position";
			}
			if (bInfo.status != _TileStatus.Available)
			{
				return "Invalid build position";
			}
			if (tower.type == _TowerType.Resource && !GameControl.IsGameStarted())
			{
				return "Cant Build Tower before spawn start";
			}
			UnitTower sampleTower = GetSampleTower(tower);
			List<int> cost = sampleTower.GetCost();
			int num = ResourceManager.HasSufficientResource(cost);
			if (num == -1)
			{
				ResourceManager.SpendResource(cost);
				GameObject gameObject = UnityEngine.Object.Instantiate(tower.gameObject, bInfo.position, bInfo.platform.thisT.rotation);
				UnitTower component = gameObject.GetComponent<UnitTower>();
				component.InitTower(++instance.towerCount);
				component.Build();
				if (BuildManager.OnbuildTower != null)
				{
					BuildManager.OnbuildTower();
				}
				if (bInfo.platform != null)
				{
					bInfo.platform.BuildTower(bInfo.position, component);
				}
				IndicatorControl.ClearBuildTileIndicator();
				return string.Empty;
			}
			return "Insufficient Resource";
		}

		public static void PreBuildTower(UnitTower tower)
		{
			PlatformTD platformTD = null;
			LayerMask mask = 1 << TDTK.GetLayerPlatform();
			Collider[] array = Physics.OverlapSphere(tower.thisT.position, GetGridSize(), mask);
			if (array.Length > 0)
			{
				platformTD = array[0].gameObject.GetComponent<PlatformTD>();
			}
			if (platformTD != null)
			{
				Vector3 tilePos = GetTilePos(platformTD, tower.thisT.position);
				tower.thisT.position = tilePos;
				tower.thisT.rotation = platformTD.thisT.rotation;
				platformTD.BuildTower(tilePos, tower);
			}
			else
			{
				UnityEngine.Debug.Log("no platform found for pre-placed tower");
			}
			tower.InitTower(++instance.towerCount);
		}

		public void InitiateSampleTowerList()
		{
			sampleTowerList = new List<UnitTower>();
			for (int i = 0; i < towerList.Count; i++)
			{
				UnitTower item = CreateSampleTower(towerList[i]);
				sampleTowerList.Add(item);
			}
		}

		public void AddNewSampleTower(UnitTower newTower)
		{
			UnitTower item = CreateSampleTower(newTower);
			sampleTowerList.Add(item);
		}

		public UnitTower CreateSampleTower(UnitTower towerPrefab)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(towerPrefab.gameObject);
			gameObject.transform.parent = base.transform;
			if (gameObject.GetComponent<Collider>() != null)
			{
				UnityEngine.Object.Destroy(gameObject.GetComponent<Collider>());
			}
			Utility.DestroyColliderRecursively(gameObject.transform);
			gameObject.SetActive(value: false);
			UnitTower component = gameObject.GetComponent<UnitTower>();
			component.SetAsSampleTower(towerPrefab);
			return component;
		}

		public static UnitTower GetSampleTower(int ID)
		{
			return instance.sampleTowerList[ID];
		}

		public static UnitTower GetSampleTower(UnitTower tower)
		{
			for (int i = 0; i < instance.sampleTowerList.Count; i++)
			{
				if (instance.sampleTowerList[i].prefabID == tower.prefabID)
				{
					return instance.sampleTowerList[i];
				}
			}
			return null;
		}

		public static void ShowSampleTower(int ID, BuildInfo buildInfo)
		{
			instance._ShowSampleTowerList(ID, buildInfo);
		}

		public void _ShowSampleTowerList(int ID, BuildInfo buildInfo)
		{
			if (currentSampleID != ID && buildInfo != null)
			{
				if (currentSampleID >= 0)
				{
					ClearSampleTower();
				}
				currentSampleID = ID;
				sampleTowerList[ID].thisT.position = buildInfo.position;
				sampleTowerList[ID].thisT.rotation = buildInfo.platform.transform.rotation;
				GameControl.SelectTower(sampleTowerList[ID]);
				sampleTowerList[ID].thisObj.SetActive(value: true);
			}
		}

		public static void ClearSampleTower()
		{
			instance._ClearSampleTower();
		}

		public void _ClearSampleTower()
		{
			if (currentSampleID >= 0)
			{
				sampleTowerList[currentSampleID].thisObj.SetActive(value: false);
				currentSampleID = -1;
			}
		}

		public static int GetTowerListCount()
		{
			return (!(instance == null)) ? instance.towerList.Count : 0;
		}

		public static List<UnitTower> GetTowerList()
		{
			return (!(instance == null)) ? instance.towerList : new List<UnitTower>();
		}

		public static UnitTower GetTowerPrefab(int ID)
		{
			foreach (UnitTower tower in instance.towerList)
			{
				if (tower.prefabID == ID)
				{
					return tower;
				}
			}
			return null;
		}
	}
}
