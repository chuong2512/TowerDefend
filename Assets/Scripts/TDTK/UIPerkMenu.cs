using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TDTK
{
	public class UIPerkMenu : MonoBehaviour
	{
		public bool manuallySetupItem;

		[Space(10f)]
		public List<UIPerkItem> perkItemList = new List<UIPerkItem>();

		private int selectID;

		[Space(10f)]
		public Text lbPerkName;

		public Text lbPerkDesp;

		public Text lbPerkReq;

		public List<UIObject> rscItemList = new List<UIObject>();

		private GameObject rscRootObj;

		[Space(10f)]
		public Text lbPerkPoint;

		public UIButton butPurchase;

		public GameObject butCloseObj;

		private GameObject thisObj;

		private RectTransform rectT;

		private CanvasGroup canvasGroup;

		private static UIPerkMenu instance;

		public GridLayoutGroup layoutGroup;

		private bool isOn;

		public void Awake()
		{
			instance = this;
			thisObj = base.gameObject;
			rectT = thisObj.GetComponent<RectTransform>();
			canvasGroup = thisObj.GetComponent<CanvasGroup>();
			if (canvasGroup == null)
			{
				canvasGroup = thisObj.AddComponent<CanvasGroup>();
			}
			canvasGroup.alpha = 0f;
		}

		private void Start()
		{
			if (!manuallySetupItem)
			{
				List<Perk> perkList = PerkManager.GetPerkList();
				for (int i = 0; i < perkList.Count; i++)
				{
					if (i == 0)
					{
						perkItemList[0].Init();
					}
					else if (i > 0)
					{
						perkItemList.Add(UIPerkItem.Clone(perkItemList[0].rootObj, "PerkButton" + (i + 1)));
					}
					perkItemList[i].imgIcon.sprite = perkList[i].icon;
					perkItemList[i].perkID = perkList[i].ID;
					perkItemList[i].selectHighlight.SetActive(i == 0);
					perkItemList[i].SetCallback(null, null, OnPerkItem);
				}
				UpdateContentRectSize();
			}
			else
			{
				for (int j = 0; j < perkItemList.Count; j++)
				{
					perkItemList[j].Init();
					perkItemList[j].selectHighlight.SetActive(j == 0);
					perkItemList[j].SetCallback(null, null, OnPerkItem);
				}
			}
			List<Rsc> resourceList = ResourceManager.GetResourceList();
			for (int k = 0; k < resourceList.Count; k++)
			{
				if (k == 0)
				{
					rscItemList[0].Init();
				}
				else
				{
					rscItemList.Add(UIObject.Clone(rscItemList[0].rootObj, "Rsc" + (k + 1)));
				}
				rscItemList[k].imgRoot.sprite = resourceList[k].icon;
				rscItemList[k].label.text = string.Empty;
			}
			rscRootObj = rscItemList[0].rectT.parent.gameObject;
			butPurchase.Init();
			UpdatePerkItemList();
			UpdateDisplay();
			rectT.localPosition = new Vector3(0f, 99999f, 0f);
		}

		private void UpdateContentRectSize()
		{
			int num = (int)Mathf.Ceil((float)perkItemList.Count / (float)layoutGroup.constraintCount);
			float num2 = num;
			Vector2 cellSize = layoutGroup.cellSize;
			float num3 = num2 * cellSize.y;
			float num4 = num;
			Vector2 spacing = layoutGroup.spacing;
			float y = num3 + num4 * spacing.y + (float)layoutGroup.padding.top;
			RectTransform component = layoutGroup.gameObject.GetComponent<RectTransform>();
			RectTransform rectTransform = component;
			Vector2 sizeDelta = component.sizeDelta;
			rectTransform.sizeDelta = new Vector2(sizeDelta.x, y);
		}

		private void Update()
		{
			if (IsOn() && UnityEngine.Input.GetKeyDown(KeyCode.Escape))
			{
				OnCloseButton();
			}
		}

		public void OnPerkItem(GameObject butObj, int pointerID)
		{
			int buttonID = GetButtonID(butObj);
			perkItemList[selectID].selectHighlight.SetActive(value: false);
			selectID = buttonID;
			perkItemList[selectID].selectHighlight.SetActive(value: true);
			UpdateDisplay();
		}

		private int GetButtonID(GameObject butObj)
		{
			for (int i = 0; i < perkItemList.Count; i++)
			{
				if (perkItemList[i].rootObj == butObj)
				{
					return i;
				}
			}
			return 0;
		}

		private void UpdateDisplay()
		{
			lbPerkPoint.text = "Points: " + PerkManager.GetPerkPoint();
			Perk perk = PerkManager.GetPerk(perkItemList[selectID].perkID);
			lbPerkName.text = perk.name;
			lbPerkDesp.text = perk.desp;
			if (perk.purchased)
			{
				lbPerkReq.text = string.Empty;
				rscRootObj.SetActive(value: false);
				butPurchase.label.text = "Purchased";
				butPurchase.button.interactable = false;
				return;
			}
			butPurchase.label.text = "Purchase";
			string text = perk.IsAvailable();
			if (text == string.Empty)
			{
				List<int> cost = perk.GetCost();
				for (int i = 0; i < rscItemList.Count; i++)
				{
					rscItemList[i].label.text = cost[i].ToString();
				}
				lbPerkReq.text = string.Empty;
				rscRootObj.SetActive(value: true);
				butPurchase.button.interactable = true;
			}
			else
			{
				lbPerkReq.text = text;
				rscRootObj.SetActive(value: false);
				butPurchase.button.interactable = false;
			}
		}

		private void UpdatePerkItemList()
		{
			for (int i = 0; i < perkItemList.Count; i++)
			{
				bool flag = PerkManager.IsPerkPurchased(perkItemList[i].perkID);
				bool flag2 = PerkManager.IsPerkAvailable(perkItemList[i].perkID) == string.Empty;
				perkItemList[i].purchasedHighlight.SetActive(flag);
				perkItemList[i].unavailableHighlight.SetActive(!flag && !flag2);
				if (perkItemList[i].connector != null)
				{
					perkItemList[i].connector.SetActive(flag);
				}
			}
		}

		public void OnPurchaseButton()
		{
			string text = PerkManager.PurchasePerk(perkItemList[selectID].perkID);
			if (text != string.Empty)
			{
				UIMessage.DisplayMessage(text);
				return;
			}
			UpdatePerkItemList();
			UpdateDisplay();
		}

		public void OnCloseButton()
		{
			UIMainControl.ClosePerkMenu();
		}

		public static void DisableCloseButton()
		{
			instance.butCloseObj.SetActive(value: false);
		}

		public static bool IsOn()
		{
			return !(instance == null) && instance.isOn;
		}

		public static void Show()
		{
			instance._Show();
		}

		public void _Show()
		{
			UpdatePerkItemList();
			UpdateDisplay();
			isOn = true;
			rectT.localPosition = Vector3.zero;
			UIMainControl.FadeIn(canvasGroup);
		}

		public static void Hide()
		{
			instance._Hide();
		}

		public void _Hide()
		{
			UIMainControl.FadeOut(canvasGroup);
			StartCoroutine(DelayHide());
		}

		private IEnumerator DelayHide()
		{
			yield return new WaitForSeconds(0.25f);
			isOn = false;
			rectT.localPosition = new Vector3(-5000f, -5000f, 0f);
		}
	}
}
