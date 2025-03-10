using UnityEngine;

public class tutorialScript : MonoBehaviour
{
	public GameObject TutorialCanvas;

	private int TutorialInt;

	private void Awake()
	{
		TutorialCanvas.SetActive(value: true);
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void DoneTutorial()
	{
		ES2.Save(1, "Tutorial");
		TutorialCanvas.SetActive(value: false);
	}
}
