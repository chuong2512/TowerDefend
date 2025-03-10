using System;

namespace TDTK
{
	[Serializable]
	public class Dot
	{
		public int effectID;

		public float duration;

		public float interval;

		public float value;

		public Dot(float dur = 0f, float i = 0f, float val = 0f)
		{
			duration = dur;
			interval = i;
			value = val;
		}

		public float GetTotalDamage()
		{
			return duration / interval * value;
		}

		public Dot Clone()
		{
			Dot dot = new Dot();
			dot.effectID = effectID;
			dot.duration = duration;
			dot.interval = interval;
			dot.value = value;
			return dot;
		}
	}
}
