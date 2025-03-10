using UnityEngine;

public class RscTowerPump : MonoBehaviour
{
	public float speed = 5f;

	public float mag = 1f;

	private Vector3 origin;

	private Transform thisT;

	private void Start()
	{
		thisT = base.transform;
		origin = thisT.localPosition;
	}

	private void Update()
	{
		thisT.localPosition = origin - new Vector3(0f, mag * Mathf.Abs(Mathf.Sin(Time.time * speed)), 0f);
	}
}
