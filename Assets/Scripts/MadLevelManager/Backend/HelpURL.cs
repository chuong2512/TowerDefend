using System;

namespace MadLevelManager.Backend
{
	public class HelpURL : Attribute
	{
		public readonly string url;

		public HelpURL(string url)
		{
			this.url = url;
		}
	}
}
