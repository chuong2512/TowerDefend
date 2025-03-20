using SIS;
using UnityEngine;

public class firstPurchaseControl : MonoBehaviour
{
	public GameObject ButtonFirst;

	public GameObject CanvasFirst;

	public static firstPurchaseControl Instance;

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		if (ES2.Exists("KeyFirstPurchase"))
		{
			if (ES2.Load<bool>("KeyFirstPurchase"))
			{
				ButtonFirst.SetActive(value: false);
			}
			else
			{
				ButtonFirst.SetActive(value: true);
			}
		}
	}

	private void Update()
	{
	}

	public void OffBtn()
	{
		ButtonFirst.SetActive(value: false);
		CanvasFirst.SetActive(value: false);
	}

	public void First1()
	{
		//IAPManager.PurchaseProduct("first_pack1");
	}

	public void First2()
	{
		//IAPManager.PurchaseProduct("first_pack2");
	}
}
