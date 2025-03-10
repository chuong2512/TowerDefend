using MadLevelManager;
using UnityEngine;

public class MadLevelLoadingBar : MonoBehaviour
{
	private MadLevelLoadingScreen loadingScreen;

	private MadSprite bar;

	private void Start()
	{
		loadingScreen = (UnityEngine.Object.FindObjectOfType(typeof(MadLevelLoadingScreen)) as MadLevelLoadingScreen);
		bar = GetComponent<MadSprite>();
	}

	private void Update()
	{
		bar.fillValue = loadingScreen.progress;
	}
}
