using System.Collections;
using TDTK;
using UnityEngine;
using UnityEngine.UI;

public class ScoreEndless : MonoBehaviour
{
	public Text TextWave;

	public Text TextGear;

	private void Awake()
	{
	}

	private void Start()
	{
		int currentWaveID = SpawnManager.GetCurrentWaveID();
		currentWaveID++;
		int num = currentWaveID;
		num *= 2;
		TextWave.text = currentWaveID.ToString();
		TextGear.text = "You Got: " + num.ToString();
		if (ES2.Exists("Gear"))
		{
			int num2 = ES2.Load<int>("Gear");
			num2 += num;
			ES2.Save(num2, "Gear");
		}
	}

	private void Update()
	{
	}

	public void ShareFB()
	{
		StartCoroutine(PostFBScreenshot());
	}

	private IEnumerator PostFBScreenshot()
	{
		yield return new WaitForEndOfFrame();
		int width = Screen.width;
		int height = Screen.height;
		Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, mipChain: false);
		tex.ReadPixels(new Rect(0f, 0f, width, height), 0, 0);
		tex.Apply();
		AndroidSocialGate.StartShareIntent("Share Text", "Galaxy war tower defense", tex, "Sample Text");
		UnityEngine.Object.Destroy(tex);
	}
}
