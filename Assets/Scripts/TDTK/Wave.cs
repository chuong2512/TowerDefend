using System;
using System.Collections.Generic;
using UnityEngine;

namespace TDTK
{
	[Serializable]
	public class Wave
	{
		[HideInInspector]
		public int waveID = -1;

		public List<SubWave> subWaveList = new List<SubWave>();

		public int lifeGain;

		public int energyGain;

		public int scoreGain = 100;

		public List<int> rscGainList = new List<int>();

		public int activeUnitCount;

		[HideInInspector]
		public bool spawned;

		[HideInInspector]
		public bool cleared;

		public float duration = 10f;

		public int subWaveSpawnedCount;

		public Wave()
		{
			subWaveList.Add(new SubWave());
		}

		public float CalculateSpawnDuration()
		{
			float num = 0f;
			for (int i = 0; i < subWaveList.Count; i++)
			{
				SubWave subWave = subWaveList[i];
				float num2 = (float)(subWave.count - 1) * subWave.interval + subWave.delay;
				if (num2 > num)
				{
					num = num2;
				}
			}
			return num;
		}

		public Wave Clone()
		{
			Wave wave = new Wave();
			wave.duration = duration;
			wave.scoreGain = scoreGain;
			for (int i = 0; i < subWaveList.Count; i++)
			{
				wave.subWaveList.Add(subWaveList[i].Clone());
			}
			wave.rscGainList = new List<int>(rscGainList);
			return wave;
		}
	}
}
