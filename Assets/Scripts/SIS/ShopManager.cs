using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace SIS
{
	public class ShopManager : MonoBehaviour
	{
		private static ShopManager instance;

		public GameObject errorWindow;

		public Text message;

		[HideInInspector]
		public List<ShopContainer> containers = new List<ShopContainer>();

		public Dictionary<string, IAPItem> IAPItems = new Dictionary<string, IAPItem>();

		public static event Action<string> itemSelectedEvent;

		public static event Action<string> itemDeselectedEvent;

		public void Init()
		{
			instance = this;
			InitShop();
			SetItemState();
			UnlockItems();
		}

		private void Start()
		{
			if (!IAPManager.GetInstance())
			{
				UnityEngine.Debug.LogWarning("ShopManager: Could not find IAPManager prefab. Have you placed it in the first scene of your app and started from there? Instantiating temporary copy...");
				GameObject gameObject = UnityEngine.Object.Instantiate(Resources.Load("IAPManager", typeof(GameObject))) as GameObject;
				gameObject.name = gameObject.name.Replace("(Clone)", string.Empty);
			}
		}

		public static ShopManager GetInstance()
		{
			return instance;
		}

		private void InitShop()
		{
			IAPItems.Clear();
			List<IAPGroup> iAPs = IAPManager.GetInstance().IAPs;
			int num = 0;
			for (int i = 0; i < iAPs.Count; i++)
			{
				IAPGroup iAPGroup = iAPs[i];
				ShopContainer container = GetContainer(iAPGroup.id);
				if (container == null || container.prefab == null || container.parent == null)
				{
					continue;
				}
				for (int j = 0; j < iAPGroup.items.Count; j++)
				{
					IAPObject iAPObject = iAPGroup.items[j];
					GameObject gameObject = UnityEngine.Object.Instantiate(container.prefab);
					gameObject.transform.SetParent(container.parent.transform, worldPositionStays: false);
					gameObject.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
					gameObject.name = "IAPItem " + $"{num + j:000}";
					IAPItem component = gameObject.GetComponent<IAPItem>();
					if (component == null)
					{
						continue;
					}
					IAPItems.Add(iAPObject.id, component);
					List<string> iAPUpgrades = IAPManager.GetIAPUpgrades(iAPObject.id);
					if (iAPUpgrades != null && iAPUpgrades.Count > 0)
					{
						for (int k = 0; k < iAPUpgrades.Count; k++)
						{
							IAPItems.Add(iAPUpgrades[k], component);
						}
						string nextUpgrade = IAPManager.GetNextUpgrade(iAPObject.id);
						if (!string.IsNullOrEmpty(nextUpgrade))
						{
							iAPObject = IAPManager.GetIAPObject(nextUpgrade);
						}
					}
					component.Init(iAPObject);
				}
				num += iAPGroup.items.Count;
			}
		}

		public static void SetItemState()
		{
			if (!DBManager.GetInstance())
			{
				return;
			}
			List<string> allPurchased = DBManager.GetAllPurchased();
			for (int i = 0; i < allPurchased.Count; i++)
			{
				if (instance.IAPItems.ContainsKey(allPurchased[i]) && IAPManager.GetIAPUpgrades(allPurchased[i]).Count == 0)
				{
					instance.IAPItems[allPurchased[i]].Purchased(state: true);
				}
			}
			Dictionary<string, List<string>> allSelected = DBManager.GetAllSelected();
			foreach (string key in allSelected.Keys)
			{
				for (int j = 0; j < allSelected[key].Count; j++)
				{
					if (instance.IAPItems.ContainsKey(allSelected[key][j]))
					{
						instance.IAPItems[allSelected[key][j]].IsSelected(thisSelect: true);
					}
				}
			}
		}

		public static void UnlockItems()
		{
			if (!DBManager.GetInstance())
			{
				return;
			}
			List<IAPGroup> iAPs = IAPManager.GetInstance().IAPs;
			for (int i = 0; i < iAPs.Count; i++)
			{
				IAPGroup iAPGroup = iAPs[i];
				for (int j = 0; j < iAPGroup.items.Count; j++)
				{
					IAPObject iAPObject = iAPGroup.items[j];
					if (iAPObject.req == null)
					{
						continue;
					}
					IAPItem iAPItem = GetIAPItem(iAPObject.id);
					if (!(iAPItem == null) && !DBManager.isPurchased(iAPObject.id) && !string.IsNullOrEmpty(iAPObject.req.entry) && DBManager.isRequirementMet(iAPObject.req))
					{
						if (IAPManager.isDebug)
						{
							UnityEngine.Debug.Log("requirement met for: " + iAPObject.id);
						}
						iAPItem.Unlock();
					}
				}
			}
		}

		public static void OverwriteWithFetch(List<IAPArticle> products)
		{
			for (int i = 0; i < products.Count; i++)
			{
				string iAPIdentifier = IAPManager.GetIAPIdentifier(products[i].id);
				IAPObject iAPObject = IAPManager.GetIAPObject(iAPIdentifier);
				if (iAPObject != null && iAPObject.fetch && instance.IAPItems.ContainsKey(iAPIdentifier))
				{
					instance.IAPItems[iAPIdentifier].Init(products[i]);
				}
			}
		}

		public static void SetToSelected(IAPItem item)
		{
			bool single = (!item.deselectButton) ? true : false;
			if (DBManager.SetToSelected(item.productId, single) && ShopManager.itemSelectedEvent != null)
			{
				ShopManager.itemSelectedEvent(item.productId);
			}
		}

		public static void SetToDeselected(IAPItem item)
		{
			DBManager.SetToDeselected(item.productId);
			if (ShopManager.itemDeselectedEvent != null)
			{
				ShopManager.itemDeselectedEvent(item.productId);
			}
		}

		public static void ShowMessage(string text)
		{
			if ((bool)instance.errorWindow)
			{
				if ((bool)instance.message)
				{
					instance.message.text = text;
				}
				instance.errorWindow.SetActive(value: true);
			}
		}

		public static IAPItem GetIAPItem(string id)
		{
			if (instance.IAPItems.ContainsKey(id))
			{
				return instance.IAPItems[id];
			}
			return null;
		}

		public ShopContainer GetContainer(string id)
		{
			for (int i = 0; i < containers.Count; i++)
			{
				if (containers[i].id.Equals(id))
				{
					return containers[i];
				}
			}
			return null;
		}
	}
}
