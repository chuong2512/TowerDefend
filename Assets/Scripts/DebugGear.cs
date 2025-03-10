using UnityEngine;

public class DebugGear : MonoBehaviour
{
	public int LevelNum = 10;

	public int NumSum = 3;

	public int StartNum = 12;

	public float SoNhan = 1f;

	private void Start()
	{
		for (int i = 0; i < LevelNum; i++)
		{
			UnityEngine.Debug.Log((float)(NumSum * i) * SoNhan + (float)UnityEngine.Random.Range(StartNum - 2, StartNum + 2));
		}
	}

	private void Update()
	{
	}
}
