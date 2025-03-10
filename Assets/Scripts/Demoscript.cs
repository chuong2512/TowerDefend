using SIS;
using UnityEngine;

public class Demoscript : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
	}

	public void okok()
	{
		DBManager.IncreaseFunds("coins", 45);
	}
}
