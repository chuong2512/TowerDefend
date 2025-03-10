using UnityEngine;

namespace TDTK
{
	public class IndicatorControl : MonoBehaviour
	{
		public bool disableCursorIndicator;

		[Header("Indicator Prefabs")]
		public Transform indicatorSelected;

		public Transform indicatorCursor;

		private GameObject indicatorSelectedObj;

		private GameObject indicatorCursorObj;

		private Renderer indicatorSelectedRenderer;

		private UnitTower currentTower;

		public Transform rangeIndicator;

		public Transform rangeIndicatorCone;

		private GameObject rangeIndicatorObj;

		private GameObject rangeIndicatorConeObj;

		private static IndicatorControl instance;

		private Transform thisT;

		private bool inDragNDropPhase;

		public void Init()
		{
			instance = this;
			thisT = base.transform;
			float gridSize = BuildManager.GetGridSize();
			if (indicatorSelected != null)
			{
				indicatorSelected = UnityEngine.Object.Instantiate(indicatorSelected);
				indicatorSelected.localScale = new Vector3(gridSize, gridSize, gridSize);
				indicatorSelected.parent = thisT;
				indicatorSelected.name = "TileIndicator_Selected";
				indicatorSelectedRenderer = indicatorSelected.GetChild(0).GetComponent<Renderer>();
				indicatorSelectedObj = indicatorSelected.gameObject;
				indicatorSelectedObj.SetActive(value: false);
			}
			if (indicatorCursor != null)
			{
				indicatorCursor = UnityEngine.Object.Instantiate(indicatorCursor);
				indicatorCursor.localScale = new Vector3(gridSize, gridSize, gridSize);
				indicatorCursor.parent = thisT;
				indicatorCursor.name = "TileIndicator_Cursor";
				indicatorCursorObj = indicatorCursor.gameObject;
				indicatorCursorObj.SetActive(value: false);
			}
			if (rangeIndicator != null)
			{
				rangeIndicator = UnityEngine.Object.Instantiate(rangeIndicator);
				rangeIndicator.parent = thisT;
				rangeIndicatorObj = rangeIndicator.gameObject;
			}
			if (rangeIndicatorCone != null)
			{
				rangeIndicatorCone = UnityEngine.Object.Instantiate(rangeIndicatorCone);
				rangeIndicatorCone.parent = thisT;
				rangeIndicatorConeObj = rangeIndicatorCone.gameObject;
			}
			_ClearRangeIndicator();
		}

		private void Update()
		{
			if (!FPSControl.IsInFPSMode() && !AbilityManager.IsSelectingTarget())
			{
				BuildManager.SetTileIndicator(UnityEngine.Input.mousePosition);
			}
		}

		public static void SetDragNDropPhase(bool flag)
		{
			instance.inDragNDropPhase = flag;
			instance.indicatorSelectedObj.SetActive(value: false);
		}

		public static void SetIndicatorCursor(Vector3 pos, Quaternion rot)
		{
			if (!instance.inDragNDropPhase)
			{
				instance.indicatorCursor.position = pos;
				instance.indicatorCursor.rotation = rot;
				instance.indicatorCursorObj.SetActive(value: true);
			}
		}

		public static void ClearIndicatorCursor()
		{
			instance.indicatorCursorObj.SetActive(value: false);
		}

		public static void SetBuildTileIndicator(BuildInfo buildInfo)
		{
			instance._SetBuildTileIndicator(buildInfo);
		}

		public void _SetBuildTileIndicator(BuildInfo buildInfo)
		{
			if (buildInfo.status == _TileStatus.NoPlatform)
			{
				indicatorSelectedObj.SetActive(value: false);
				return;
			}
			if (buildInfo.status == _TileStatus.Available)
			{
				indicatorSelectedRenderer.material.SetColor("_Color", new Color(0f, 1f, 0f, 1f));
			}
			else
			{
				indicatorSelectedRenderer.material.SetColor("_Color", new Color(1f, 0f, 0f, 1f));
			}
			indicatorSelected.position = buildInfo.position;
			if (buildInfo.platform != null)
			{
				indicatorSelected.rotation = buildInfo.platform.thisT.rotation;
			}
			indicatorSelectedObj.SetActive(value: true);
		}

		public static void ClearBuildTileIndicator()
		{
			instance.indicatorSelectedObj.SetActive(value: false);
		}

		public static void ShowTowerRangeIndicator(UnitTower tower)
		{
			instance._ShowTowerRangeIndicator(tower);
		}

		public void _ShowTowerRangeIndicator(UnitTower tower)
		{
			_ClearRangeIndicator();
			if (tower.type == _TowerType.Block || tower.type == _TowerType.Resource)
			{
				return;
			}
			_ClearRangeIndicator();
			currentTower = tower;
			indicatorSelectedRenderer.material.SetColor("_Color", new Color(0f, 1f, 0f, 1f));
			indicatorSelected.position = tower.thisT.position;
			indicatorSelected.rotation = tower.thisT.rotation;
			indicatorSelectedObj.SetActive(value: true);
			float range = tower.GetRange();
			Transform transform = tower.directionalTargeting ? rangeIndicatorCone : rangeIndicator;
			if (transform != null)
			{
				transform.position = tower.thisT.position;
				transform.localScale = new Vector3(2f * range, 1f, 2f * range);
				transform.parent = tower.thisT;
				if (tower.directionalTargeting)
				{
					transform.localRotation = Quaternion.identity * Quaternion.Euler(0f, tower.dirScanAngle, 0f);
				}
				transform.gameObject.SetActive(value: true);
			}
		}

		public static void ClearRangeIndicator()
		{
			instance._ClearRangeIndicator();
		}

		public void _ClearRangeIndicator()
		{
			currentTower = null;
			indicatorSelectedObj.SetActive(value: false);
			rangeIndicatorObj.SetActive(value: false);
			rangeIndicatorConeObj.SetActive(value: false);
			rangeIndicator.parent = thisT;
			rangeIndicatorCone.parent = thisT;
		}

		public static void TowerScanAngleChanged(UnitTower tower)
		{
			instance.rangeIndicatorCone.localRotation = tower.thisT.localRotation * Quaternion.Euler(0f, tower.dirScanAngle, 0f);
		}

		public static void TowerRemoved(UnitTower tower)
		{
			if (instance.currentTower == tower)
			{
				ClearRangeIndicator();
			}
		}
	}
}
