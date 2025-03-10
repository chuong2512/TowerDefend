using System;

namespace SIS
{
	[Serializable]
	public class IAPIdentifier
	{
		public bool overridden;

		public string id;

		public string GetIdentifier()
		{
			if (overridden)
			{
				return id;
			}
			return null;
		}
	}
}
