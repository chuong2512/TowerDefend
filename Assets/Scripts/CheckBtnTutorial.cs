using MadLevelManager;
using UnityEngine;

public class CheckBtnTutorial : MonoBehaviour
{
	public GameObject ButtonLaser;

	public GameObject ButtonBomb;

	public GameObject ButtonSunRay;

	public GameObject ButtonBeam;

	private int NumUnlock;

	private void Start()
	{
		if (CheckLevelComplete() >= 4 && CheckLevelComplete() < 7)
		{
			NumUnlock = 1;
		}
		else if (CheckLevelComplete() >= 7 && CheckLevelComplete() < 10)
		{
			NumUnlock = 2;
		}
		else if (CheckLevelComplete() >= 10 && CheckLevelComplete() < 15)
		{
			NumUnlock = 3;
		}
		else if (CheckLevelComplete() >= 15 && CheckLevelComplete() < 50)
		{
			NumUnlock = 4;
		}
		else if (CheckLevelComplete() >= 0 && CheckLevelComplete() < 4)
		{
			NumUnlock = 0;
		}
		if (NumUnlock == 0)
		{
			ButtonLaser.SetActive(value: false);
			ButtonBomb.SetActive(value: false);
			ButtonSunRay.SetActive(value: false);
			ButtonBeam.SetActive(value: false);
		}
		else if (NumUnlock == 1)
		{
			ButtonLaser.SetActive(value: true);
			ButtonBomb.SetActive(value: false);
			ButtonSunRay.SetActive(value: false);
			ButtonBeam.SetActive(value: false);
		}
		else if (NumUnlock == 2)
		{
			ButtonLaser.SetActive(value: true);
			ButtonBomb.SetActive(value: true);
			ButtonSunRay.SetActive(value: false);
			ButtonBeam.SetActive(value: false);
		}
		else if (NumUnlock == 3)
		{
			ButtonLaser.SetActive(value: true);
			ButtonBomb.SetActive(value: true);
			ButtonSunRay.SetActive(value: true);
			ButtonBeam.SetActive(value: false);
		}
		else if (NumUnlock == 4)
		{
			ButtonLaser.SetActive(value: true);
			ButtonBomb.SetActive(value: true);
			ButtonSunRay.SetActive(value: true);
			ButtonBeam.SetActive(value: true);
		}
	}

	private int CheckLevelComplete()
	{
		string text = MadLevel.FindLastUnlockedLevelName();
		return int.Parse(text.Substring(6));
	}
}
