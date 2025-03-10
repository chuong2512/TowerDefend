using System.Collections;
using UnityEngine;

public class LaserFade : MonoBehaviour
{
	private LineRenderer lineR;

	public Vector2 uvAnimationRate = new Vector2(1f, 0f);

	private Vector2 uvOffset = Vector2.zero;

	private void Awake()
	{
		lineR = base.gameObject.GetComponent<LineRenderer>();
	}

	private void OnEnable()
	{
		StartCoroutine(Fade());
	}

	private IEnumerator Fade()
	{
		float duration = 0f;
		lineR.materials[0].SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0.5f));
		while (duration < 1f)
		{
			lineR.materials[0].SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, (1f - duration) / 2f));
			duration += Time.fixedDeltaTime * 4f;
			yield return new WaitForSeconds(Time.fixedDeltaTime);
		}
		lineR.materials[0].SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0f));
	}

	private void Update()
	{
		uvOffset += uvAnimationRate * Time.deltaTime;
		lineR.materials[0].SetTextureOffset("_MainTex", uvOffset);
	}
}
