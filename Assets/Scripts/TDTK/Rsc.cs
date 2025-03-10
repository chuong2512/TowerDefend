using System;

namespace TDTK
{
	[Serializable]
	public class Rsc : TDTKItem
	{
		public int value;

		public Rsc Clone()
		{
			Rsc rsc = new Rsc();
			rsc.ID = ID;
			rsc.name = name;
			rsc.icon = icon;
			rsc.value = value;
			return rsc;
		}

		public bool IsMatch(Rsc rsc)
		{
			if (rsc.ID != ID)
			{
				return false;
			}
			if (rsc.name != name)
			{
				return false;
			}
			if (rsc.icon != icon)
			{
				return false;
			}
			return true;
		}
	}
}
