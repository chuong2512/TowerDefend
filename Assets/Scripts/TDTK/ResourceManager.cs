using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TDTK
{
	public class ResourceManager : MonoBehaviour
	{
		public bool carryFromLastScene;

		private static List<int> lastLevelValueList = new List<int>();

		private static List<int> initialValueList = new List<int>();

		public bool enableRscGen;

		public List<float> rscGenRateList = new List<float>();

		public List<int> startingValueList = new List<int>();

		[HideInInspector]
		public List<Rsc> rscList = new List<Rsc>();

		public static ResourceManager instance;

		public void Init()
		{
			instance = this;
			rscList = ResourceDB.LoadClone();
			if (carryFromLastScene)
			{
				for (int i = 0; i < lastLevelValueList.Count; i++)
				{
					rscList[i].value = lastLevelValueList[i];
				}
			}
			else
			{
				for (int j = 0; j < rscList.Count; j++)
				{
					if (j < startingValueList.Count)
					{
						rscList[j].value = startingValueList[j];
					}
				}
			}
			for (int k = 0; k < rscList.Count; k++)
			{
				initialValueList.Add(rscList[k].value);
			}
			if (enableRscGen)
			{
				StartCoroutine(RscGenRoutine());
			}
		}

		public static int GetResourceCount()
		{
			return instance.rscList.Count;
		}

		public static List<Rsc> GetResourceList()
		{
			return instance.rscList;
		}

		public static void OnGameOver(bool flag)
		{
			instance._OnGameOver(flag);
		}

		public void _OnGameOver(bool flag)
		{
			if (flag)
			{
				lastLevelValueList = new List<int>();
				for (int i = 0; i < rscList.Count; i++)
				{
					lastLevelValueList.Add(rscList[i].value);
				}
			}
		}

		public static void OnRestartLevel()
		{
			lastLevelValueList = new List<int>(initialValueList);
		}

		private IEnumerator RscGenRoutine()
		{
			List<float> temp = new List<float>();
			for (int i = 0; i < rscList.Count; i++)
			{
				temp.Add(0f);
			}
			while (true)
			{
				yield return new WaitForSeconds(1f);
				List<float> perkRegenRate = PerkManager.GetRscRegen();
				List<int> valueList = new List<int>();
				bool increased = false;
				for (int j = 0; j < rscList.Count; j++)
				{
					int index;
					List<float> list;
					(list = temp)[index = j] = list[index] + (rscGenRateList[j] + perkRegenRate[j]);
					valueList.Add(0);
					if (temp[j] >= 1f)
					{
						while (temp[j] >= 1f)
						{
							List<int> list2;
							int index2;
							(list2 = valueList)[index2 = j] = list2[index2] + 1;
							int index3;
							(list = temp)[index3 = j] = list[index3] - 1f;
						}
						increased = true;
					}
				}
				if (increased)
				{
					GainResource(valueList);
				}
			}
		}

		public static int HasSufficientResource(List<int> rscL)
		{
			return instance._HasSufficientResource(rscL);
		}

		public int _HasSufficientResource(List<int> rscL)
		{
			if (rscList.Count != rscL.Count)
			{
				UnityEngine.Debug.Log("error, resource number doesnt match!    " + rscList.Count + "     " + rscL.Count);
				return -99;
			}
			for (int i = 0; i < rscList.Count; i++)
			{
				if (rscList[i].value < rscL[i])
				{
					return i;
				}
			}
			return -1;
		}

		public static void SpendResource(List<int> rscL)
		{
			instance._GainResource(rscL, null, useMul: false, -1f);
		}

		public static void GainResource(List<int> rscL, List<float> mulL = null, bool useMul = true)
		{
			instance._GainResource(rscL, mulL, useMul);
		}

		public void _GainResource(List<int> rscL, List<float> mulL = null, bool useMul = true, float sign = 1f)
		{
			if (rscList.Count != rscL.Count)
			{
				return;
			}
			if (sign == 1f && useMul)
			{
				List<float> rscGain = PerkManager.GetRscGain();
				if (mulL != null)
				{
					for (int i = 0; i < rscGain.Count; i++)
					{
						List<float> list;
						int index;
						(list = rscGain)[index = i] = list[index] + mulL[i];
					}
				}
				for (int j = 0; j < rscGain.Count; j++)
				{
					rscL[j] = (int)((float)rscL[j] * (1f + rscGain[j]));
				}
			}
			if (sign == -1f)
			{
				for (int k = 0; k < rscL.Count; k++)
				{
					rscL[k] = (int)((float)rscL[k] * sign);
				}
			}
			for (int l = 0; l < rscList.Count; l++)
			{
				rscList[l].value = Mathf.Max(0, rscList[l].value + rscL[l]);
			}
			TDTK.OnResource(rscL);
		}

		public static void SetResourceValue(List<int> rscL)
		{
			instance._SetResourceValue(rscL);
		}

		public void _SetResourceValue(List<int> rscL)
		{
			if (rscList.Count != rscL.Count)
			{
				UnityEngine.Debug.Log("error, resource number doesnt match!");
				return;
			}
			for (int i = 0; i < rscList.Count; i++)
			{
				rscList[i].value = Mathf.Max(0, rscL[i]);
			}
		}

		public static void NewSceneNotification()
		{
		}

		public static void ResetCummulatedResource()
		{
		}
	}
}
