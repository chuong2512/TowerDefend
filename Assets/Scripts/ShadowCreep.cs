using UnityEngine;

public class ShadowCreep : MonoBehaviour
{
	public GameObject FatherShadow;

	public float FlyHight = 1f;

	private void Start()
	{
	}

	private void Update()
	{
		Transform transform = base.gameObject.transform;
		Vector3 position = FatherShadow.transform.position;
		float x = position.x;
		Vector3 position2 = FatherShadow.transform.position;
		float y = position2.y;
		Vector3 position3 = FatherShadow.transform.position;
		transform.position = new Vector3(x, y, position3.z - FlyHight);
	}
}
