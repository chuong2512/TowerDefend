using System;
using UnityEngine;

namespace TDTK
{
	[Serializable]
	public class InstantKill
	{
		public float chance;

		public float HPThreshold = 0.3f;

		public InstantKill(float c = 0f, float th = 0f)
		{
			chance = c;
			HPThreshold = th;
		}

		public bool IsValid()
		{
			if (HPThreshold > 0f && chance > 0f)
			{
				return true;
			}
			return false;
		}

		public bool IsApplicable(float HP, float fullHP)
		{
			if (HP / fullHP <= HPThreshold && UnityEngine.Random.Range(0f, 1f) < chance)
			{
				return true;
			}
			return false;
		}

		public InstantKill Clone()
		{
			InstantKill instantKill = new InstantKill();
			instantKill.chance = chance;
			instantKill.HPThreshold = HPThreshold;
			return instantKill;
		}
	}
}
