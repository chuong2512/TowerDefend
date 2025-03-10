using System;

namespace MadLevelManager.Backend
{
	public class DisplayedName : Attribute
	{
		public readonly string name;

		public DisplayedName(string name)
		{
			this.name = name;
		}
	}
}
