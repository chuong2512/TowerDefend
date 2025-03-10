using System.Collections;
using UnityEngine;

public class RandomRotate : MonoBehaviour
{
	public _Axis rotateAxis;

	public float min = -30f;

	public float max = 30f;

	private Quaternion targetRot;

	private float rotateSpeed;

	private void Start()
	{
		StartCoroutine(RotateRoutine());
	}

	private IEnumerator RotateRoutine()
	{
		yield return new WaitForSeconds(UnityEngine.Random.Range(1f, 5f));
		while (true)
		{
			rotateSpeed = UnityEngine.Random.Range(3, 6);
			float val = UnityEngine.Random.Range(min, max);
			if (rotateAxis == _Axis.X)
			{
				targetRot = Quaternion.Euler(val, 0f, 0f);
			}
			else if (rotateAxis == _Axis.Y)
			{
				targetRot = Quaternion.Euler(0f, val, 0f);
			}
			else if (rotateAxis == _Axis.Z)
			{
				targetRot = Quaternion.Euler(0f, 0f, val);
			}
			yield return new WaitForSeconds(UnityEngine.Random.Range(3f, 5f));
		}
	}

	private void Update()
	{
		base.transform.localRotation = Quaternion.Slerp(base.transform.localRotation, targetRot, Time.deltaTime * rotateSpeed);
	}
}
