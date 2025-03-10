using System;

namespace TDTK
{
	[Serializable]
	public class Buff
	{
		public int effectID;

		public float damageBuff;

		public float cooldownBuff;

		public float rangeBuff;

		public float criticalBuff;

		public float hitBuff;

		public float dodgeBuff;

		public float regenHP;

		public Buff Clone()
		{
			Buff buff = new Buff();
			buff.effectID = effectID;
			buff.damageBuff = damageBuff;
			buff.cooldownBuff = cooldownBuff;
			buff.rangeBuff = rangeBuff;
			buff.criticalBuff = criticalBuff;
			buff.hitBuff = hitBuff;
			buff.dodgeBuff = dodgeBuff;
			buff.regenHP = regenHP;
			return buff;
		}
	}
}
