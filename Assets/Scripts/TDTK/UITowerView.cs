using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

namespace TDTK
{
	public class UITowerView : MonoBehaviour
	{
		private Transform thisT;

		private GameObject thisObj;

		private CanvasGroup canvasGroup;

		private static UITowerView instance;

		private int currentState;

		private int currentUpgradeIndex;

		private UnitTower currentTower;

		public RectTransform towerPanelRectT;

		private float towerPanelPosX;

		public Text lbTowerName;

		public Text lbTowerLevel;

		public Text lbTowerDesp1;

		public Text lbTowerDesp2;

		public GameObject butUpgradeObj1;

		public GameObject butUpgradeObj2;

		public GameObject butSellObj;

		public GameObject butFPSObj;

		private UIButton butSell;

		private UIButton butUpgrade1;

		private UIButton butUpgrade2;

		private CanvasGroup butUpgrade1Canvas;

		private CanvasGroup butUpgrade2Canvas;

		public GameObject directionControlObj;

		public Slider sliderDrection;

		public GameObject rscPanelObj;

		public List<UIObject> rscItemList = new List<UIObject>();

		[CompilerGenerated]
		private static TDTK.TowerUpgradingHandler _003C_003Ef__mg_0024cache0;

		[CompilerGenerated]
		private static TDTK.TowerUpgradingHandler _003C_003Ef__mg_0024cache1;

		public static UITowerView GetInstance()
		{
			return instance;
		}

		private void Awake()
		{
			instance = this;
			thisT = base.transform;
			thisObj = base.gameObject;
			canvasGroup = thisObj.GetComponent<CanvasGroup>();
			if (canvasGroup == null)
			{
				canvasGroup = thisObj.AddComponent<CanvasGroup>();
			}
			canvasGroup.alpha = 0f;
			thisT.localPosition = new Vector3(0f, 9999f, 0f);
			butUpgrade1Canvas = butUpgradeObj1.GetComponent<CanvasGroup>();
			butUpgrade2Canvas = butUpgradeObj2.GetComponent<CanvasGroup>();
			butSell = new UIButton(butSellObj);
			butUpgrade1 = new UIButton(butUpgradeObj1);
			butUpgrade2 = new UIButton(butUpgradeObj2);
		}

		private void Start()
		{
			List<Rsc> resourceList = ResourceManager.GetResourceList();
			for (int i = 0; i < resourceList.Count; i++)
			{
				if (i == 0)
				{
					rscItemList[0].Init();
				}
				else
				{
					rscItemList.Add(UIObject.Clone(rscItemList[0].rootObj, "Rsc" + (i + 1)));
				}
				rscItemList[i].imgRoot.sprite = resourceList[i].icon;
				rscItemList[i].label.text = resourceList[i].value.ToString();
			}
			rscPanelObj.SetActive(value: false);
			Vector3 localPosition = towerPanelRectT.localPosition;
			towerPanelPosX = localPosition.x;
			if (UIMainControl.InTouchMode())
			{
				butSell.SetCallback(null, null, OnSellButton);
				butUpgrade1.SetCallback(null, null, OnUpgradeButton1);
				butUpgrade2.SetCallback(null, null, OnUpgradeButton2);
			}
			else
			{
				butSell.SetCallback(OnHoverSellButton, OnExitSellButton, OnSellButton);
				butUpgrade1.SetCallback(OnHoverUpgradeButton1, OnExitUpgradeButton, OnUpgradeButton1);
				butUpgrade2.SetCallback(OnHoverUpgradeButton2, OnExitUpgradeButton, OnUpgradeButton2);
			}
			butSell.imgHighlight.enabled = false;
			butUpgrade1.imgHighlight.enabled = false;
			butUpgrade2.imgHighlight.enabled = false;
		}

		private void OnEnable()
		{
			TDTK.onTowerUpgradingE += Show;
			TDTK.onTowerDestroyedE += OnRemoveTower;
			TDTK.onTowerSoldE += OnRemoveTower;
		}

		private void OnDisable()
		{
			TDTK.onTowerUpgradingE -= Show;
			TDTK.onTowerDestroyedE -= OnRemoveTower;
			TDTK.onTowerSoldE -= OnRemoveTower;
		}

		private void Update()
		{
			if (!(currentTower == null))
			{
				Vector3 vector = Camera.main.WorldToScreenPoint(currentTower.thisT.position);
				float num = (!(vector.x > (float)(Screen.width / 2))) ? towerPanelPosX : (0f - towerPanelPosX);
				RectTransform rectTransform = towerPanelRectT;
				float x = num;
				Vector3 localPosition = towerPanelRectT.localPosition;
				rectTransform.localPosition = new Vector3(x, localPosition.y, 0f);
				if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
				{
					Hide();
				}
			}
		}

		private void OnRemoveTower(UnitTower tower)
		{
			if (tower == currentTower)
			{
				Hide();
			}
		}

		private void ClearState()
		{
			currentState = 0;
			butSell.imgHighlight.enabled = false;
			butUpgrade1.imgHighlight.enabled = false;
			butUpgrade2.imgHighlight.enabled = false;
		}

		private void SetCurrentStateToTwo(int index)
		{
			butSell.imgHighlight.enabled = false;
			OnHoverUpgradeButton(index);
			currentUpgradeIndex = index;
			if (currentUpgradeIndex == 0)
			{
				butUpgrade1.imgHighlight.enabled = true;
			}
			else
			{
				butUpgrade2.imgHighlight.enabled = true;
			}
		}

		public void OnSellButton(GameObject butObj = null, int index = -1)
		{
			if (UIMainControl.InTouchMode())
			{
				if (currentState != 1)
				{
					ClearState();
					OnHoverSellButton();
					currentState = 1;
					butSell.imgHighlight.enabled = true;
					return;
				}
				ClearState();
				OnExitSellButton();
			}
			currentTower.Sell();
			Hide();
		}

		public void OnUpgradeButton1(GameObject butObj = null, int index = -1)
		{
			OnUpgradeButton(0);
		}

		public void OnUpgradeButton2(GameObject butObj = null, int index = -1)
		{
			OnUpgradeButton(1);
		}

		public void OnUpgradeButton(int index)
		{
			if (UIMainControl.InTouchMode())
			{
				if (currentState != 2)
				{
					currentState = 2;
					SetCurrentStateToTwo(index);
					return;
				}
				if (currentUpgradeIndex != index)
				{
					SetCurrentStateToTwo(index);
					return;
				}
				ClearState();
				OnExitUpgradeButton();
			}
			string text = currentTower.Upgrade(index);
			if (text != string.Empty)
			{
				UIMessage.DisplayMessage(text);
			}
		}

		public void OnHoverSellButton(GameObject butObj = null)
		{
			if (!(currentTower == null))
			{
				UpdateResourcePanel(currentTower.GetValue());
			}
		}

		public void OnExitSellButton(GameObject butObj = null)
		{
			rscPanelObj.SetActive(value: false);
		}

		public void OnHoverUpgradeButton1(GameObject butObj = null)
		{
			OnHoverUpgradeButton(0);
		}

		public void OnHoverUpgradeButton2(GameObject butObj = null)
		{
			OnHoverUpgradeButton(1);
		}

		public void OnHoverUpgradeButton(int index)
		{
			if (!(currentTower == null) && (index != 0 || !(butUpgrade1Canvas.alpha < 1f)) && (index != 1 || !(butUpgrade2Canvas.alpha < 1f)))
			{
				UpdateResourcePanel(currentTower.GetCost(index));
			}
		}

		public void OnExitUpgradeButton(GameObject butObj = null)
		{
			rscPanelObj.SetActive(value: false);
		}

		public void UpdateResourcePanel(List<int> costList)
		{
			for (int i = 0; i < rscItemList.Count; i++)
			{
				rscItemList[i].label.text = costList[i].ToString();
			}
		}

		public void OnFPSButton()
		{
			FPSControl.Show(currentTower);
			_Hide();
		}

		public void OnDirectionSlider()
		{
			if (!(currentTower == null))
			{
				currentTower.ChangeScanAngle((int)sliderDrection.value);
			}
		}

		public void UpdateDisplay()
		{
			if (!(currentTower == null))
			{
				lbTowerName.text = currentTower.unitName;
				lbTowerLevel.text = "lvl" + currentTower.GetLevel();
				float num = (currentTower.GetDamageMax() + currentTower.GetDamageMin()) / 2f * (1f / currentTower.GetCooldown());
				lbTowerDesp1.text = "damage: " + currentTower.GetDamageMin() + "-" + currentTower.GetDamageMax() + "\nDamage Per Second: " + num;
				lbTowerDesp1.text = currentTower.GetDespStats();
				lbTowerDesp2.text = currentTower.GetDespGeneral();
				sliderDrection.value = currentTower.dirScanAngle;
				directionControlObj.SetActive(currentTower.directionalTargeting);
				int num2 = currentTower.ReadyToBeUpgrade();
				butUpgrade1Canvas.alpha = ((num2 >= 1) ? 1 : 0);
				butUpgrade1Canvas.interactable = ((num2 >= 1) ? true : false);
				butUpgrade2Canvas.alpha = ((num2 >= 2) ? 1 : 0);
				butUpgrade2Canvas.interactable = ((num2 >= 2) ? true : false);
				butSellObj.SetActive(currentTower.canBeSold);
				List<int> value = currentTower.GetValue();
				butSell.label.text = value[0].ToString();
				if (currentTower.ReadyToBeUpgrade() == 1)
				{
					List<int> cost = currentTower.GetCost();
					butUpgrade1.label.text = cost[0].ToString();
				}
				bool flag = FPSControl.ActiveInScene();
				if (flag && currentTower.disableFPS)
				{
					flag = false;
				}
				if (flag && FPSControl.UseTowerWeapon() && currentTower.FPSWeaponID == -1)
				{
					flag = false;
				}
				if (flag && !FPSControl.IsIDAvailable(currentTower.FPSWeaponID))
				{
					flag = false;
				}
				butFPSObj.SetActive(flag);
			}
		}

		public static bool IsOn()
		{
			return !(instance == null) && instance.currentTower != null;
		}

		public static void Show(UnitTower tower)
		{
			instance._Show(tower);
		}

		public void _Show(UnitTower tower)
		{
			if (UIMainControl.InTouchMode())
			{
				ClearState();
				rscPanelObj.SetActive(value: false);
			}
			bool flag = currentTower == null;
			currentTower = tower;
			UpdateDisplay();
			thisT.localPosition = Vector3.zero;
			if (flag)
			{
				UIMainControl.FadeIn(canvasGroup);
			}
		}

		public static void Hide()
		{
			instance._Hide();
		}

		public void _Hide()
		{
			if (thisObj.activeInHierarchy)
			{
				currentTower = null;
				GameControl.SelectTower();
				UIMainControl.FadeOut(canvasGroup);
				StartCoroutine(DelayHide());
			}
		}

		private IEnumerator DelayHide()
		{
			yield return new WaitForSeconds(0.25f);
			thisT.localPosition = new Vector3(0f, 9999f, 0f);
		}
	}
}
