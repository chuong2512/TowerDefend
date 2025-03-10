using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityIndicator : MonoBehaviour
{
	public List<ParticleSystem> particleList = new List<ParticleSystem>();

	private void OnEnable()
	{
		StartCoroutine(DelayedEnable());
	}

	private IEnumerator DelayedEnable()
	{
		yield return null;
		for (int i = 0; i < particleList.Count; i++)
		{
			particleList[i].Clear();
			ParticleSystem particleSystem = particleList[i];
			Vector3 localScale = base.transform.localScale;
			particleSystem.startSize = localScale.x * 1.75f;
			particleList[i].Play();
		}
	}

	private void OnDisable()
	{
		for (int i = 0; i < particleList.Count; i++)
		{
			particleList[i].Stop();
		}
	}
}
