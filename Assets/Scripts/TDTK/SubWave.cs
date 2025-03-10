using System;
using System.Collections.Generic;
using UnityEngine;

namespace TDTK
{
	[Serializable]
	public class SubWave
	{
		public GameObject unit;

		public UnitCreep unitC;

		public int count = 1;

		public float interval = 1f;

		public float delay;

		public PathTD path;

		public float overrideHP = -1f;

		public float overrideShield = -1f;

		public float overrideMoveSpd = -1f;

		public int overrideLifeCost = -1;

		public int overrideScoreCost = -1;

		public int[] overrideValue = new int[0];

		public List<int> overrideValueMin = new List<int>();

		public List<int> overrideValueMax = new List<int>();

		public SubWave Clone()
		{
			SubWave subWave = new SubWave();
			subWave.unit = unit;
			subWave.unitC = unitC;
			subWave.count = count;
			subWave.interval = interval;
			subWave.delay = delay;
			subWave.path = path;
			subWave.overrideHP = overrideHP;
			subWave.overrideShield = overrideShield;
			subWave.overrideMoveSpd = overrideMoveSpd;
			subWave.overrideLifeCost = overrideLifeCost;
			subWave.overrideScoreCost = overrideScoreCost;
			subWave.overrideValueMin = new List<int>(overrideValueMin);
			subWave.overrideValueMax = new List<int>(overrideValueMax);
			return subWave;
		}
	}
}
