using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SIS
{
	public class IAPManager : MonoBehaviour
	{
		public static bool isDebug;

		public string serverUrl;

		public RemoteType remoteType;

		public string remoteFileName;

		private static IAPManager instance;

		private WWW request;

		private static bool restoreInProgress;

		private string[] realIDs;

		private ReceiptValidator validator;

		private List<IAPArticle> productCache = new List<IAPArticle>();

		[HideInInspector]
		public List<IAPGroup> IAPs = new List<IAPGroup>();

		[HideInInspector]
		public List<IAPCurrency> currency = new List<IAPCurrency>();

		public Dictionary<string, IAPObject> IAPObjects = new Dictionary<string, IAPObject>();

		public static event Action<string> purchaseSucceededEvent;

		public static event Action<string> purchaseFailedEvent;

		public static event Action<string> inventoryRequestFailedEvent;

		private void Awake()
		{
			if ((bool)instance)
			{
				UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
			UnityEngine.Object.DontDestroyOnLoad(this);
			isDebug = UnityEngine.Debug.isDebugBuild;
			instance = this;
			InitIds();
			AndroidNativeSettings.Instance.InAppProducts.Clear();
			for (int i = 0; i < realIDs.Length; i++)
			{
				AndroidInAppPurchaseManager.Client.AddProduct(realIDs[i]);
			}
			AndroidInAppPurchaseManager.ActionBillingSetupFinished += RequestProductData;
			AndroidInAppPurchaseManager.ActionRetrieveProducsFinished += ProductDataReceived;
			AndroidInAppPurchaseManager.ActionProductPurchased += PurchaseSucceeded;
			AndroidInAppPurchaseManager.ActionProductConsumed += ConsumeSucceeded;
			//AndroidInAppPurchaseManager.Client.Connect();
			GetComponent<IAPListener>().Init();
			GetComponent<DBManager>().Init();
			StartCoroutine(RemoteDownload());
			validator = GetComponent<ReceiptValidator>();
			SceneManager.sceneLoaded += OnSceneWasLoaded;
			OnSceneLoaded();
		}

		public void OnSceneWasLoaded(Scene scene, LoadSceneMode m)
		{
			OnSceneLoaded();
		}

		private void OnSceneLoaded()
		{
			if (!(instance != this))
			{
				ShopManager shopManager = null;
				GameObject gameObject = GameObject.Find("ShopManager");
				if ((bool)gameObject)
				{
					shopManager = gameObject.GetComponent<ShopManager>();
				}
				if ((bool)shopManager)
				{
					shopManager.Init();
					ShopManager.OverwriteWithFetch(productCache);
				}
			}
		}

		public static IAPManager GetInstance()
		{
			return instance;
		}

		private void InitIds()
		{
			List<string> list = new List<string>();
			if (IAPs.Count == 0)
			{
				UnityEngine.Debug.LogError("Initializing IAPManager, but IAP List is empty. Did you set up IAPs in the IAP Settings?");
			}
			for (int i = 0; i < IAPs.Count; i++)
			{
				IAPGroup iAPGroup = IAPs[i];
				for (int j = 0; j < iAPGroup.items.Count; j++)
				{
					IAPObject iAPObject = iAPGroup.items[j];
					if (string.IsNullOrEmpty(iAPObject.id) || IAPObjects.ContainsKey(iAPObject.id))
					{
						UnityEngine.Debug.LogError("Found IAP UnityEngine.Object in IAP Settings without an identifier  or " + iAPObject.id + " does exist already. Skipping product.");
						continue;
					}
					IAPObjects.Add(iAPObject.id, iAPObject);
					if (!iAPObject.isVirtual)
					{
						list.Add(iAPObject.id);
					}
				}
			}
			if (list.Contains("restore"))
			{
				list.Remove("restore");
			}
			realIDs = list.ToArray();
		}

		private void RequestProductData(BillingResult result)
		{
			if (!result.IsSuccess)
			{
				BillingNotSupported(result.Response + ", " + result.Message);
			}
			else
			{
				AndroidInAppPurchaseManager.Client.RetrieveProducDetails();
			}
		}

		private void ProductDataReceived(BillingResult result)
		{
			if (!result.IsSuccess)
			{
				BillingNotSupported(result.Response + ", " + result.Message);
				return;
			}
			AndroidInventory inventory = AndroidInAppPurchaseManager.Client.Inventory;
			List<GooglePurchaseTemplate> purchases = inventory.Purchases;
			for (int i = 0; i < purchases.Count; i++)
			{
				IAPObject iAPObject = GetIAPObject(purchases[i].SKU);
				if (iAPObject != null && iAPObject.type == IAPType.consumable)
				{
					AndroidInAppPurchaseManager.Client.Consume(purchases[i].SKU);
				}
			}
			if ((bool)validator && validator.shouldValidate(VerificationType.onStart))
			{
				validator.Validate();
			}
			productCache = new List<IAPArticle>();
			for (int j = 0; j < realIDs.Length; j++)
			{
				if (!string.IsNullOrEmpty(inventory.GetProductDetails(realIDs[j]).Description))
				{
					productCache.Add(new IAPArticle(inventory.GetProductDetails(realIDs[j])));
				}
			}
			if ((bool)ShopManager.GetInstance())
			{
				ShopManager.OverwriteWithFetch(productCache);
			}
		}

		public static void PurchaseProduct(string productId)
		{
			if (productId == "restore")
			{
				RestoreTransactions();
				return;
			}
			IAPObject iAPObject = GetIAPObject(productId);
			if (iAPObject == null)
			{
				if (isDebug)
				{
					UnityEngine.Debug.LogError("Product " + productId + " not found in IAP Settings.");
				}
			}
			else if (iAPObject.isVirtual)
			{
				PurchaseProduct(iAPObject);
			}
			else
			{
				productId = GetIAPObject(productId).GetIdentifier();
				AndroidInAppPurchaseManager.Client.Purchase(productId);
			}
		}

		private void PurchaseSucceeded(BillingResult result)
		{
			if (!result.IsSuccess)
			{
				PurchaseFailed(result.Response + ", " + result.Message);
				return;
			}
			string iAPIdentifier = GetIAPIdentifier(result.Purchase.SKU);
			if ((bool)validator && validator.shouldValidate(VerificationType.onPurchase))
			{
				validator.Validate(iAPIdentifier, result.Purchase.OriginalJson);
			}
			else
			{
				PurchaseVerified(iAPIdentifier);
			}
		}

		private void ConsumeSucceeded(BillingResult result)
		{
			if (!result.IsSuccess)
			{
				PurchaseFailed(result.Response + ", " + result.Message);
			}
			else
			{
				IAPManager.purchaseSucceededEvent(GetIAPIdentifier(result.Purchase.SKU));
			}
		}

		private void PurchaseVerified(string id)
		{
			if (!IAPObjects.ContainsKey(id))
			{
				return;
			}
			IAPObject iAPObject = IAPObjects[id];
			if (iAPObject.type == IAPType.consumable)
			{
				AndroidInAppPurchaseManager.Client.Consume(iAPObject.GetIdentifier());
			}
			else if (!DBManager.isPurchased(id))
			{
				if (iAPObject.type == IAPType.nonConsumable || iAPObject.type == IAPType.subscription)
				{
					DBManager.SetToPurchased(id);
				}
				IAPManager.purchaseSucceededEvent(id);
			}
		}

		public static void PurchaseProduct(IAPObject obj)
		{
			string id = obj.id;
			if (DBManager.isPurchased(id))
			{
				PurchaseFailed("Product already purchased.");
				return;
			}
			bool flag = DBManager.VerifyVirtualPurchase(obj);
			if (isDebug)
			{
				UnityEngine.Debug.Log("Purchasing virtual product " + id + ", result: " + flag);
			}
			if (flag)
			{
				if (obj.type == IAPType.nonConsumableVirtual)
				{
					DBManager.SetToPurchased(id);
				}
				IAPManager.purchaseSucceededEvent(id);
			}
			else
			{
				PurchaseFailed("Insufficient funds.");
			}
		}

		public static void RestoreTransactions()
		{
			List<GooglePurchaseTemplate> purchases = AndroidInAppPurchaseManager.Client.Inventory.Purchases;
			if (purchases == null)
			{
				RestoreFailed("Restoring transactions failed. Please try again.");
				return;
			}
			for (int i = 0; i < purchases.Count; i++)
			{
				string iAPIdentifier = GetIAPIdentifier(purchases[i].SKU);
				if (!DBManager.isPurchased(iAPIdentifier))
				{
					DBManager.SetToPurchased(iAPIdentifier);
				}
			}
			RestoreSucceeded();
			if ((bool)ShopManager.GetInstance())
			{
				ShopManager.SetItemState();
			}
		}

		private void DebugConsumeProducts()
		{
			UnityEngine.Debug.Log("Attempting to consume all purchases.");
			List<GooglePurchaseTemplate> purchases = AndroidInAppPurchaseManager.Client.Inventory.Purchases;
			for (int i = 0; i < purchases.Count; i++)
			{
				AndroidInAppPurchaseManager.Client.Consume(purchases[i].SKU);
			}
		}

		private IEnumerator RemoteDownload()
		{
			string url = serverUrl + remoteFileName;
			switch (remoteType)
			{
			case RemoteType.cached:
				DBManager.LoadRemoteConfig();
				yield return StartCoroutine(Download(url));
				DBManager.SaveRemoteConfig(request.text);
				break;
			case RemoteType.overwrite:
				yield return StartCoroutine(Download(url));
				DBManager.ConvertToIAPs(request.text);
				break;
			}
		}

		private IEnumerator Download(string url)
		{
			request = new WWW(url);
			yield return request;
			if (!string.IsNullOrEmpty(request.error))
			{
				UnityEngine.Debug.Log("Failed remote config download with error: " + request.error);
			}
			else if (isDebug)
			{
				UnityEngine.Debug.Log("Downloaded remotely hosted config file: \n" + request.text);
			}
		}

		private static void BillingNotSupported(string error)
		{
			if (isDebug)
			{
				UnityEngine.Debug.Log("IAPManager reports: BillingNotSupported. Error: " + error);
			}
			if (IAPManager.inventoryRequestFailedEvent != null)
			{
				IAPManager.inventoryRequestFailedEvent(error);
			}
		}

		private static void PurchaseFailed(string error)
		{
			if (isDebug)
			{
				UnityEngine.Debug.Log("IAPManager reports: PurchaseFailed. Error: " + error);
			}
			if (IAPManager.purchaseFailedEvent != null)
			{
				IAPManager.purchaseFailedEvent(error);
			}
		}

		private static void RestoreSucceeded()
		{
			IAPManager.purchaseSucceededEvent("restore");
		}

		private static void RestoreFailed(string error)
		{
			if (isDebug)
			{
				UnityEngine.Debug.Log("IAPManager reports: RestoreFailed. Error: " + error);
			}
			if (IAPManager.purchaseFailedEvent != null)
			{
				IAPManager.purchaseFailedEvent(error);
			}
		}

		public static List<string> GetIAPUpgrades(string productId)
		{
			List<string> list = new List<string>();
			IAPObject iAPObject = GetIAPObject(productId);
			if (iAPObject == null)
			{
				if (isDebug)
				{
					UnityEngine.Debug.LogError("Product " + productId + " not found in IAP Settings. Make sure to remove your app from the device before deploying it again!");
				}
			}
			else
			{
				while (iAPObject != null && !string.IsNullOrEmpty(iAPObject.req.nextId))
				{
					list.Add(iAPObject.req.nextId);
					iAPObject = GetIAPObject(iAPObject.req.nextId);
				}
			}
			return list;
		}

		public static string GetCurrentUpgrade(string productId)
		{
			if (!DBManager.isPurchased(productId))
			{
				return productId;
			}
			string result = productId;
			List<string> iAPUpgrades = GetIAPUpgrades(productId);
			for (int num = iAPUpgrades.Count - 1; num >= 0; num--)
			{
				if (DBManager.isPurchased(iAPUpgrades[num]))
				{
					result = iAPUpgrades[num];
					break;
				}
			}
			return result;
		}

		public static string GetNextUpgrade(string productId)
		{
			string currentUpgrade = GetCurrentUpgrade(productId);
			IAPObject iAPObject = GetIAPObject(currentUpgrade);
			if (!DBManager.isPurchased(currentUpgrade) || string.IsNullOrEmpty(iAPObject.req.nextId))
			{
				return currentUpgrade;
			}
			return iAPObject.req.nextId;
		}

		public static string GetIAPIdentifier(string id)
		{
			foreach (IAPObject value in instance.IAPObjects.Values)
			{
				if (value.type != IAPType.consumableVirtual && value.type != IAPType.nonConsumableVirtual && value.GetIdentifier() == id)
				{
					return value.id;
				}
			}
			return id;
		}

		public static List<IAPCurrency> GetCurrency()
		{
			return instance.currency;
		}

		public static string[] GetIAPKeys()
		{
			string[] array = new string[instance.IAPObjects.Count];
			instance.IAPObjects.Keys.CopyTo(array, 0);
			return array;
		}

		public static IAPObject GetIAPObject(string id)
		{
			if (!instance || !instance.IAPObjects.ContainsKey(id))
			{
				return null;
			}
			return instance.IAPObjects[id];
		}

		public static string GetIAPObjectGroupName(string id)
		{
			if (instance.IAPObjects.ContainsKey(id))
			{
				IAPObject iAPObject = GetIAPObject(id);
				for (int i = 0; i < instance.IAPs.Count; i++)
				{
					if (instance.IAPs[i].items.Contains(iAPObject))
					{
						return instance.IAPs[i].name;
					}
				}
			}
			return null;
		}
	}
}
