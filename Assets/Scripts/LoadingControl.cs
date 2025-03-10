using MadLevelManager;
using System.Collections;
using UnityEngine;

public class LoadingControl : MonoBehaviour
{
	public static string _LevelName;

	private void Start()
	{
		StartCoroutine(load(_LevelName));
	}

	private IEnumerator load(string LVname)
	{
		yield return new WaitForSeconds(0.2f);
		MadLevel.LoadLevelByNameAsync(LVname);
	}
}
