using System;
using System.Collections;
using UnityEngine;

namespace TDTK
{
	[Serializable]
	public class Ability : TDTKItem
	{
		public enum _TargetType
		{
			Hostile,
			Friendly,
			Hybrid
		}

		public bool disableInAbilityManager;

		public int cost = 10;

		public float cooldown = 10f;

		[HideInInspector]
		public float currentCD;

		public bool requireTargetSelection = true;

		public bool singleUnitTargeting;

		public _TargetType targetType;

		public int maxUseCount = -1;

		[HideInInspector]
		public int usedCount;

		[HideInInspector]
		public int usedRemained;

		public bool useDefaultEffect = true;

		public AbilityEffect effect = new AbilityEffect();

		public float aoeRadius = 2f;

		public float effectDelay = 0.25f;

		public Transform indicator;

		public GameObject effectObj;

		public bool destroyEffectObj = true;

		public float destroyEffectDuration = 1.5f;

		public bool useCustomDesp;

		public string desp = string.Empty;

		public void Init()
		{
			if (maxUseCount > 0)
			{
				usedRemained = maxUseCount;
			}
			else
			{
				usedRemained = -1;
			}
			if (indicator != null)
			{
				indicator = UnityEngine.Object.Instantiate(indicator);
				indicator.parent = AbilityManager.GetInstance().transform;
			}
		}

		public void Activate(Vector3 pos)
		{
			usedCount++;
			usedRemained--;
			AbilityManager.GetInstance().StartCoroutine(CooldownRoutine());
			if (effectObj != null)
			{
				if (!destroyEffectObj)
				{
					ObjectPoolManager.Spawn(effectObj, pos, Quaternion.identity);
				}
				else
				{
					ObjectPoolManager.Spawn(effectObj, pos, Quaternion.identity, destroyEffectDuration);
				}
			}
		}

		public IEnumerator CooldownRoutine()
		{
			currentCD = GetCooldown();
			while (currentCD > 0f)
			{
				currentCD -= Time.deltaTime;
				yield return null;
			}
			TDTK.OnAbilityReady(this);
		}

		public string IsAvailable()
		{
			if (GetCost() > AbilityManager.GetEnergy())
			{
				return "Insufficient Energy";
			}
			if (currentCD > 0f)
			{
				return "Ability is on cooldown";
			}
			if (maxUseCount > 0 && usedCount >= maxUseCount)
			{
				return "Usage limit exceed";
			}
			return string.Empty;
		}

		public Ability Clone()
		{
			Ability ability = new Ability();
			ability.ID = ID;
			ability.name = name;
			ability.icon = icon;
			ability.disableInAbilityManager = disableInAbilityManager;
			ability.cost = cost;
			ability.cooldown = cooldown;
			ability.currentCD = currentCD;
			ability.requireTargetSelection = requireTargetSelection;
			ability.singleUnitTargeting = singleUnitTargeting;
			ability.targetType = targetType;
			ability.maxUseCount = maxUseCount;
			ability.usedCount = usedCount;
			ability.usedRemained = usedRemained;
			ability.useDefaultEffect = useDefaultEffect;
			ability.effect = effect.Clone();
			ability.aoeRadius = aoeRadius;
			ability.effectDelay = effectDelay;
			ability.indicator = indicator;
			ability.effectObj = effectObj;
			ability.destroyEffectObj = destroyEffectObj;
			ability.destroyEffectDuration = destroyEffectDuration;
			ability.useCustomDesp = useCustomDesp;
			ability.desp = desp;
			return ability;
		}

		public float GetCost()
		{
			return (float)cost * (1f - PerkManager.GetAbilityCost(ID));
		}

		public float GetCooldown()
		{
			return cooldown * (1f - PerkManager.GetAbilityCooldown(ID));
		}

		public float GetAOERadius()
		{
			return aoeRadius * (1f + PerkManager.GetAbilityAOERadius(ID));
		}

		public AbilityEffect GetActiveEffect()
		{
			AbilityEffect abilityEffect = new AbilityEffect();
			abilityEffect.duration = effect.duration * (1f + PerkManager.GetAbilityDuration(ID));
			abilityEffect.damageMin = effect.damageMin * (1f + PerkManager.GetAbilityDamage(ID));
			abilityEffect.damageMax = effect.damageMax * (1f + PerkManager.GetAbilityDamage(ID));
			abilityEffect.stunChance = effect.stunChance * (1f + PerkManager.GetAbilityStunChance(ID));
			abilityEffect.slow = PerkManager.ModifySlowWithPerkBonus(effect.slow.Clone(), ID, 2);
			abilityEffect.dot = PerkManager.ModifyDotWithPerkBonus(effect.dot.Clone(), ID, 2);
			abilityEffect.damageBuff = effect.damageBuff * (1f + PerkManager.GetAbilityDamageBuff(ID));
			abilityEffect.rangeBuff = effect.rangeBuff * (1f + PerkManager.GetAbilityRangeBuff(ID));
			abilityEffect.cooldownBuff = effect.cooldownBuff * (1f + PerkManager.GetAbilityCooldownBuff(ID));
			abilityEffect.HPGainMin = effect.HPGainMin * (1f + PerkManager.GetAbilityHPGain(ID));
			abilityEffect.HPGainMax = effect.HPGainMax * (1f + PerkManager.GetAbilityHPGain(ID));
			return abilityEffect;
		}

		public string GetDesp()
		{
			if (useCustomDesp)
			{
				return desp;
			}
			string text = string.Empty;
			AbilityEffect activeEffect = GetActiveEffect();
			if (activeEffect.damageMax > 0f)
			{
				if (requireTargetSelection)
				{
					string text2 = text;
					text = text2 + "Deals " + activeEffect.damageMin + "-" + activeEffect.damageMax + " to hostile target in range\n";
				}
				else
				{
					string text2 = text;
					text = text2 + "Deals " + activeEffect.damageMin + "-" + activeEffect.damageMax + " to all hostile on the map\n";
				}
			}
			if (activeEffect.stunChance > 0f && activeEffect.duration > 0f)
			{
				if (requireTargetSelection)
				{
					string text2 = text;
					text = text2 + (activeEffect.stunChance * 100f).ToString("f0") + "% chance to stun hostile target for " + activeEffect.duration + "s\n";
				}
				else
				{
					string text2 = text;
					text = text2 + (activeEffect.stunChance * 100f).ToString("f0") + "% chance to stun all hostile on the map for " + activeEffect.duration + "s\n";
				}
			}
			if (activeEffect.slow.IsValid())
			{
				if (requireTargetSelection)
				{
					string text2 = text;
					text = text2 + "Slows hostile target down for " + activeEffect.duration + "s\n";
				}
				else
				{
					string text2 = text;
					text = text2 + "Slows all hostile on the map down for " + activeEffect.duration + "s\n";
				}
			}
			if (activeEffect.dot.GetTotalDamage() > 0f)
			{
				if (requireTargetSelection)
				{
					string text2 = text;
					text = text2 + "Deals " + activeEffect.dot.GetTotalDamage().ToString("f0") + " to hostile target over " + activeEffect.duration + "s\n";
				}
				else
				{
					string text2 = text;
					text = text2 + "Deals " + activeEffect.dot.GetTotalDamage().ToString("f0") + " to all hostile on the map over " + activeEffect.duration + "s\n";
				}
			}
			if (activeEffect.HPGainMax > 0f)
			{
				if (requireTargetSelection)
				{
					string text2 = text;
					text = text2 + "Restore " + activeEffect.HPGainMin + "-" + activeEffect.HPGainMax + " of friendly target HP\n";
				}
				else
				{
					string text2 = text;
					text = text2 + "Restore " + activeEffect.HPGainMin + "-" + activeEffect.HPGainMax + " of all tower HP\n";
				}
			}
			if (activeEffect.duration > 0f)
			{
				if (activeEffect.damageBuff > 0f)
				{
					text = ((!requireTargetSelection) ? (text + "Increase all towers damage by " + (activeEffect.damageBuff * 100f).ToString("f0") + "%\n") : (text + "Increase friendly target damage by " + (activeEffect.damageBuff * 100f).ToString("f0") + "%\n"));
				}
				if (activeEffect.rangeBuff > 0f)
				{
					text = ((!requireTargetSelection) ? (text + "Increase all towers range by " + (activeEffect.rangeBuff * 100f).ToString("f0") + "%\n") : (text + "Increase friendly target range by " + (activeEffect.rangeBuff * 100f).ToString("f0") + "%\n"));
				}
				if (activeEffect.cooldownBuff > 0f)
				{
					text = ((!requireTargetSelection) ? (text + "Decrease all towers cooldown by " + (activeEffect.cooldownBuff * 100f).ToString("f0") + "%\n") : (text + "Decrease friendly target cooldown by " + (activeEffect.cooldownBuff * 100f).ToString("f0") + "%\n"));
				}
			}
			return text;
		}
	}
}
