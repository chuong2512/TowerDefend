using System;
using UnityEngine;

namespace TDTK
{
	[Serializable]
	public class Stun
	{
		public float chance;

		public float duration;

		public Stun(float c = 0f, float dur = 0f)
		{
			chance = c;
			duration = dur;
		}

		public bool IsValid()
		{
			if (duration > 0f && chance > 0f)
			{
				return true;
			}
			return false;
		}

		public bool IsApplicable()
		{
			if (duration > 0f && UnityEngine.Random.Range(0f, 1f) < chance)
			{
				return true;
			}
			return false;
		}

		public Stun Clone()
		{
			Stun stun = new Stun();
			stun.chance = chance;
			stun.duration = duration;
			return stun;
		}
	}
}
