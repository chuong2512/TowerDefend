using System;

namespace TDTK
{
	[Serializable]
	public class Slow
	{
		public int effectID;

		public float duration;

		public float slowMultiplier = 1f;

		public Slow(float s = 0f, float dur = 0f)
		{
			slowMultiplier = s;
			duration = dur;
		}

		public bool IsValid()
		{
			return (!(duration <= 0f) && !(slowMultiplier <= 0f)) ? true : false;
		}

		public Slow Clone()
		{
			Slow slow = new Slow();
			slow.effectID = effectID;
			slow.slowMultiplier = slowMultiplier;
			slow.duration = duration;
			return slow;
		}
	}
}
