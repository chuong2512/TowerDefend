using UnityEngine;

public class Hover : MonoBehaviour
{
	public float offset;

	private void Start()
	{
		offset = UnityEngine.Random.Range(-5f, 5f);
	}

	private void Update()
	{
		base.transform.Translate(Vector3.up * 0.018f * Mathf.Sin(6.5f * Time.time + offset) * Time.deltaTime);
	}
}
