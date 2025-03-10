using UnityEngine;
using UnityEngine.UI;

namespace SIS
{
	public class IAPItem : MonoBehaviour
	{
		[HideInInspector]
		public string productId;

		public Text title;

		public Text description;

		public bool uppercase;

		public Image icon;

		public Text[] price;

		public GameObject buyButton;

		public GameObject buyTrigger;

		public Text lockedLabel;

		public GameObject[] hideOnUnlock;

		public GameObject[] showOnUnlock;

		public GameObject sold;

		public GameObject selected;

		public GameObject selectButton;

		public GameObject deselectButton;

		private Toggle selCheck;

		[HideInInspector]
		public IAPType type;

		private void Start()
		{
			if ((bool)selectButton)
			{
				selCheck = selectButton.GetComponent<Toggle>();
				if ((bool)selCheck)
				{
					selCheck.group = base.transform.parent.GetComponent<ToggleGroup>();
				}
			}
		}

		private void OnDisable()
		{
			if ((bool)buyTrigger)
			{
				ConfirmPurchase(selected: false);
			}
		}

		public void Init(IAPObject prod)
		{
			type = prod.type;
			string text = prod.title;
			string text2 = prod.description.Replace("\\n", "\n");
			string text3 = prod.req.labelText;
			productId = prod.id;
			if ((bool)icon)
			{
				icon.sprite = prod.icon;
			}
			if (uppercase)
			{
				text = text.ToUpper();
				text2 = text2.ToUpper();
				text3 = text3.ToUpper();
			}
			if ((bool)title)
			{
				title.text = text;
			}
			if ((bool)description)
			{
				description.text = text2;
			}
			if (type == IAPType.consumable || type == IAPType.nonConsumable || type == IAPType.subscription)
			{
				if (price.Length > 0)
				{
					price[0].text = prod.realPrice;
				}
			}
			else if (prod.virtualPrice.Count > 0)
			{
				for (int i = 0; i < price.Length; i++)
				{
					if ((bool)price[i])
					{
						price[i].text = prod.virtualPrice[i].amount.ToString();
					}
				}
			}
			if ((bool)lockedLabel && !string.IsNullOrEmpty(prod.req.entry) && !string.IsNullOrEmpty(prod.req.labelText))
			{
				lockedLabel.text = text3;
			}
		}

		public void Init(IAPArticle prod)
		{
			string text = prod.title;
			string text2 = prod.description.Replace("\\n", "\n");
			int num = text.IndexOf("(");
			if (num > 0)
			{
				text = text.Substring(0, num - 1);
			}
			if (uppercase)
			{
				text = text.ToUpper();
				text2 = text2.ToUpper();
			}
			if ((bool)title)
			{
				title.text = text;
			}
			if ((bool)description)
			{
				description.text = text2;
			}
			if (price.Length > 0)
			{
				price[0].text = prod.price;
			}
		}

		public void Unlock()
		{
			for (int i = 0; i < hideOnUnlock.Length; i++)
			{
				hideOnUnlock[i].SetActive(value: false);
			}
			for (int j = 0; j < showOnUnlock.Length; j++)
			{
				showOnUnlock[j].SetActive(value: true);
			}
		}

		public void ConfirmPurchase(bool selected)
		{
			if (!selected)
			{
				buyButton.SetActive(value: false);
			}
		}

		public void Purchase()
		{
			IAPManager.PurchaseProduct(productId);
			if ((bool)buyTrigger)
			{
				ConfirmPurchase(selected: false);
			}
		}

		public void Purchased(bool state)
		{
			Unlock();
			if (!state)
			{
				Deselect();
			}
			else if ((bool)selectButton)
			{
				selectButton.SetActive(state);
			}
			IAPObject iAPObject = IAPManager.GetIAPObject(productId);
			bool flag = false;
			string nextId = iAPObject.req.nextId;
			if (!string.IsNullOrEmpty(nextId))
			{
				flag = true;
				Init(IAPManager.GetIAPObject(nextId));
				SendMessage("OnLanguageChanged", SendMessageOptions.DontRequireReceiver);
			}
			if (state && flag)
			{
				state = !flag;
			}
			if ((bool)sold)
			{
				sold.SetActive(state);
			}
			if (type != IAPType.subscription)
			{
				if ((bool)buyTrigger)
				{
					buyTrigger.SetActive(!state);
					buyButton.SetActive(value: false);
				}
				else
				{
					buyButton.SetActive(!state);
				}
			}
		}

		public void IsSelected(bool thisSelect)
		{
			if (thisSelect)
			{
				ShopManager.SetToSelected(this);
				if ((bool)deselectButton)
				{
					deselectButton.SetActive(value: true);
				}
				if ((bool)selected)
				{
					selected.SetActive(value: true);
				}
				Toggle component = selectButton.GetComponent<Toggle>();
				if ((bool)component.group)
				{
					IAPItem[] componentsInChildren = component.group.GetComponentsInChildren<IAPItem>(includeInactive: true);
					for (int i = 0; i < componentsInChildren.Length; i++)
					{
						if (componentsInChildren[i].selCheck.isOn && componentsInChildren[i] != this)
						{
							componentsInChildren[i].IsSelected(thisSelect: false);
							break;
						}
					}
				}
				component.isOn = true;
				selectButton.SetActive(value: false);
			}
			else
			{
				if (!deselectButton)
				{
					selectButton.SetActive(value: true);
				}
				if ((bool)selected)
				{
					selected.SetActive(value: false);
				}
			}
		}

		public void Deselect()
		{
			deselectButton.SetActive(value: false);
			if ((bool)selected)
			{
				selected.SetActive(value: false);
			}
			if ((bool)selCheck)
			{
				selCheck.isOn = false;
			}
			ShopManager.SetToDeselected(this);
			selectButton.SetActive(value: true);
		}
	}
}
