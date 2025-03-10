using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TDTK
{
	public class FPSWeapon : MonoBehaviour
	{
		[Header("Base Info")]
		public int prefabID;

		public string weaponName = "Weapon";

		public Sprite icon;

		public bool disableInFPSControl;

		[Header("Attack Setting")]
		public int damageType;

		public float recoil = 1f;

		[Space(5f)]
		public List<Transform> shootPoints = new List<Transform>();

		[Space(5f)]
		public List<UnitStat> stats = new List<UnitStat>
		{
			new UnitStat()
		};

		private int currentActiveStat;

		private float currentCD;

		private int currentAmmo = 10;

		public AudioClip reloadSound;

		[Space(10f)]
		[Multiline]
		public string desp = string.Empty;

		private float reloadDuration;

		public int GetShootPointCount()
		{
			return shootPoints.Count;
		}

		public int GetCurrentAmmo()
		{
			return currentAmmo;
		}

		private void Awake()
		{
			currentAmmo = GetClipSize();
		}

		private void OnEnable()
		{
			reloadDuration = 0f;
		}

		public bool ReadyToFire()
		{
			if (IsOnCooldown())
			{
				return false;
			}
			if (OutOfAmmo())
			{
				return false;
			}
			return true;
		}

		public bool Shoot()
		{
			if (IsReloading())
			{
				return false;
			}
			if (IsOnCooldown())
			{
				return false;
			}
			if (OutOfAmmo())
			{
				StartCoroutine(ReloadRoutine());
				return false;
			}
			StartCoroutine(CooldownRoutine());
			currentAmmo--;
			if (OutOfAmmo())
			{
				StartCoroutine(ReloadRoutine());
			}
			return true;
		}

		public void Reload()
		{
			if (currentAmmo != GetClipSize())
			{
				StartCoroutine(ReloadRoutine());
			}
		}

		public IEnumerator CooldownRoutine()
		{
			currentCD = GetCooldown();
			while (currentCD > 0f)
			{
				currentCD -= Time.fixedDeltaTime;
				yield return new WaitForSeconds(Time.fixedDeltaTime);
			}
		}

		public IEnumerator ReloadRoutine()
		{
			FPSControl.StartReload(this);
			if (reloadSound != null)
			{
				AudioManager.PlaySound(reloadSound);
			}
			reloadDuration = GetReloadDuration();
			while (reloadDuration > 0f)
			{
				reloadDuration -= Time.deltaTime;
				yield return null;
			}
			currentAmmo = GetClipSize();
			FPSControl.ReloadComplete(this);
		}

		public float GetReloadProgress()
		{
			return 1f - reloadDuration / GetReloadDuration();
		}

		public bool IsOnCooldown()
		{
			return (currentCD > 0f) ? true : false;
		}

		public bool OutOfAmmo()
		{
			return (currentAmmo <= 0) ? true : false;
		}

		public bool IsReloading()
		{
			return (reloadDuration > 0f) ? true : false;
		}

		public float GetDamageMin()
		{
			return Mathf.Max(0f, stats[currentActiveStat].damageMin * (1f + PerkManager.GetFPSWeaponDamage(prefabID)));
		}

		public float GetDamageMax()
		{
			return Mathf.Max(0f, stats[currentActiveStat].damageMax * (1f + PerkManager.GetFPSWeaponDamage(prefabID)));
		}

		public float GetCooldown()
		{
			return Mathf.Max(0.05f, stats[currentActiveStat].cooldown * (1f + PerkManager.GetFPSWeaponCD(prefabID)));
		}

		public int GetClipSize()
		{
			return (int)(stats[currentActiveStat].clipSize * (1f + PerkManager.GetFPSWeaponClipSize(prefabID)));
		}

		public float GetReloadDuration()
		{
			return Mathf.Max(0.05f, stats[currentActiveStat].reloadDuration * (1f + PerkManager.GetFPSWeaponReloadDuration(prefabID)));
		}

		public float GetAOERange()
		{
			return stats[currentActiveStat].aoeRadius * (1f + PerkManager.GetFPSWeaponAOERadius(prefabID));
		}

		public float GetCritChance()
		{
			return stats[currentActiveStat].crit.chance + PerkManager.GetFPSWeaponDamage(prefabID);
		}

		public float GetCritMultiplier()
		{
			return stats[currentActiveStat].crit.dmgMultiplier + PerkManager.GetFPSWeaponDamage(prefabID);
		}

		public float GetShieldBreak()
		{
			return stats[currentActiveStat].shieldBreak + PerkManager.GetFPSWeaponShieldBreak(prefabID);
		}

		public float GetShieldPierce()
		{
			return stats[currentActiveStat].shieldPierce + PerkManager.GetFPSWeaponShieldPierce(prefabID);
		}

		public bool DamageShieldOnly()
		{
			return stats[currentActiveStat].damageShieldOnly;
		}

		public Stun GetStun()
		{
			return PerkManager.ModifyStunWithPerkBonus(stats[currentActiveStat].stun.Clone(), prefabID, 1);
		}

		public Slow GetSlow()
		{
			return PerkManager.ModifySlowWithPerkBonus(stats[currentActiveStat].slow.Clone(), prefabID, 1);
		}

		public Dot GetDot()
		{
			return PerkManager.ModifyDotWithPerkBonus(stats[currentActiveStat].dot.Clone(), prefabID, 1);
		}

		public InstantKill GetInstantKill()
		{
			return PerkManager.ModifyInstantKillWithPerkBonus(stats[currentActiveStat].instantKill.Clone(), prefabID, 1);
		}

		public Transform GetShootObject()
		{
			return stats[currentActiveStat].shootObject.transform;
		}
	}
}
