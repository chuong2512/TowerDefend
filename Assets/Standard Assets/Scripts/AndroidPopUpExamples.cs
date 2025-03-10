using UnityEngine;

public class AndroidPopUpExamples : MonoBehaviour
{
	private string rateText = "If you enjoy using Google Earth, please take a moment to rate it. Thanks for your support!";

	private string rateUrl = "market://details?id=com.unionassets.android.plugin.preview";

	private void RateDialogPopUp()
	{
		AndroidRateUsPopUp androidRateUsPopUp = AndroidRateUsPopUp.Create("Rate Us", rateText, rateUrl);
		androidRateUsPopUp.ActionComplete += OnRatePopUpClose;
	}

	private void DialogPopUp()
	{
		AndroidDialog androidDialog = AndroidDialog.Create("Dialog Titile", "Dialog message");
		androidDialog.ActionComplete += OnDialogClose;
	}

	private void MessagePopUp()
	{
		AndroidMessage androidMessage = AndroidMessage.Create("Message Titile", "Message message");
		androidMessage.ActionComplete += OnMessageClose;
	}

	private void ShowPreloader()
	{
		Invoke("HidePreloader", 2f);
		AndroidNativeUtility.ShowPreloader("Loading", "Wait 2 seconds please");
	}

	private void HidePreloader()
	{
		AndroidNativeUtility.HidePreloader();
	}

	private void OpenRatingPage()
	{
		AndroidNativeUtility.OpenAppRatingPage(rateUrl);
	}

	private void OnRatePopUpClose(AndroidDialogResult result)
	{
		switch (result)
		{
		case AndroidDialogResult.RATED:
			UnityEngine.Debug.Log("RATED button pressed");
			break;
		case AndroidDialogResult.REMIND:
			UnityEngine.Debug.Log("REMIND button pressed");
			break;
		case AndroidDialogResult.DECLINED:
			UnityEngine.Debug.Log("DECLINED button pressed");
			break;
		}
		AN_PoupsProxy.showMessage("Result", result.ToString() + " button pressed");
	}

	private void OnDialogClose(AndroidDialogResult result)
	{
		switch (result)
		{
		case AndroidDialogResult.YES:
			UnityEngine.Debug.Log("Yes button pressed");
			break;
		case AndroidDialogResult.NO:
			UnityEngine.Debug.Log("No button pressed");
			break;
		}
		AN_PoupsProxy.showMessage("Result", result.ToString() + " button pressed");
	}

	private void OnMessageClose(AndroidDialogResult result)
	{
		AN_PoupsProxy.showMessage("Result", "Message Closed");
	}
}
