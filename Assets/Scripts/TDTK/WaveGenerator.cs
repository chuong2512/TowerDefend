using System;
using System.Collections.Generic;
using UnityEngine;

namespace TDTK
{
	[Serializable]
	public class WaveGenerator
	{
		public bool similarSubWave;

		public float waveSpacingTimeMin = 5f;

		public float waveSpacingTimeMax = 5f;

		public ProceduralVariable subWaveCount = new ProceduralVariable(1f, 4f);

		public ProceduralVariable unitCount = new ProceduralVariable(5f, 50f);

		public List<PathTD> pathList = new List<PathTD>();

		public bool utiliseAllPath = true;

		public List<ProceduralVariable> rscSettingList = new List<ProceduralVariable>();

		public List<ProceduralUnitSetting> unitSettingList = new List<ProceduralUnitSetting>();

		public void CheckPathList()
		{
			for (int i = 0; i < pathList.Count; i++)
			{
				if (pathList[i] == null)
				{
					pathList.RemoveAt(i);
					i--;
				}
			}
		}

		public void Init()
		{
			int resourceCount = ResourceManager.GetResourceCount();
			while (rscSettingList.Count < resourceCount)
			{
				rscSettingList.Add(new ProceduralVariable(0f, 0f));
			}
			while (rscSettingList.Count > resourceCount)
			{
				rscSettingList.RemoveAt(rscSettingList.Count - 1);
			}
			CheckPathList();
		}

		public Wave Generate(int waveID)
		{
			if (pathList.Count == 0)
			{
				UnityEngine.Debug.Log("no path at all");
				return null;
			}
			Wave wave = new Wave();
			wave.waveID = waveID;
			waveID++;
			int b = Mathf.Max(1, (int)subWaveCount.GetValueAtWave(waveID));
			int num = (int)unitCount.GetValueAtWave(waveID);
			b = Mathf.Min(num, b);
			List<ProceduralUnitSetting> list = new List<ProceduralUnitSetting>();
			int index = 0;
			float num2 = float.PositiveInfinity;
			for (int i = 0; i < unitSettingList.Count; i++)
			{
				if (unitSettingList[i].enabled)
				{
					if (unitSettingList[i].minWave <= waveID)
					{
						list.Add(unitSettingList[i]);
					}
					if (list.Count == 0 && (float)unitSettingList[i].minWave < num2)
					{
						num2 = unitSettingList[i].minWave;
						index = i;
					}
				}
			}
			if (list.Count == 0)
			{
				list.Add(unitSettingList[index]);
			}
			wave.subWaveList = new List<SubWave>();
			if (similarSubWave)
			{
				wave.subWaveList.Add(GenerateSubWave(waveID, 0, list));
				for (int j = 1; j < b; j++)
				{
					wave.subWaveList.Add(wave.subWaveList[0].Clone());
				}
			}
			else
			{
				for (int k = 0; k < b; k++)
				{
					wave.subWaveList.Add(GenerateSubWave(waveID, k, list));
				}
			}
			int num3 = UnityEngine.Random.Range(0, pathList.Count);
			List<int> list2 = new List<int>();
			for (int l = 0; l < wave.subWaveList.Count; l++)
			{
				int num4;
				for (num4 = num3 + ((UnityEngine.Random.Range(0f, 1f) > 0.75f) ? 1 : 0); num4 >= pathList.Count; num4 -= pathList.Count)
				{
				}
				if (utiliseAllPath && list2.Count < pathList.Count && list2.Contains(num4))
				{
					num4++;
					if (num4 >= pathList.Count)
					{
						num4 = 0;
					}
				}
				wave.subWaveList[l].path = pathList[num4];
				list2.Add(num4);
			}
			if (similarSubWave)
			{
				int count = (int)Mathf.Floor(num / wave.subWaveList.Count);
				for (int m = 0; m < wave.subWaveList.Count; m++)
				{
					wave.subWaveList[m].count = count;
				}
			}
			else
			{
				int num5 = num;
				while (num5 > 0)
				{
					for (int n = 0; n < b; n++)
					{
						if (wave.subWaveList[n].count == 0)
						{
							wave.subWaveList[n].count = 1;
							num5--;
							continue;
						}
						int a = UnityEngine.Random.Range(0, 3);
						a = Mathf.Min(a, num5);
						wave.subWaveList[n].count += a;
						num5 -= a;
					}
				}
			}
			wave.duration = wave.CalculateSpawnDuration() + UnityEngine.Random.Range(waveSpacingTimeMin, waveSpacingTimeMax);
			float num6 = 0f;
			for (int num7 = 0; num7 < b; num7++)
			{
				float pathDistance = wave.subWaveList[num7].path.GetPathDistance();
				float overrideMoveSpd = wave.subWaveList[num7].overrideMoveSpd;
				float num8 = pathDistance / overrideMoveSpd;
				if (num8 > num6)
				{
					num6 = num8;
				}
			}
			wave.duration += num6 * UnityEngine.Random.Range(0.5f, 0.8f);
			for (int num9 = 0; num9 < rscSettingList.Count; num9++)
			{
				wave.rscGainList.Add((int)rscSettingList[num9].GetValueAtWave(waveID));
			}
			return wave;
		}

		private SubWave GenerateSubWave(int waveID, int subWaveID, List<ProceduralUnitSetting> availableUnitList)
		{
			SubWave subWave = new SubWave();
			int index = UnityEngine.Random.Range(0, availableUnitList.Count);
			ProceduralUnitSetting proceduralUnitSetting = availableUnitList[index];
			subWave.unit = proceduralUnitSetting.unit.gameObject;
			subWave.count = 0;
			subWave.overrideHP = proceduralUnitSetting.HP.GetValueAtWave(waveID);
			subWave.overrideShield = proceduralUnitSetting.shield.GetValueAtWave(waveID);
			subWave.overrideMoveSpd = proceduralUnitSetting.speed.GetValueAtWave(waveID);
			subWave.interval = Mathf.Max(0.25f, proceduralUnitSetting.interval.GetValueAtWave(waveID));
			subWave.delay = (float)subWaveID * UnityEngine.Random.Range(1f, 3f);
			return subWave;
		}
	}
}
