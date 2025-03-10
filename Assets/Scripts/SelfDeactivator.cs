using System.Collections;
using UnityEngine;

public class SelfDeactivator : MonoBehaviour
{
	public bool useObjectPool = true;

	public float duration = 1f;

	private void OnEnable()
	{
		StartCoroutine(DeactivateRoutine());
	}

	private IEnumerator DeactivateRoutine()
	{
		yield return new WaitForSeconds(duration);
		if (useObjectPool)
		{
			ObjectPoolManager.Unspawn(base.gameObject);
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
