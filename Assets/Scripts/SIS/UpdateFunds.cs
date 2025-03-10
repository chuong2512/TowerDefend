using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace SIS
{
	public class UpdateFunds : MonoBehaviour
	{
		public Text label;

		public string currency;

		public float duration = 2f;

		private int curValue;

		private void OnEnable()
		{
			IAPManager.purchaseSucceededEvent += UpdateValue;
			DBManager.updatedDataEvent += UpdateValue;
			if ((bool)DBManager.GetInstance())
			{
				int funds = DBManager.GetFunds(currency);
				label.text = funds.ToString();
				curValue = funds;
			}
		}

		private void OnDisable()
		{
			IAPManager.purchaseSucceededEvent -= UpdateValue;
			DBManager.updatedDataEvent -= UpdateValue;
		}

		private void UpdateValue()
		{
			UpdateValue(null);
		}

		private void UpdateValue(string s)
		{
			StopCoroutine("CountTo");
			if (base.gameObject.activeInHierarchy)
			{
				StartCoroutine("CountTo", DBManager.GetFunds(currency));
			}
		}

		private IEnumerator CountTo(int target)
		{
			int start = curValue;
			for (float timer = 0f; timer < duration; timer += Time.deltaTime)
			{
				float progress = timer / duration;
				curValue = (int)Mathf.Lerp(start, target, progress);
				label.text = curValue + string.Empty;
				yield return null;
			}
			curValue = target;
			label.text = curValue + string.Empty;
		}
	}
}
