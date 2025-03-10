using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TDTK
{
	public class UIBuildingOverlay : MonoBehaviour
	{
		public List<Slider> buildingBarList = new List<Slider>();

		private static UIBuildingOverlay instance;

		public static UIBuildingOverlay GetInstance()
		{
			return instance;
		}

		private void Awake()
		{
			instance = this;
			for (int i = 0; i < 20; i++)
			{
				if (i > 0)
				{
					GameObject gameObject = UI.Clone(buildingBarList[0].gameObject, string.Empty);
					buildingBarList.Add(gameObject.GetComponent<Slider>());
				}
				buildingBarList[i].gameObject.SetActive(value: false);
			}
		}

		private void OnEnable()
		{
			TDTK.onTowerConstructingE += OnShowBar;
			TDTK.onTowerUpgradingE += OnShowBar;
		}

		private void OnDisable()
		{
			TDTK.onTowerConstructingE -= OnShowBar;
			TDTK.onTowerUpgradingE -= OnShowBar;
		}

		public void OnShowBar(UnitTower tower)
		{
			StartCoroutine(BuildingBarRoutine(tower));
		}

		public IEnumerator BuildingBarRoutine(UnitTower tower)
		{
			Slider bar = buildingBarList[GetUnusedBuildingBarIndex()];
			Transform barT = bar.transform;
			bar.gameObject.SetActive(value: true);
			while (tower != null && tower.IsInConstruction())
			{
				bar.value = tower.GetBuildProgress();
				Vector3 screenPos = Camera.main.WorldToScreenPoint(tower.thisT.position + new Vector3(0f, 0f, 0f));
				barT.localPosition = (screenPos + new Vector3(0f, -20f, 0f)) * UIMainControl.GetScaleFactor();
				yield return null;
			}
			bar.gameObject.SetActive(value: false);
		}

		private int GetUnusedBuildingBarIndex()
		{
			for (int i = 0; i < buildingBarList.Count; i++)
			{
				if (!buildingBarList[i].gameObject.activeInHierarchy)
				{
					return i;
				}
			}
			GameObject gameObject = UI.Clone(buildingBarList[0].gameObject, string.Empty);
			buildingBarList.Add(gameObject.GetComponent<Slider>());
			return buildingBarList.Count - 1;
		}
	}
}
