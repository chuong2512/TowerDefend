using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace TDTK
{
	public class UIUnitOverlayManager : MonoBehaviour
	{
		public List<Unit> unitList = new List<Unit>();

		public List<UIUnitOverlay> unitOverlayList = new List<UIUnitOverlay>();

		private static UIUnitOverlayManager instance;

		[CompilerGenerated]
		private static TDTK.UnitDamagedHandler _003C_003Ef__mg_0024cache0;

		[CompilerGenerated]
		private static TDTK.UnitDamagedHandler _003C_003Ef__mg_0024cache1;

		public static UIUnitOverlayManager GetInstance()
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
					GameObject gameObject = UI.Clone(unitOverlayList[0].gameObject, string.Empty);
					unitOverlayList.Add(gameObject.GetComponent<UIUnitOverlay>());
				}
				unitOverlayList[i].gameObject.SetActive(value: false);
			}
		}

		private void Start()
		{
			if (!UIMainControl.EnableHPOverlay())
			{
				base.gameObject.SetActive(value: false);
			}
		}

		private void OnEnable()
		{
			TDTK.onUnitDamagedE += OnUnitDamaged;
		}

		private void OnDisable()
		{
			TDTK.onUnitDamagedE += OnUnitDamaged;
		}

		public static void OnUnitDamaged(Unit unit)
		{
			instance._OnUnitDamaged(unit);
		}

		public void _OnUnitDamaged(Unit unit)
		{
			if (UIMainControl.EnableHPOverlay() && !unitList.Contains(unit))
			{
				unitList.Add(unit);
				int unusedUnitOverlayIndex = GetUnusedUnitOverlayIndex();
				unitOverlayList[unusedUnitOverlayIndex].SetUnit(unit);
				unitOverlayList[unusedUnitOverlayIndex].gameObject.SetActive(value: true);
			}
		}

		private int GetUnusedUnitOverlayIndex()
		{
			for (int i = 0; i < unitOverlayList.Count; i++)
			{
				if (!(unitOverlayList[i].unit != null))
				{
					return i;
				}
			}
			GameObject gameObject = UI.Clone(unitOverlayList[0].gameObject, string.Empty);
			unitOverlayList.Add(gameObject.GetComponent<UIUnitOverlay>());
			return unitOverlayList.Count - 1;
		}

		public static void RemoveUnit(Unit unit)
		{
			instance.unitList.Remove(unit);
		}
	}
}
