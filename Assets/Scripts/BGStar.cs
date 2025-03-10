using TDTK;
using UnityEngine;

public class BGStar : MonoBehaviour
{
	private void Start()
	{
		OnRefreshMainCamera();
	}

	private void OnEnable()
	{
		TDTK.TDTK.onFPSSwitchCameraE += OnRefreshMainCamera;
	}

	private void OnDisable()
	{
		TDTK.TDTK.onFPSSwitchCameraE -= OnRefreshMainCamera;
	}

	private void OnRefreshMainCamera()
	{
		Camera main = Camera.main;
		base.transform.parent = main.transform;
		base.transform.localPosition = Vector3.zero;
	}
}
