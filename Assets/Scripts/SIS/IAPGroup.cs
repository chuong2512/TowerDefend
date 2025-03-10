using System;
using System.Collections.Generic;

namespace SIS
{
	[Serializable]
	public class IAPGroup
	{
		public string id;

		public string name;

		public List<IAPObject> items = new List<IAPObject>();
	}
}
