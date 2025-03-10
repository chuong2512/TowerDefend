using UnityEngine;

public class UnlockShopBtn : MonoBehaviour
{
	public int NumUnlock;

	private int CurrentNumUnlock;

	private void Start()
	{
		CurrentNumUnlock = ES2.Load<int>("Tutorial");
		if (CurrentNumUnlock <= NumUnlock)
		{
			ES2.Save(NumUnlock, "Tutorial");
		}
	}
}
