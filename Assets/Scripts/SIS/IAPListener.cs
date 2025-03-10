using UnityEngine;

namespace SIS
{
	public class IAPListener : MonoBehaviour
	{
		public void Init()
		{
			IAPManager.inventoryRequestFailedEvent += HandleFailedInventory;
			IAPManager.purchaseSucceededEvent += HandleSuccessfulPurchase;
			IAPManager.purchaseFailedEvent += HandleFailedPurchase;
			ShopManager.itemSelectedEvent += HandleSelectedItem;
			ShopManager.itemDeselectedEvent += HandleDeselectedItem;
		}

		public void HandleSuccessfulPurchase(string id)
		{
			if (IAPManager.isDebug)
			{
				UnityEngine.Debug.Log("HandleSuccessfulPurchase: " + id);
			}
			IAPItem iAPItem = null;
			if ((bool)ShopManager.GetInstance())
			{
				iAPItem = ShopManager.GetIAPItem(id);
			}
			if (iAPItem != null && (iAPItem.type == IAPType.nonConsumable || iAPItem.type == IAPType.nonConsumableVirtual || iAPItem.type == IAPType.subscription))
			{
				iAPItem.Purchased(state: true);
			}
			switch (id)
			{
			case "first_pack1":
				ES2.Save(param: true, "KeyFirstPurchase");
				if ((bool)firstPurchaseControl.Instance)
				{
					firstPurchaseControl.Instance.OffBtn();
				}
				DBManager.IncreaseFunds("coins", 12000);
				ShowMessage("12.000 Gear were added to your balance!");
				break;
			case "first_pack2":
				ES2.Save(param: true, "KeyFirstPurchase");
				if ((bool)firstPurchaseControl.Instance)
				{
					firstPurchaseControl.Instance.OffBtn();
				}
				DBManager.IncreaseFunds("coins", 70000);
				ShowMessage("70.000 Gear were added to your balance!");
				break;
			case "small_pack":
				DBManager.IncreaseFunds("coins", 2000);
				ShowMessage("2.000 coins were added to your balance!");
				break;
			case "medium_pack":
				DBManager.IncreaseFunds("coins", 5500);
				ShowMessage("5.500 coins were added to your balance!");
				break;
			case "big_pack":
				DBManager.IncreaseFunds("coins", 12000);
				ShowMessage("12.000 coins were added to your balance!");
				break;
			case "xl_pack":
				DBManager.IncreaseFunds("coins", 26000);
				ShowMessage("26.000 coins were added to your balance!");
				break;
			case "huge_pack":
				DBManager.IncreaseFunds("coins", 70000);
				ShowMessage("70.000 coins were added to your balance!");
				break;
			case "ultra_pack":
				DBManager.IncreaseFunds("coins", 150000);
				ShowMessage("150.000 coins were added to your balance!");
				break;
			case "remove_ads":
				ES2.Save(1, "Ads");
				ShowMessage("Ads disabled!");
				break;
			case "restore":
				ShowMessage("Restored transactions!");
				break;
			case "bullets":
				ShowMessage("Bullets were added to your inventory!");
				break;
			case "GunJetRange1":
				ES2.Save(1, "GunJetRange");
				break;
			case "GunJetRange2":
				ES2.Save(2, "GunJetRange");
				break;
			case "GunJetRange3":
				ES2.Save(3, "GunJetRange");
				break;
			case "GunJetRange4":
				ES2.Save(4, "GunJetRange");
				break;
			case "GunJetRange5":
				ES2.Save(5, "GunJetRange");
				break;
			case "GunJetDamage1":
				ES2.Save(1, "GunJetDamage");
				break;
			case "GunJetDamage2":
				ES2.Save(2, "GunJetDamage");
				break;
			case "GunJetDamage3":
				ES2.Save(3, "GunJetDamage");
				break;
			case "GunJetDamage4":
				ES2.Save(4, "GunJetDamage");
				break;
			case "GunJetDamage5":
				ES2.Save(5, "GunJetDamage");
				break;
			case "GunJetCooldown1":
				ES2.Save(1, "GunJetCooldown");
				break;
			case "GunJetCooldown2":
				ES2.Save(2, "GunJetCooldown");
				break;
			case "GunJetCooldown3":
				ES2.Save(3, "GunJetCooldown");
				break;
			case "GunJetCooldown4":
				ES2.Save(4, "GunJetCooldown");
				break;
			case "GunJetCooldown5":
				ES2.Save(5, "GunJetCooldown");
				break;
			case "GunJetCrift1":
				ES2.Save(1, "GunJetCrift");
				break;
			case "GunJetCrift2":
				ES2.Save(2, "GunJetCrift");
				break;
			case "GunJetCrift3":
				ES2.Save(3, "GunJetCrift");
				break;
			case "GunJetCrift4":
				ES2.Save(4, "GunJetCrift");
				break;
			case "GunJetCrift5":
				ES2.Save(5, "GunJetCrift");
				break;
			case "GunJetStun1":
				ES2.Save(1, "GunJetStun");
				break;
			case "GunJetStun2":
				ES2.Save(2, "GunJetStun");
				break;
			case "GunJetStun3":
				ES2.Save(3, "GunJetStun");
				break;
			case "GunJetStun4":
				ES2.Save(4, "GunJetStun");
				break;
			case "GunJetStun5":
				ES2.Save(5, "GunJetStun");
				break;
			case "SlowRange1":
				ES2.Save(1, "SlowRange");
				break;
			case "SlowRange2":
				ES2.Save(2, "SlowRange");
				break;
			case "SlowRange3":
				ES2.Save(3, "SlowRange");
				break;
			case "SlowRange4":
				ES2.Save(4, "SlowRange");
				break;
			case "SlowRange5":
				ES2.Save(5, "SlowRange");
				break;
			case "SlowCooldown1":
				ES2.Save(1, "SlowCooldown");
				break;
			case "SlowCooldown2":
				ES2.Save(2, "SlowCooldown");
				break;
			case "SlowCooldown3":
				ES2.Save(3, "SlowCooldown");
				break;
			case "SlowCooldown4":
				ES2.Save(4, "SlowCooldown");
				break;
			case "SlowCooldown5":
				ES2.Save(5, "SlowCooldown");
				break;
			case "SlowDamage1":
				ES2.Save(1, "SlowDamage");
				break;
			case "SlowDamage2":
				ES2.Save(2, "SlowDamage");
				break;
			case "SlowDamage3":
				ES2.Save(3, "SlowDamage");
				break;
			case "SlowDamage4":
				ES2.Save(4, "SlowDamage");
				break;
			case "SlowDamage5":
				ES2.Save(5, "SlowDamage");
				break;
			case "SlowTime1":
				ES2.Save(1, "SlowTime");
				break;
			case "SlowTime2":
				ES2.Save(2, "SlowTime");
				break;
			case "SlowTime3":
				ES2.Save(3, "SlowTime");
				break;
			case "SlowTime4":
				ES2.Save(4, "SlowTime");
				break;
			case "SlowTime5":
				ES2.Save(5, "SlowTime");
				break;
			case "CannonRange1":
				ES2.Save(1, "CannonRange");
				break;
			case "CannonRange2":
				ES2.Save(2, "CannonRange");
				break;
			case "CannonRange3":
				ES2.Save(3, "CannonRange");
				break;
			case "CannonRange4":
				ES2.Save(4, "CannonRange");
				break;
			case "CannonRange5":
				ES2.Save(5, "CannonRange");
				break;
			case "CannonDamage1":
				ES2.Save(1, "CannonDamage");
				break;
			case "CannonDamage2":
				ES2.Save(2, "CannonDamage");
				break;
			case "CannonDamage3":
				ES2.Save(3, "CannonDamage");
				break;
			case "CannonDamage4":
				ES2.Save(4, "CannonDamage");
				break;
			case "CannonDamage5":
				ES2.Save(5, "CannonDamage");
				break;
			case "CannonCooldown1":
				ES2.Save(1, "CannonCooldown");
				break;
			case "CannonCooldown2":
				ES2.Save(2, "CannonCooldown");
				break;
			case "CannonCooldown3":
				ES2.Save(3, "CannonCooldown");
				break;
			case "CannonCooldown4":
				ES2.Save(4, "CannonCooldown");
				break;
			case "CannonCooldown5":
				ES2.Save(5, "CannonCooldown");
				break;
			case "CannonCrift1":
				ES2.Save(1, "CannonCrift");
				break;
			case "CannonCrift2":
				ES2.Save(2, "CannonCrift");
				break;
			case "CannonCrift3":
				ES2.Save(3, "CannonCrift");
				break;
			case "CannonCrift4":
				ES2.Save(4, "CannonCrift");
				break;
			case "CannonCrift5":
				ES2.Save(5, "CannonCrift");
				break;
			case "CannonStun1":
				ES2.Save(1, "CannonStun");
				break;
			case "CannonStun2":
				ES2.Save(2, "CannonStun");
				break;
			case "CannonStun3":
				ES2.Save(3, "CannonStun");
				break;
			case "CannonStun4":
				ES2.Save(4, "CannonStun");
				break;
			case "CannonStun5":
				ES2.Save(5, "CannonStun");
				break;
			case "LaserRange1":
				ES2.Save(1, "LaserRange");
				break;
			case "LaserRange2":
				ES2.Save(2, "LaserRange");
				break;
			case "LaserRange3":
				ES2.Save(3, "LaserRange");
				break;
			case "LaserRange4":
				ES2.Save(4, "LaserRange");
				break;
			case "LaserRange5":
				ES2.Save(5, "LaserRange");
				break;
			case "LaserDamage1":
				ES2.Save(1, "LaserDamage");
				break;
			case "LaserDamage2":
				ES2.Save(2, "LaserDamage");
				break;
			case "LaserDamage3":
				ES2.Save(3, "LaserDamage");
				break;
			case "LaserDamage4":
				ES2.Save(4, "LaserDamage");
				break;
			case "LaserDamage5":
				ES2.Save(5, "LaserDamage");
				break;
			case "LaserCooldown1":
				ES2.Save(1, "LaserCooldown");
				break;
			case "LaserCooldown2":
				ES2.Save(2, "LaserCooldown");
				break;
			case "LaserCooldown3":
				ES2.Save(3, "LaserCooldown");
				break;
			case "LaserCooldown4":
				ES2.Save(4, "LaserCooldown");
				break;
			case "LaserCooldown5":
				ES2.Save(5, "LaserCooldown");
				break;
			case "LaserCrift1":
				ES2.Save(1, "LaserCrift");
				break;
			case "LaserCrift2":
				ES2.Save(2, "LaserCrift");
				break;
			case "LaserCrift3":
				ES2.Save(3, "LaserCrift");
				break;
			case "LaserCrift4":
				ES2.Save(4, "LaserCrift");
				break;
			case "LaserCrift5":
				ES2.Save(5, "LaserCrift");
				break;
			case "LaserStun1":
				ES2.Save(1, "LaserStun");
				break;
			case "LaserStun2":
				ES2.Save(2, "LaserStun");
				break;
			case "LaserStun3":
				ES2.Save(3, "LaserStun");
				break;
			case "LaserStun4":
				ES2.Save(4, "LaserStun");
				break;
			case "LaserStun5":
				ES2.Save(5, "LaserStun");
				break;
			case "BombRange1":
				ES2.Save(1, "BombRange");
				break;
			case "BombRange2":
				ES2.Save(2, "BombRange");
				break;
			case "BombRange3":
				ES2.Save(3, "BombRange");
				break;
			case "BombRange4":
				ES2.Save(4, "BombRange");
				break;
			case "BombRange5":
				ES2.Save(5, "BombRange");
				break;
			case "BombDamage1":
				ES2.Save(1, "BombDamage");
				break;
			case "BombDamage2":
				ES2.Save(2, "BombDamage");
				break;
			case "BombDamage3":
				ES2.Save(3, "BombDamage");
				break;
			case "BombDamage4":
				ES2.Save(4, "BombDamage");
				break;
			case "BombDamage5":
				ES2.Save(5, "BombDamage");
				break;
			case "BombCooldown1":
				ES2.Save(1, "BombCooldown");
				break;
			case "BombCooldown2":
				ES2.Save(2, "BombCooldown");
				break;
			case "BombCooldown3":
				ES2.Save(3, "BombCooldown");
				break;
			case "BombCooldown4":
				ES2.Save(4, "BombCooldown");
				break;
			case "BombCooldown5":
				ES2.Save(5, "BombCooldown");
				break;
			case "BombCrift1":
				ES2.Save(1, "BombCrift");
				break;
			case "BombCrift2":
				ES2.Save(2, "BombCrift");
				break;
			case "BombCrift3":
				ES2.Save(3, "BombCrift");
				break;
			case "BombCrift4":
				ES2.Save(4, "BombCrift");
				break;
			case "BombCrift5":
				ES2.Save(5, "BombCrift");
				break;
			case "BombStun1":
				ES2.Save(1, "BombStun");
				break;
			case "BombStun2":
				ES2.Save(2, "BombStun");
				break;
			case "BombStun3":
				ES2.Save(3, "BombStun");
				break;
			case "BombStun4":
				ES2.Save(4, "BombStun");
				break;
			case "BombStun5":
				ES2.Save(5, "BombStun");
				break;
			case "SunrayRange1":
				ES2.Save(1, "SunrayRange");
				break;
			case "SunrayRange2":
				ES2.Save(2, "SunrayRange");
				break;
			case "SunrayRange3":
				ES2.Save(3, "SunrayRange");
				break;
			case "SunrayRange4":
				ES2.Save(4, "SunrayRange");
				break;
			case "SunrayRange5":
				ES2.Save(5, "SunrayRange");
				break;
			case "SunrayDamage1":
				ES2.Save(1, "SunrayDamage");
				break;
			case "SunrayDamage2":
				ES2.Save(2, "SunrayDamage");
				break;
			case "SunrayDamage3":
				ES2.Save(3, "SunrayDamage");
				break;
			case "SunrayDamage4":
				ES2.Save(4, "SunrayDamage");
				break;
			case "SunrayDamage5":
				ES2.Save(5, "SunrayDamage");
				break;
			case "SunrayCooldown1":
				ES2.Save(1, "SunrayCooldown");
				break;
			case "SunrayCooldown2":
				ES2.Save(2, "SunrayCooldown");
				break;
			case "SunrayCooldown3":
				ES2.Save(3, "SunrayCooldown");
				break;
			case "SunrayCooldown4":
				ES2.Save(4, "SunrayCooldown");
				break;
			case "SunrayCooldown5":
				ES2.Save(5, "SunrayCooldown");
				break;
			case "SunrayCrift1":
				ES2.Save(1, "SunrayCrift");
				break;
			case "SunrayCrift2":
				ES2.Save(2, "SunrayCrift");
				break;
			case "SunrayCrift3":
				ES2.Save(3, "SunrayCrift");
				break;
			case "SunrayCrift4":
				ES2.Save(4, "SunrayCrift");
				break;
			case "SunrayCrift5":
				ES2.Save(5, "SunrayCrift");
				break;
			case "SunrayStun1":
				ES2.Save(1, "SunrayStun");
				break;
			case "SunrayStun2":
				ES2.Save(2, "SunrayStun");
				break;
			case "SunrayStun3":
				ES2.Save(3, "SunrayStun");
				break;
			case "SunrayStun4":
				ES2.Save(4, "SunrayStun");
				break;
			case "SunrayStun5":
				ES2.Save(5, "SunrayStun");
				break;
			case "BeamRange1":
				ES2.Save(1, "BeamRange");
				break;
			case "BeamRange2":
				ES2.Save(2, "BeamRange");
				break;
			case "BeamRange3":
				ES2.Save(3, "BeamRange");
				break;
			case "BeamRange4":
				ES2.Save(4, "BeamRange");
				break;
			case "BeamRange5":
				ES2.Save(5, "BeamRange");
				break;
			case "BeamDamage1":
				ES2.Save(1, "BeamDamage");
				break;
			case "BeamDamage2":
				ES2.Save(2, "BeamDamage");
				break;
			case "BeamDamage3":
				ES2.Save(3, "BeamDamage");
				break;
			case "BeamDamage4":
				ES2.Save(4, "BeamDamage");
				break;
			case "BeamDamage5":
				ES2.Save(5, "BeamDamage");
				break;
			case "BeamCooldown1":
				ES2.Save(1, "BeamCooldown");
				break;
			case "BeamCooldown2":
				ES2.Save(2, "BeamCooldown");
				break;
			case "BeamCooldown3":
				ES2.Save(3, "BeamCooldown");
				break;
			case "BeamCooldown4":
				ES2.Save(4, "BeamCooldown");
				break;
			case "BeamCooldown5":
				ES2.Save(5, "BeamCooldown");
				break;
			case "BeamCrift1":
				ES2.Save(1, "BeamCrift");
				break;
			case "BeamCrift2":
				ES2.Save(2, "BeamCrift");
				break;
			case "BeamCrift3":
				ES2.Save(3, "BeamCrift");
				break;
			case "BeamCrift4":
				ES2.Save(4, "BeamCrift");
				break;
			case "BeamCrift5":
				ES2.Save(5, "BeamCrift");
				break;
			case "BeamStuns1":
				ES2.Save(1, "BeamStun");
				break;
			case "BeamStuns2":
				ES2.Save(2, "BeamStun");
				break;
			case "BeamStuns3":
				ES2.Save(3, "BeamStun");
				break;
			case "BeamStuns4":
				ES2.Save(4, "BeamStun");
				break;
			case "BeamStuns5":
				ES2.Save(5, "BeamStun");
				break;
			case "AtomRange1":
				ES2.Save(1, "AtomRange");
				break;
			case "AtomRange2":
				ES2.Save(2, "AtomRange");
				break;
			case "AtomRange3":
				ES2.Save(3, "AtomRange");
				break;
			case "AtomRange4":
				ES2.Save(4, "AtomRange");
				break;
			case "AtomRange5":
				ES2.Save(5, "AtomRange");
				break;
			case "AtomDamage1":
				ES2.Save(1, "AtomDamage");
				break;
			case "AtomDamage2":
				ES2.Save(2, "AtomDamage");
				break;
			case "AtomDamage3":
				ES2.Save(3, "AtomDamage");
				break;
			case "AtomDamage4":
				ES2.Save(4, "AtomDamage");
				break;
			case "AtomDamage5":
				ES2.Save(5, "AtomDamage");
				break;
			case "AtomDamage6":
				ES2.Save(6, "AtomDamage");
				break;
			case "AtomDamage7":
				ES2.Save(7, "AtomDamage");
				break;
			case "AtomDamage8":
				ES2.Save(8, "AtomDamage");
				break;
			case "AtomDamage9":
				ES2.Save(9, "AtomDamage");
				break;
			case "AtomDamage10":
				ES2.Save(10, "AtomDamage");
				break;
			case "AtomCooldown1":
				ES2.Save(1, "AtomCooldown");
				break;
			case "AtomCooldown2":
				ES2.Save(2, "AtomCooldown");
				break;
			case "AtomCooldown3":
				ES2.Save(3, "AtomCooldown");
				break;
			case "AtomCooldown4":
				ES2.Save(4, "AtomCooldown");
				break;
			case "AtomCooldown5":
				ES2.Save(5, "AtomCooldown");
				break;
			case "AtomStun1":
				ES2.Save(1, "AtomStun");
				break;
			case "AtomStun2":
				ES2.Save(2, "AtomStun");
				break;
			case "AtomStun3":
				ES2.Save(3, "AtomStun");
				break;
			case "AtomStun4":
				ES2.Save(4, "AtomStun");
				break;
			case "AtomStun5":
				ES2.Save(5, "AtomStun");
				break;
			}
		}

		private void ShowMessage(string text)
		{
			if ((bool)ShopManager.GetInstance())
			{
				ShopManager.ShowMessage(text);
			}
		}

		private void HandleFailedInventory(string error)
		{
			if ((bool)ShopManager.GetInstance())
			{
				ShopManager.ShowMessage(error);
			}
		}

		private void HandleFailedPurchase(string error)
		{
			if ((bool)ShopManager.GetInstance())
			{
				ShopManager.ShowMessage(error);
			}
		}

		private void HandleSelectedItem(string id)
		{
			if (IAPManager.isDebug)
			{
				UnityEngine.Debug.Log("Selected: " + id);
			}
		}

		private void HandleDeselectedItem(string id)
		{
			if (IAPManager.isDebug)
			{
				UnityEngine.Debug.Log("Deselected: " + id);
			}
		}
	}
}
