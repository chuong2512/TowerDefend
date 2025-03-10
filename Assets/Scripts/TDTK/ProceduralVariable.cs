using System;
using UnityEngine;

namespace TDTK
{
	[Serializable]
	public class ProceduralVariable
	{
		public float startValue = 5f;

		public float incMultiplier = 1f;

		public float devMultiplier = 0.2f;

		public float minValue = 1f;

		public float maxValue = 50f;

		public ProceduralVariable()
		{
		}

		public ProceduralVariable(float val1, float val2)
		{
			startValue = val1;
			maxValue = val2;
		}

		public float GetValueAtWave(int waveID)
		{
			float value = (incMultiplier * (float)waveID + startValue) * (1f + UnityEngine.Random.Range(0f - devMultiplier, devMultiplier));
			return Mathf.Clamp(value, minValue, maxValue);
		}
	}
}
