using UnityEngine;

namespace SIS
{
	public class DebugCalls : MonoBehaviour
	{
		public void Reset()
		{
			if ((bool)DBManager.GetInstance())
			{
				DBManager.ClearAll();
				DBManager.GetInstance().Init();
			}
		}

		public void LevelUp()
		{
			if ((bool)DBManager.GetInstance())
			{
				int num = DBManager.IncrementPlayerData("level", 1);
				if ((bool)ShopManager.GetInstance())
				{
					ShopManager.UnlockItems();
				}
				UnityEngine.Debug.Log("Leveled up to level: " + num + "! Shop Manager tried to unlock new items.");
			}
		}
	}
}
