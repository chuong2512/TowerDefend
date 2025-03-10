using System;

namespace TDTK
{
	[Serializable]
	public class AbilityEffect
	{
		public float duration;

		public float damageMin;

		public float damageMax;

		public float stunChance;

		public Slow slow = new Slow();

		public Dot dot = new Dot();

		public float damageBuff;

		public float rangeBuff;

		public float cooldownBuff;

		public float HPGainMin;

		public float HPGainMax;

		public AbilityEffect Clone()
		{
			AbilityEffect abilityEffect = new AbilityEffect();
			abilityEffect.duration = duration;
			abilityEffect.damageMin = damageMin;
			abilityEffect.damageMax = damageMax;
			abilityEffect.stunChance = stunChance;
			abilityEffect.slow = slow.Clone();
			abilityEffect.dot = dot.Clone();
			abilityEffect.slow.duration = abilityEffect.duration;
			abilityEffect.dot.duration = abilityEffect.duration;
			abilityEffect.damageBuff = damageBuff;
			abilityEffect.rangeBuff = rangeBuff;
			abilityEffect.cooldownBuff = cooldownBuff;
			abilityEffect.HPGainMin = HPGainMin;
			abilityEffect.HPGainMax = HPGainMax;
			return abilityEffect;
		}
	}
}
