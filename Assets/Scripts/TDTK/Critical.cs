using System;

namespace TDTK
{
	[Serializable]
	public class Critical
	{
		public float chance;

		public float dmgMultiplier;

		public Critical Clone()
		{
			Critical critical = new Critical();
			critical.chance = chance;
			critical.dmgMultiplier = dmgMultiplier;
			return critical;
		}
	}
}
