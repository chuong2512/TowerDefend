using UnityEngine;
using UnityEngine.UI;

namespace TDTK
{
	public class UIUnitOverlay : MonoBehaviour
	{
		[HideInInspector]
		public Unit unit;

		public Slider sliderHP;

		public Slider sliderShield;

		private GameObject thisObj;

		private RectTransform rectT;

		private void Awake()
		{
			thisObj = base.gameObject;
			rectT = base.gameObject.GetComponent<RectTransform>();
		}

		private void LateUpdate()
		{
			if (unit == null)
			{
				if (thisObj.activeInHierarchy)
				{
					thisObj.SetActive(value: false);
				}
			}
			else if (unit.IsDestroyed() || (unit.HP >= unit.GetFullHP() && unit.shield >= unit.GetFullShield()))
			{
				UIUnitOverlayManager.RemoveUnit(unit);
				unit = null;
				thisObj.SetActive(value: false);
			}
			else if (thisObj.activeInHierarchy)
			{
				Vector3 a = Camera.main.WorldToScreenPoint(unit.thisT.position + new Vector3(0f, 1f, 0f));
				a.z = 0f;
				rectT.localPosition = a * UIMainControl.GetScaleFactor();
				float fullHP = unit.GetFullHP();
				sliderHP.value = ((!(fullHP <= 0f)) ? (unit.HP / fullHP) : 0f);
				if (sliderShield.gameObject.activeInHierarchy)
				{
					float fullShield = unit.GetFullShield();
					sliderShield.value = ((!(fullShield <= 0f)) ? (unit.shield / fullShield) : 0f);
				}
			}
		}

		public void SetUnit(Unit tgtUnit)
		{
			unit = tgtUnit;
			sliderShield.gameObject.SetActive((!(unit.GetFullShield() <= 0f)) ? true : false);
		}
	}
}
