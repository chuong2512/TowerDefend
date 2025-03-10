using UnityEngine;

public class Tutorial2 : MonoBehaviour
{
	public GameObject[] ListTT;

	private int ClickCount;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void BtnFc()
	{
		ClickCount++;
		UnityEngine.Debug.Log(ClickCount);
		switch (ClickCount)
		{
		case 1:
			ListTT[0].SetActive(value: false);
			ListTT[1].SetActive(value: true);
			break;
		case 2:
			ListTT[1].SetActive(value: false);
			ListTT[2].SetActive(value: true);
			break;
		case 3:
			ListTT[2].SetActive(value: false);
			ListTT[3].SetActive(value: true);
			ListTT[16].SetActive(value: true);
			break;
		case 4:
			ListTT[3].SetActive(value: false);
			ListTT[4].SetActive(value: true);
			break;
		case 5:
			ListTT[4].SetActive(value: false);
			ListTT[5].SetActive(value: true);
			break;
		case 6:
			ListTT[5].SetActive(value: false);
			ListTT[6].SetActive(value: true);
			break;
		case 7:
			ListTT[6].SetActive(value: false);
			ListTT[7].SetActive(value: true);
			break;
		case 8:
			ListTT[7].SetActive(value: false);
			ListTT[8].SetActive(value: true);
			break;
		case 9:
			ListTT[8].SetActive(value: false);
			ListTT[9].SetActive(value: true);
			break;
		case 10:
			ListTT[9].SetActive(value: false);
			ListTT[10].SetActive(value: true);
			break;
		case 11:
			ListTT[10].SetActive(value: false);
			ListTT[11].SetActive(value: true);
			break;
		case 12:
			ListTT[11].SetActive(value: false);
			ListTT[12].SetActive(value: true);
			break;
		case 13:
			ListTT[12].SetActive(value: false);
			ListTT[13].SetActive(value: true);
			break;
		case 14:
			ListTT[13].SetActive(value: false);
			ListTT[16].SetActive(value: false);
			ListTT[3].SetActive(value: true);
			ListTT[14].SetActive(value: true);
			break;
		case 15:
			ListTT[15].SetActive(value: false);
			ES2.Save(1, "Tutorial");
			break;
		}
	}
}
