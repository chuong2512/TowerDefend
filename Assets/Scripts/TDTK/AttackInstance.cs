using UnityEngine;

namespace TDTK
{
	public class AttackInstance
	{
		public bool processed;

		public FPSWeapon srcWeapon;

		public Unit srcUnit;

		public Unit tgtUnit;

		public Vector3 impactPoint;

		public bool missed;

		public bool critical;

		public bool destroy;

		public bool stunned;

		public bool slowed;

		public bool dotted;

		public bool instantKill;

		public bool breakShield;

		public bool pierceShield;

		public float damage;

		public float damageHP;

		public float damageShield;

		public Stun stun;

		public Slow slow;

		public Dot dot;

		public AttackInstance()
		{
		}

		public AttackInstance(Unit sUnit, Unit tUnit)
		{
			srcUnit = sUnit;
			tgtUnit = tUnit;
		}

		public void Process()
		{
			if (!processed)
			{
				processed = true;
				if (srcUnit != null)
				{
					Process_SrcUnit();
				}
				else if (srcWeapon != null)
				{
					Process_SrcWeapon();
				}
			}
		}

		public void Process_SrcWeapon()
		{
			if (srcWeapon.GetInstantKill().IsApplicable(tgtUnit.HP, tgtUnit.GetFullHP()))
			{
				damage = tgtUnit.HP;
				damageHP = tgtUnit.HP;
				instantKill = true;
				destroy = true;
				return;
			}
			damage = Random.Range(srcWeapon.GetDamageMin(), srcWeapon.GetDamageMax());
			damage /= srcWeapon.GetShootPointCount();
			float num = srcWeapon.GetCritChance();
			if (tgtUnit.immuneToCrit)
			{
				num = -1f;
			}
			if (Random.Range(0f, 1f) < num)
			{
				critical = true;
				damage *= srcWeapon.GetCritMultiplier();
			}
			float modifier = DamageTable.GetModifier(tgtUnit.armorType, srcWeapon.damageType);
			damage *= modifier;
			if (damage >= tgtUnit.shield)
			{
				damageShield = tgtUnit.shield;
				damageHP = damage - tgtUnit.shield;
			}
			else
			{
				damageShield = damage;
				damageHP = 0f;
			}
			if (Random.Range(0f, 1f) < srcWeapon.GetShieldPierce() && damageShield > 0f)
			{
				damageHP += damageShield;
				damageShield = 0f;
				pierceShield = true;
			}
			if (srcWeapon.DamageShieldOnly())
			{
				damageHP = 0f;
			}
			if (damageHP >= tgtUnit.HP)
			{
				destroy = true;
				return;
			}
			if (Random.Range(0f, 1f) < srcWeapon.GetShieldBreak() && tgtUnit.defaultShield > 0f)
			{
				breakShield = true;
			}
			stunned = srcWeapon.GetStun().IsApplicable();
			if (tgtUnit.immuneToStun)
			{
				stunned = false;
			}
			slowed = srcWeapon.GetSlow().IsValid();
			if (tgtUnit.immuneToSlow)
			{
				slowed = false;
			}
			if (srcWeapon.GetDot().GetTotalDamage() > 0f)
			{
				dotted = true;
			}
			if (stunned)
			{
				stun = srcWeapon.GetStun().Clone();
			}
			if (slowed)
			{
				slow = srcWeapon.GetSlow().Clone();
			}
			if (dotted)
			{
				dot = srcWeapon.GetDot().Clone();
			}
		}

		public void Process_SrcUnit()
		{
			if (srcUnit.GetHit() <= 0f)
			{
				UnityEngine.Debug.LogWarning("Attacking unit (" + srcUnit.unitName + ") has default hitChance of 0%, is this intended?", srcUnit);
			}
			float num = Mathf.Clamp(srcUnit.GetHit() - tgtUnit.GetDodge(), 0f, 1f);
			if (Random.Range(0f, 1f) > num)
			{
				missed = true;
				return;
			}
			if (srcUnit.GetInstantKill().IsApplicable(tgtUnit.HP, tgtUnit.GetFullHP()))
			{
				damage = tgtUnit.HP;
				damageHP = tgtUnit.HP;
				instantKill = true;
				destroy = true;
				return;
			}
			damage = Random.Range(srcUnit.GetDamageMin(), srcUnit.GetDamageMax());
			damage /= srcUnit.GetShootPointCount();
			float num2 = srcUnit.GetCritChance();
			if (tgtUnit.immuneToCrit)
			{
				num2 = -1f;
			}
			if (Random.Range(0f, 1f) < num2)
			{
				critical = true;
				damage *= srcUnit.GetCritMultiplier();
			}
			float modifier = DamageTable.GetModifier(tgtUnit.armorType, srcUnit.damageType);
			damage *= modifier;
			if (damage >= tgtUnit.shield)
			{
				damageShield = tgtUnit.shield;
				damageHP = damage - tgtUnit.shield;
			}
			else
			{
				damageShield = damage;
				damageHP = 0f;
			}
			if (Random.Range(0f, 1f) < srcUnit.GetShieldPierce() && damageShield > 0f)
			{
				damageHP += damageShield;
				damageShield = 0f;
				pierceShield = true;
			}
			if (srcUnit.DamageShieldOnly())
			{
				damageHP = 0f;
			}
			if (damageHP >= tgtUnit.HP)
			{
				destroy = true;
				return;
			}
			if (Random.Range(0f, 1f) < srcUnit.GetShieldBreak() && tgtUnit.defaultShield > 0f)
			{
				breakShield = true;
			}
			stunned = srcUnit.GetStun().IsApplicable();
			if (tgtUnit.immuneToStun)
			{
				stunned = false;
			}
			slowed = srcUnit.GetSlow().IsValid();
			if (tgtUnit.immuneToSlow)
			{
				slowed = false;
			}
			if (srcUnit.GetDot().GetTotalDamage() > 0f)
			{
				dotted = true;
			}
			if (stunned)
			{
				stun = srcUnit.GetStun().Clone();
			}
			if (slowed)
			{
				slow = srcUnit.GetSlow().Clone();
			}
			if (dotted)
			{
				dot = srcUnit.GetDot().Clone();
			}
		}

		public AttackInstance Clone()
		{
			AttackInstance attackInstance = new AttackInstance();
			attackInstance.processed = processed;
			attackInstance.srcWeapon = srcWeapon;
			attackInstance.srcUnit = srcUnit;
			attackInstance.tgtUnit = tgtUnit;
			attackInstance.missed = missed;
			attackInstance.critical = critical;
			attackInstance.destroy = destroy;
			attackInstance.stunned = stunned;
			attackInstance.slowed = slowed;
			attackInstance.dotted = dotted;
			attackInstance.instantKill = instantKill;
			attackInstance.breakShield = breakShield;
			attackInstance.pierceShield = pierceShield;
			attackInstance.damage = damage;
			attackInstance.damageHP = damageHP;
			attackInstance.damageShield = damageShield;
			attackInstance.stun = stun;
			attackInstance.slow = slow;
			attackInstance.dot = dot;
			return attackInstance;
		}
	}
}
