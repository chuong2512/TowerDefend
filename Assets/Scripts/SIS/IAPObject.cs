using System;
using System.Collections.Generic;
using UnityEngine;

namespace SIS
{
	[Serializable]
	public class IAPObject
	{
		public string id;

		public List<IAPIdentifier> localIDs = new List<IAPIdentifier>();

		public bool fetch;

		public IAPType type;

		public string title;

		public string description;

		public string realPrice;

		public Sprite icon;

		public List<IAPCurrency> virtualPrice = new List<IAPCurrency>();

		public IAPRequirement req = new IAPRequirement();

		public bool isVirtual;

		public bool platformFoldout;

		public string GetIdentifier()
		{
			string text = null;
			switch (Application.platform)
			{
			case RuntimePlatform.Android:
				text = localIDs[0].GetIdentifier();
				break;
			case RuntimePlatform.IPhonePlayer:
				text = localIDs[1].GetIdentifier();
				break;
			}
			if (!string.IsNullOrEmpty(text))
			{
				return text;
			}
			return id;
		}
	}
}
