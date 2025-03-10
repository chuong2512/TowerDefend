using UnityEngine;

public class RotateYaxis : MonoBehaviour
{
	public float speed = 5f;

	private Transform thisT;

	private void Start()
	{
		thisT = base.transform;
	}

	private void Update()
	{
		thisT.Rotate(Vector3.up * speed * Time.deltaTime * 35f, Space.World);
	}
}
