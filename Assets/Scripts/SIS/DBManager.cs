using SimpleJSON;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using UnityEngine;

namespace SIS
{
	public class DBManager : MonoBehaviour
	{
		private string prefsKey = "data";

		private string remoteKey = "remote";

		private string idPrefix = "SIS_";

		private bool keepLegacy = true;

		public bool encrypt;

		public string obfuscKey;

		private JSONNode gameData;

		private string currency = "Currency";

		private string content = "Content";

		private string selected = "Selected";

		private string player = "Player";

		private static DBManager instance;

		public static event Action updatedDataEvent;

		public void Init()
		{
			instance = this;
			InitDB();
		}

		private void InitDB()
		{
			gameData = new JSONClass();
			if (PlayerPrefs.HasKey(prefsKey))
			{
				string @string = PlayerPrefs.GetString(prefsKey);
				if (encrypt)
				{
					gameData = JSON.Parse(Decrypt(@string));
				}
				else
				{
					gameData = JSON.Parse(@string);
				}
			}
			string[] array = new string[0];
			if (!IAPManager.GetInstance())
			{
				return;
			}
			array = IAPManager.GetIAPKeys();
			if (!keepLegacy)
			{
				string[] array2 = new string[gameData[content].Count];
				gameData[content].AsObject.Keys.CopyTo(array2, 0);
				for (int i = 0; i < array2.Length; i++)
				{
					string iAPIdentifier = IAPManager.GetIAPIdentifier(array2[i]);
					IAPObject iAPObject = IAPManager.GetIAPObject(iAPIdentifier);
					if (iAPObject != null && iAPObject.type != 0 && iAPObject.type != IAPType.consumableVirtual)
					{
						continue;
					}
					gameData[content].Remove(iAPIdentifier);
					for (int j = 0; j < gameData[selected].Count; j++)
					{
						if (gameData[selected][j].ToString().Contains(iAPIdentifier))
						{
							gameData[selected][j].Remove(iAPIdentifier);
						}
					}
				}
				array2 = new string[gameData[currency].Count];
				gameData[currency].AsObject.Keys.CopyTo(array2, 0);
				List<IAPCurrency> list = IAPManager.GetCurrency();
				List<string> list2 = new List<string>();
				for (int k = 0; k < list.Count; k++)
				{
					list2.Add(list[k].name);
				}
				foreach (string text in array2)
				{
					if (!list2.Contains(text))
					{
						gameData[currency].Remove(text);
					}
				}
			}
			foreach (string text2 in array)
			{
				IAPObject iAPObject2 = IAPManager.GetIAPObject(text2);
				if (!string.IsNullOrEmpty(gameData[content][text2]))
				{
					continue;
				}
				if (iAPObject2.type == IAPType.nonConsumable || iAPObject2.type == IAPType.subscription)
				{
					gameData[content][text2].AsBool = false;
				}
				else
				{
					if (iAPObject2.type != IAPType.nonConsumableVirtual)
					{
						continue;
					}
					bool asBool = true;
					for (int n = 0; n < iAPObject2.virtualPrice.Count; n++)
					{
						if (iAPObject2.virtualPrice[n].amount > 0)
						{
							asBool = false;
							break;
						}
					}
					gameData[content][text2].AsBool = asBool;
				}
			}
			List<IAPCurrency> list3 = IAPManager.GetCurrency();
			for (int num = 0; num < list3.Count; num++)
			{
				string name = list3[num].name;
				if (string.IsNullOrEmpty(name))
				{
					UnityEngine.Debug.LogError("Found Currency in IAP Settings without a name. The database will not know how to save it. Aborting.");
					return;
				}
				if (string.IsNullOrEmpty(gameData[currency][name]))
				{
					gameData[currency][name].AsInt = list3[num].amount;
				}
			}
			Save();
		}

		public static DBManager GetInstance()
		{
			return instance;
		}

		public static void SetToPurchased(string id)
		{
			string aKey = instance.content;
			instance.gameData[aKey][id].AsBool = true;
			Save();
		}

		public static void RemovePurchased(string id)
		{
			string aKey = instance.content;
			instance.gameData[aKey][id].AsBool = false;
			Save();
		}

		public static bool isPurchased(string id)
		{
			string aKey = instance.content;
			if (instance.gameData[aKey][id] != null)
			{
				return instance.gameData[aKey][id].AsBool;
			}
			return false;
		}

		public static bool isRequirementMet(IAPRequirement req)
		{
			string aKey = instance.player;
			if (instance.gameData[aKey][req.entry] != null && instance.gameData[aKey][req.entry].AsInt >= req.target)
			{
				return true;
			}
			return false;
		}

		public static void SaveReceipt(string id, string data)
		{
			string text = instance.idPrefix + id;
			if (instance.encrypt)
			{
				text = instance.Encrypt(text);
				data = instance.Encrypt(data);
			}
			PlayerPrefs.SetString(text, data);
			PlayerPrefs.Save();
		}

		public static string GetReceipt(string id)
		{
			string text = instance.idPrefix + id;
			if (instance.encrypt)
			{
				text = instance.Encrypt(text);
			}
			string text2 = PlayerPrefs.GetString(text, string.Empty);
			if (instance.encrypt)
			{
				text2 = instance.Decrypt(text2);
			}
			return text2;
		}

		public static bool VerifyVirtualPurchase(IAPObject obj)
		{
			Dictionary<string, int> allCurrencies = GetAllCurrencies();
			for (int i = 0; i < obj.virtualPrice.Count; i++)
			{
				IAPCurrency iAPCurrency = obj.virtualPrice[i];
				if (allCurrencies.ContainsKey(iAPCurrency.name) && iAPCurrency.amount > allCurrencies[iAPCurrency.name])
				{
					return false;
				}
			}
			string aKey = instance.currency;
			for (int j = 0; j < obj.virtualPrice.Count; j++)
			{
				IAPCurrency iAPCurrency2 = obj.virtualPrice[j];
				if (allCurrencies.ContainsKey(iAPCurrency2.name))
				{
					instance.gameData[aKey][iAPCurrency2.name].AsInt -= iAPCurrency2.amount;
				}
			}
			Save();
			return true;
		}

		public static void SetPlayerData(string id, JSONData data)
		{
			string aKey = instance.player;
			instance.gameData[aKey][id] = data;
			Save();
		}

		public static int IncrementPlayerData(string id, int value)
		{
			string aKey = instance.player;
			int num = instance.gameData[aKey][id].AsInt + value;
			instance.gameData[aKey][id].AsInt = num;
			Save();
			return num;
		}

		public static JSONNode GetPlayerData(string id)
		{
			string aKey = instance.player;
			return instance.gameData[aKey][id];
		}

		public static void RemovePlayerData(string id)
		{
			string aKey = instance.player;
			instance.gameData[aKey].Remove(id);
			Save();
		}

		public static void IncreaseFunds(string currency, int value)
		{
			string aKey = instance.currency;
			if (instance.gameData[aKey].Count == 0)
			{
				UnityEngine.Debug.LogError("Couldn't increase funds, no currency specified.");
				return;
			}
			if (string.IsNullOrEmpty(instance.gameData[aKey][currency]))
			{
				UnityEngine.Debug.LogError("Couldn't increase funds, currency: '" + currency + "' not found.");
				return;
			}
			JSONNode jSONNode = instance.gameData[aKey][currency];
			jSONNode.AsInt += value;
			if (jSONNode.AsInt < 0)
			{
				jSONNode.AsInt = 0;
			}
			Save();
		}

		public static void SetFunds(string currency, int value)
		{
			string aKey = instance.currency;
			instance.gameData[aKey][currency].AsInt = value;
			Save();
		}

		public static int GetFunds(string currency)
		{
			string aKey = instance.currency;
			int result = 0;
			if (instance.gameData[aKey].Count == 0)
			{
				UnityEngine.Debug.LogError("Couldn't increase funds, no currency specified.");
			}
			else if (string.IsNullOrEmpty(instance.gameData[aKey][currency]))
			{
				UnityEngine.Debug.LogError("Couldn't get funds, currency: '" + currency + "' not found.");
			}
			else
			{
				result = instance.gameData[aKey][currency].AsInt;
			}
			return result;
		}

		public static List<string> GetAllPurchased()
		{
			string aKey = instance.content;
			List<string> list = new List<string>();
			JSONNode jSONNode = instance.gameData[aKey];
			foreach (string key in jSONNode.AsObject.Keys)
			{
				if (jSONNode[key].AsBool && !(key != IAPManager.GetCurrentUpgrade(key)))
				{
					list.Add(key);
				}
			}
			return list;
		}

		public static Dictionary<string, int> GetAllCurrencies()
		{
			string aKey = instance.currency;
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			JSONNode jSONNode = instance.gameData[aKey];
			foreach (string key in jSONNode.AsObject.Keys)
			{
				dictionary.Add(key, jSONNode[key].AsInt);
			}
			return dictionary;
		}

		public static Dictionary<string, List<string>> GetAllSelected()
		{
			string aKey = instance.selected;
			Dictionary<string, List<string>> dictionary = new Dictionary<string, List<string>>();
			JSONNode jSONNode = instance.gameData[aKey];
			foreach (string key2 in jSONNode.AsObject.Keys)
			{
				string key = key2;
				if (!dictionary.ContainsKey(key))
				{
					dictionary.Add(key, new List<string>());
				}
				for (int i = 0; i < jSONNode[key2].Count; i++)
				{
					dictionary[key].Add(jSONNode[key2][i].Value);
				}
			}
			return dictionary;
		}

		public static bool SetToSelected(string id, bool single)
		{
			string aKey = instance.selected;
			string iAPObjectGroupName = IAPManager.GetIAPObjectGroupName(id);
			JSONNode jSONNode = instance.gameData[aKey][iAPObjectGroupName];
			if (jSONNode.ToString().Contains(id))
			{
				return false;
			}
			int count = jSONNode.Count;
			if (single)
			{
				for (int i = 0; i < count; i++)
				{
					instance.gameData[aKey][iAPObjectGroupName].Remove(i);
				}
				instance.gameData[aKey][iAPObjectGroupName][0] = id;
			}
			else
			{
				instance.gameData[aKey][iAPObjectGroupName][count] = id;
			}
			Save();
			return true;
		}

		public static void SetToDeselected(string id)
		{
			string aKey = instance.selected;
			string iAPObjectGroupName = IAPManager.GetIAPObjectGroupName(id);
			if (instance.gameData[aKey].ToString().Contains(id))
			{
				instance.gameData[aKey][iAPObjectGroupName].Remove(id);
				if (instance.gameData[aKey][iAPObjectGroupName].Count == 0)
				{
					instance.gameData[aKey].Remove(iAPObjectGroupName);
				}
				Save();
			}
		}

		public static bool isSelected(string id)
		{
			string aKey = instance.selected;
			if (instance.gameData[aKey].ToString().Contains(id))
			{
				return true;
			}
			return false;
		}

		public static void Save()
		{
			string text = instance.gameData.ToString();
			if (instance.encrypt)
			{
				text = instance.Encrypt(text);
			}
			PlayerPrefs.SetString(instance.prefsKey, text);
			PlayerPrefs.Save();
			if (DBManager.updatedDataEvent != null)
			{
				DBManager.updatedDataEvent();
			}
		}

		public static void SaveRemoteConfig(string data)
		{
			if (instance.encrypt)
			{
				data = instance.Encrypt(data);
			}
			PlayerPrefs.SetString(instance.remoteKey, data);
			PlayerPrefs.Save();
		}

		public static void LoadRemoteConfig()
		{
			if (PlayerPrefs.HasKey(instance.remoteKey))
			{
				string text = PlayerPrefs.GetString(instance.remoteKey);
				if (instance.encrypt)
				{
					text = instance.Decrypt(text);
				}
				ConvertToIAPs(text);
			}
		}

		public static void Clear(string data)
		{
			if (!(instance.gameData == null))
			{
				instance.gameData.Remove(data);
				Save();
			}
		}

		public static void ClearAll()
		{
			PlayerPrefs.DeleteKey(instance.prefsKey);
			PlayerPrefs.DeleteKey(instance.remoteKey);
			instance.gameData = null;
			string[] iAPKeys = IAPManager.GetIAPKeys();
			for (int i = 0; i < iAPKeys.Length; i++)
			{
				string text = instance.idPrefix + iAPKeys[i];
				if (instance.encrypt)
				{
					text = instance.Encrypt(text);
				}
				PlayerPrefs.DeleteKey(text);
			}
		}

		public static void ConvertToIAPs(string jsonText)
		{
			if (!string.IsNullOrEmpty(jsonText))
			{
				JSONNode jSONNode = new JSONClass();
				jSONNode = JSON.Parse(jsonText);
				foreach (string key in jSONNode.AsObject.Keys)
				{
					IAPObject iAPObject = IAPManager.GetIAPObject(key);
					if (iAPObject != null)
					{
						JSONNode jSONNode2 = jSONNode[key];
						if (!string.IsNullOrEmpty(jSONNode2["title"]))
						{
							iAPObject.title = jSONNode2["title"];
						}
						if (!string.IsNullOrEmpty(jSONNode2["description"]))
						{
							iAPObject.description = jSONNode2["description"];
						}
						if (!string.IsNullOrEmpty(jSONNode2["type"]))
						{
							string text = jSONNode2["type"];
							if (text != null && text == "consumable")
							{
								iAPObject.type = IAPType.consumableVirtual;
							}
							else
							{
								iAPObject.type = IAPType.nonConsumableVirtual;
							}
						}
						if (!string.IsNullOrEmpty(jSONNode2["virtualPrice"].ToString()))
						{
							JSONNode jSONNode3 = jSONNode2["virtualPrice"];
							List<IAPCurrency> list = new List<IAPCurrency>();
							for (int i = 0; i < jSONNode3.Count; i++)
							{
								IAPCurrency iAPCurrency = new IAPCurrency();
								iAPCurrency.name = jSONNode3[i]["name"];
								iAPCurrency.amount = jSONNode3[i]["amount"].AsInt;
								list.Add(iAPCurrency);
							}
							iAPObject.virtualPrice = list;
						}
						if (!string.IsNullOrEmpty(jSONNode2["requirement"].ToString()))
						{
							JSONNode jSONNode4 = jSONNode2["requirement"];
							iAPObject.req.entry = jSONNode4["entry"];
							iAPObject.req.labelText = jSONNode4["labelText"];
							iAPObject.req.target = jSONNode4["target"].AsInt;
						}
					}
				}
			}
		}

		private string Encrypt(string toEncrypt)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(obfuscKey);
			byte[] bytes2 = Encoding.UTF8.GetBytes(toEncrypt);
			byte[] array = null;
			DESCryptoServiceProvider dESCryptoServiceProvider = new DESCryptoServiceProvider();
			dESCryptoServiceProvider.Key = bytes;
			dESCryptoServiceProvider.Mode = CipherMode.ECB;
			dESCryptoServiceProvider.Padding = PaddingMode.PKCS7;
			ICryptoTransform cryptoTransform = dESCryptoServiceProvider.CreateEncryptor();
			array = cryptoTransform.TransformFinalBlock(bytes2, 0, bytes2.Length);
			return Convert.ToBase64String(array, 0, array.Length);
		}

		private string Decrypt(string toDecrypt)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(obfuscKey);
			byte[] array = Convert.FromBase64String(toDecrypt);
			byte[] array2 = null;
			DESCryptoServiceProvider dESCryptoServiceProvider = new DESCryptoServiceProvider();
			dESCryptoServiceProvider.Key = bytes;
			dESCryptoServiceProvider.Mode = CipherMode.ECB;
			dESCryptoServiceProvider.Padding = PaddingMode.PKCS7;
			ICryptoTransform cryptoTransform = dESCryptoServiceProvider.CreateDecryptor();
			array2 = cryptoTransform.TransformFinalBlock(array, 0, array.Length);
			return Encoding.UTF8.GetString(array2, 0, array2.Length);
		}
	}
}
