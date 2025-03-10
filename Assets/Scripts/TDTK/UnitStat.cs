using System;
using System.Collections.Generic;
using UnityEngine;

namespace TDTK
{
	[Serializable]
	public class UnitStat
	{
		public float damageMin = 5f;

		public float damageMax = 6f;

		public float cooldown = 1f;

		public float clipSize = -1f;

		public float reloadDuration = 2f;

		public float range = 10f;

		[HideInInspector]
		public float minRange;

		public float aoeRadius;

		public float hit;

		public float shieldBreak;

		public float shieldPierce;

		public bool damageShieldOnly;

		public Critical crit;

		public Stun stun;

		public Slow slow;

		public Dot dot;

		public InstantKill instantKill;

		public Buff buff;

		public List<int> rscGain = new List<int>();

		public List<int> cost = new List<int>();

		public float buildDuration = 1f;

		public float unBuildDuration = 1f;

		public ShootObject shootObject;

		public GameObject effectObject;

		public bool autoDestroyEffect = true;

		public float effectDuration = 1.5f;

		public bool useCustomDesp;

		public string desp = string.Empty;

		public UnitStat()
		{
			stun = new Stun();
			crit = new Critical();
			slow = new Slow();
			dot = new Dot();
			instantKill = new InstantKill();
			buff = new Buff();
		}

		public UnitStat Clone()
		{
			UnitStat unitStat = new UnitStat();
			unitStat.damageMin = damageMin;
			unitStat.damageMax = damageMax;
			unitStat.cooldown = cooldown;
			unitStat.clipSize = clipSize;
			unitStat.reloadDuration = reloadDuration;
			unitStat.minRange = minRange;
			unitStat.range = range;
			unitStat.aoeRadius = aoeRadius;
			unitStat.hit = hit;
			unitStat.shieldBreak = shieldBreak;
			unitStat.shieldPierce = shieldPierce;
			unitStat.damageShieldOnly = damageShieldOnly;
			unitStat.crit = crit.Clone();
			unitStat.stun = stun.Clone();
			unitStat.slow = slow.Clone();
			unitStat.dot = dot.Clone();
			unitStat.instantKill = instantKill.Clone();
			unitStat.buff = buff.Clone();
			unitStat.buildDuration = buildDuration;
			unitStat.unBuildDuration = unBuildDuration;
			unitStat.shootObject = shootObject;
			unitStat.autoDestroyEffect = autoDestroyEffect;
			unitStat.effectDuration = effectDuration;
			unitStat.desp = desp;
			unitStat.rscGain = new List<int>(rscGain);
			unitStat.cost = new List<int>(cost);
			return unitStat;
		}
	}
}
