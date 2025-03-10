using System.Collections;
using UnityEngine;

public class Blink_MGMuzzle : MonoBehaviour
{
	private float posOffset = 0.025f;

	private Renderer ren;

	private Transform thisT;

	private void Start()
	{
		thisT = base.transform;
		ren = base.transform.GetComponent<Renderer>();
	}

	private void OnEnable()
	{
		if (ren != null)
		{
			ren.enabled = true;
		}
		StartCoroutine(Blinking());
	}

	private IEnumerator Blinking()
	{
		while (true)
		{
			if (ren == null)
			{
				yield return null;
				continue;
			}
			float x = UnityEngine.Random.Range(0f - posOffset, posOffset);
			float y = UnityEngine.Random.Range(0f - posOffset, posOffset);
			float z = UnityEngine.Random.Range(0f - posOffset, posOffset);
			thisT.localPosition = new Vector3(0f, 0f, 0.5f) + new Vector3(x, y, z);
			ren.enabled = !ren.enabled;
			yield return new WaitForSeconds(0.05f);
		}
	}
}
