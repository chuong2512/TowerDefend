using UnityEngine;

public class changLineMaterial : MonoBehaviour
{
	private LineRenderer lineR4;

	public Material[] LMaterial;

	private float LineTime = 0.2f;

	private void Awake()
	{
		lineR4 = base.gameObject.GetComponent<LineRenderer>();
	}

	private void Start()
	{
	}

	private void OnEnable()
	{
		LineTime -= Time.fixedDeltaTime;
		if (LineTime <= 0f)
		{
			ChangeLine();
		}
	}

	public void ChangeLine()
	{
		lineR4.materials[0] = LMaterial[Random.Range(0, LMaterial.Length)];
		LineTime = 0.2f;
	}
}
