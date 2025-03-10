using System;
using UnityEngine;

namespace SIS
{
	[Serializable]
	public class ShopContainer
	{
		public string id;

		public GameObject prefab;

		public IAPContainer parent;
	}
}
