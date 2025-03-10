using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TDTK
{
	public class Unit : MonoBehaviour
	{
		public enum _TargetPriority
		{
			Nearest,
			Weakest,
			Toughest,
			First,
			Random
		}

		[Header("Base Info")]
		public int prefabID = -1;

		public int instanceID = -1;

		public string unitName = "unit";

		public Sprite iconSprite;

		public bool useCustomDesp;

		[Multiline]
		public string desp = string.Empty;

		protected bool isCreep;

		protected bool isTower;

		[Header("Basic Stats")]
		public float defaultHP = 10f;

		public float HP = 10f;

		public float HPRegenRate;

		public float HPStaggerDuration = 10f;

		private float currentHPStagger;

		[Space(10f)]
		public float defaultShield;

		public float shield;

		public float shieldRegenRate = 1f;

		public float shieldStaggerDuration = 1f;

		private float currentShieldStagger;

		[Space(10f)]
		public int damageType;

		public int armorType;

		public float dodgeChance;

		[Space(10f)]
		public bool immuneToCrit;

		public bool immuneToSlow;

		public bool immuneToStun;

		[Space(10f)]
		public Transform targetPoint;

		public float hitThreshold = 0.25f;

		[Header("Attack And Aim Setting")]
		public List<UnitStat> stats = new List<UnitStat>();

		protected int currentActiveStat;

		[HideInInspector]
		public ShootObject localShootObject;

		public _TargetPriority targetPriority = _TargetPriority.Random;

		[Space(10f)]
		public Transform turretObject;

		public Transform barrelObject;

		public List<Transform> shootPoints = new List<Transform>();

		public bool rotateTurretAimInXAxis = true;

		public float delayBetweenShootPoint;

		private float turretRotateSpeed = 25f;

		private float aimTolerance = 5f;

		private bool targetInLOS;

		[Space(10f)]
		public bool directionalTargeting;

		public float dirScanAngle;

		public float dirScanFOV = 60f;

		protected Vector3 dirScanV;

		protected Quaternion dirScanRot;

		protected LayerMask maskTarget = 0;

		public Transform scanDirT;

		[Header("Unit Status")]
		protected Unit target;

		protected bool destroyed;

		protected bool stunned;

		private float stunDuration;

		protected float slowMultiplier = 1f;

		protected List<Slow> slowEffectList = new List<Slow>();

		protected List<Buff> buffEffect = new List<Buff>();

		[Header("Visual Effects")]
		public GameObject destroyedEffObj;

		public bool autoDestroyEff = true;

		public float destroyEffDuration = 1f;

		[HideInInspector]
		public GameObject thisObj;

		[HideInInspector]
		public Transform thisT;

		private bool turretOnCooldown;

		private List<Unit> buffedUnit = new List<Unit>();

		private bool supportRoutineRunning;

		[Header("Buff multiplier")]
		[HideInInspector]
		public List<Buff> activeBuffList = new List<Buff>();

		[HideInInspector]
		public float damageBuffMul;

		[HideInInspector]
		public float cooldownBuffMul;

		[HideInInspector]
		public float rangeBuffMul;

		[HideInInspector]
		public float criticalBuffMod = 0.1f;

		[HideInInspector]
		public float hitBuffMod = 0.1f;

		[HideInInspector]
		public float dodgeBuffMod = 0.1f;

		[HideInInspector]
		public float regenHPBuff;

		private float dmgABMul;

		private float rangeABMul;

		private float cdABMul;

		private UnitAnimation uAnimation;

		public bool IsCreep()
		{
			return isCreep;
		}

		public bool IsTower()
		{
			return isTower;
		}

		public UnitStat GetBaseStats()
		{
			return stats[0];
		}

		public LayerMask GetTargetMask()
		{
			return maskTarget;
		}

		public bool IsDestroyed()
		{
			return destroyed;
		}

		public virtual void Awake()
		{
			thisObj = base.gameObject;
			thisT = base.transform;
			if (shootPoints.Count == 0)
			{
				shootPoints.Add(thisT);
			}
			ResetBuff();
			for (int i = 0; i < stats.Count; i++)
			{
				if (stats[i].shootObject != null && localShootObject == null)
				{
					localShootObject = stats[i].shootObject;
				}
			}
		}

		public void Init()
		{
			destroyed = false;
			stunned = false;
			HP = GetFullHP();
			shield = GetFullShield();
			currentHPStagger = 0f;
			currentShieldStagger = 0f;
			ResetBuff();
			ResetSlow();
		}

		public virtual void Start()
		{
		}

		public virtual void OnEnable()
		{
		}

		public virtual void OnDisable()
		{
		}

		public virtual void Update()
		{
		}

		public virtual void FixedUpdate()
		{
			if (regenHPBuff != 0f)
			{
				HP += regenHPBuff * Time.fixedDeltaTime;
				HP = Mathf.Clamp(HP, 0f, GetFullHP());
			}
			if (HPRegenRate > 0f && currentHPStagger <= 0f)
			{
				HP += GetHPRegenRate() * Time.fixedDeltaTime;
				HP = Mathf.Clamp(HP, 0f, GetFullHP());
			}
			if (defaultShield > 0f && shieldRegenRate > 0f && currentShieldStagger <= 0f)
			{
				shield += GetShieldRegenRate() * Time.fixedDeltaTime;
				shield = Mathf.Clamp(shield, 0f, GetFullShield());
			}
			currentHPStagger -= Time.fixedDeltaTime;
			currentShieldStagger -= Time.fixedDeltaTime;
			if (target != null && !IsInConstruction() && !stunned)
			{
				if (turretObject != null)
				{
					if (rotateTurretAimInXAxis && barrelObject != null)
					{
						Vector3 position = target.GetTargetT().position;
						Vector3 a = position;
						Vector3 position2 = turretObject.position;
						a.y = position2.y;
						Quaternion b = Quaternion.LookRotation(a - turretObject.position);
						turretObject.rotation = Quaternion.Slerp(turretObject.rotation, b, turretRotateSpeed * Time.deltaTime);
						Vector3 eulerAngles = Quaternion.LookRotation(position - barrelObject.position).eulerAngles;
						float x = eulerAngles.x;
						float num = Mathf.Min(1f, Vector3.Distance(turretObject.position, position) / GetSOMaxRange());
						float num2 = num * GetSOMaxAngle();
						b = turretObject.rotation * Quaternion.Euler(x - num2, 0f, 0f);
						barrelObject.rotation = Quaternion.Slerp(barrelObject.rotation, b, turretRotateSpeed * Time.deltaTime);
						if (Quaternion.Angle(barrelObject.rotation, b) < aimTolerance)
						{
							targetInLOS = true;
						}
						else
						{
							targetInLOS = false;
						}
					}
					else
					{
						Vector3 position3 = target.GetTargetT().position;
						if (!rotateTurretAimInXAxis)
						{
							Vector3 position4 = turretObject.position;
							position3.y = position4.y;
						}
						Quaternion quaternion = Quaternion.LookRotation(position3 - turretObject.position);
						if (rotateTurretAimInXAxis)
						{
							float num3 = Mathf.Min(1f, Vector3.Distance(turretObject.position, position3) / GetSOMaxRange());
							float num4 = num3 * GetSOMaxAngle();
							quaternion *= Quaternion.Euler(0f - num4, 0f, 0f);
						}
						turretObject.rotation = Quaternion.Slerp(turretObject.rotation, quaternion, turretRotateSpeed * Time.deltaTime);
						if (Quaternion.Angle(turretObject.rotation, quaternion) < aimTolerance)
						{
							targetInLOS = true;
						}
						else
						{
							targetInLOS = false;
						}
					}
				}
				else
				{
					targetInLOS = true;
				}
			}
			if (IsCreep() && target == null && turretObject != null && !stunned)
			{
				turretObject.localRotation = Quaternion.Slerp(turretObject.localRotation, Quaternion.identity, turretRotateSpeed * Time.deltaTime * 0.25f);
			}
		}

		private float GetSOMaxRange()
		{
			if (stats[currentActiveStat].shootObject == null)
			{
				return localShootObject.GetMaxShootRange();
			}
			return stats[currentActiveStat].shootObject.GetMaxShootRange();
		}

		private float GetSOMaxAngle()
		{
			if (stats[currentActiveStat].shootObject == null)
			{
				return localShootObject.GetMaxShootAngle();
			}
			return stats[currentActiveStat].shootObject.GetMaxShootAngle();
		}

		public virtual void IterateTargetPriority(int i = 1)
		{
			int num = (int)(targetPriority + i);
			if (num >= 5)
			{
				num = 0;
			}
			if (num < 0)
			{
				num = 4;
			}
			targetPriority = (_TargetPriority)num;
		}

		public virtual void ChangeScanAngle(int angle)
		{
			dirScanAngle = angle;
			dirScanRot = thisT.rotation * Quaternion.Euler(0f, dirScanAngle, 0f);
			if (turretObject != null && target == null)
			{
				turretObject.localRotation = Quaternion.identity * Quaternion.Euler(0f, dirScanAngle, 0f);
			}
		}

		public IEnumerator ScanForTargetRoutine()
		{
			while (true)
			{
				ScanForTarget();
				yield return new WaitForSeconds(0.1f);
				if (GameControl.ResetTargetAfterShoot())
				{
					while (turretOnCooldown)
					{
						yield return null;
					}
				}
			}
		}

		private void ScanForTarget()
		{
			if (destroyed || IsInConstruction() || stunned)
			{
				return;
			}
			if (directionalTargeting)
			{
				if (IsCreep())
				{
					dirScanRot = thisT.rotation;
				}
				else
				{
					dirScanRot = thisT.rotation * Quaternion.Euler(0f, dirScanAngle, 0f);
				}
			}
			if (directionalTargeting && scanDirT != null)
			{
				scanDirT.rotation = dirScanRot;
			}
			if (target == null)
			{
				List<Unit> list = TDTK.GetUnitInRange(thisT.position, GetRange(), GetRangeMin(), maskTarget);
				if (list.Count > 0 && directionalTargeting)
				{
					List<Unit> list2 = new List<Unit>();
					for (int i = 0; i < list.Count; i++)
					{
						Quaternion b = Quaternion.LookRotation(list[i].thisT.position - thisT.position);
						if (Quaternion.Angle(dirScanRot, b) <= dirScanFOV * 0.5f)
						{
							list2.Add(list[i]);
						}
					}
					list = list2;
				}
				if (list.Count > 0)
				{
					if (targetPriority == _TargetPriority.Random)
					{
						target = list[Random.Range(0, list.Count - 1)];
					}
					else if (targetPriority == _TargetPriority.Nearest)
					{
						float num = float.PositiveInfinity;
						for (int j = 0; j < list.Count; j++)
						{
							float num2 = Vector3.Distance(thisT.position, list[j].thisT.position);
							if (num2 < num)
							{
								num = num2;
								target = list[j];
							}
						}
					}
					else if (targetPriority == _TargetPriority.Weakest)
					{
						float num3 = float.PositiveInfinity;
						for (int k = 0; k < list.Count; k++)
						{
							if (list[k].HP < num3)
							{
								num3 = list[k].HP;
								target = list[k];
							}
						}
					}
					else if (targetPriority == _TargetPriority.Toughest)
					{
						float num4 = 0f;
						for (int l = 0; l < list.Count; l++)
						{
							if (list[l].HP > num4)
							{
								num4 = list[l].HP;
								target = list[l];
							}
						}
					}
					else if (targetPriority == _TargetPriority.First)
					{
						target = list[Random.Range(0, list.Count - 1)];
						float num5 = float.PositiveInfinity;
						for (int m = 0; m < list.Count; m++)
						{
							if (list[m].GetDistFromDestination() < num5)
							{
								num5 = list[m].GetDistFromDestination();
								target = list[m];
							}
						}
					}
				}
				targetInLOS = false;
				return;
			}
			float num6 = Vector3.Distance(thisT.position, target.thisT.position);
			if (target.IsDestroyed() || num6 > GetRange())
			{
				target = null;
			}
			if (target != null && directionalTargeting)
			{
				Quaternion b2 = Quaternion.LookRotation(target.thisT.position - thisT.position);
				if (Quaternion.Angle(dirScanRot, b2) >= dirScanFOV * 0.6f)
				{
					target = null;
				}
			}
		}

		public IEnumerator TurretRoutine()
		{
			turretOnCooldown = false;
			for (int j = 0; j < shootPoints.Count; j++)
			{
				if (shootPoints[j] == null)
				{
					shootPoints.RemoveAt(j);
					j--;
				}
			}
			if (shootPoints.Count == 0)
			{
				UnityEngine.Debug.LogWarning("ShootPoint not assigned for unit - " + unitName + ", auto assigned", this);
				shootPoints.Add(thisT);
			}
			for (int k = 0; k < stats.Count; k++)
			{
				if (stats[k].shootObject != null)
				{
					ObjectPoolManager.New(stats[k].shootObject.gameObject, 3);
				}
			}
			yield return null;
			while (true)
			{
				if (!(target == null) && !stunned && !IsInConstruction() && targetInLOS)
				{
					turretOnCooldown = true;
					Unit currentTarget = target;
					float animationDelay = PlayAnimAttack();
					if (animationDelay > 0f)
					{
						yield return new WaitForSeconds(animationDelay);
					}
					AttackInstance attInstance = new AttackInstance();
					attInstance.srcUnit = this;
					attInstance.tgtUnit = currentTarget;
					attInstance.Process();
					for (int i = 0; i < shootPoints.Count; i++)
					{
						Transform sp = shootPoints[i];
						GameObject sObj = ObjectPoolManager.Spawn(GetShootObject().gameObject, sp.position, sp.rotation);
						ShootObject shootObj = sObj.GetComponent<ShootObject>();
						shootObj.Shoot(attInstance, sp);
						if (delayBetweenShootPoint > 0f)
						{
							yield return new WaitForSeconds(delayBetweenShootPoint);
						}
					}
					yield return new WaitForSeconds(GetCooldown() - animationDelay - (float)shootPoints.Count * delayBetweenShootPoint);
					if (GameControl.ResetTargetAfterShoot())
					{
						target = null;
					}
					turretOnCooldown = false;
				}
				else
				{
					yield return null;
				}
			}
		}

		public void ApplyEffect(AttackInstance attInstance)
		{
			if (destroyed || attInstance.missed)
			{
				return;
			}
			shield -= attInstance.damageShield;
			HP -= attInstance.damageHP;
			new TextOverlay(GetTextOverlayPos(), attInstance.damage.ToString("f0"), new Color(1f, 1f, 1f, 1f));
			PlayAnimHit();
			TDTK.OnUnitDamaged(this);
			currentHPStagger = GetHPStaggerDuration();
			currentShieldStagger = GetShieldStaggerDuration();
			if (attInstance.destroy || HP <= 0f)
			{
				Destroyed();
				return;
			}
			if (attInstance.breakShield)
			{
				defaultShield = 0f;
				shield = 0f;
			}
			if (attInstance.stunned)
			{
				ApplyStun(attInstance.stun.duration);
			}
			if (attInstance.slowed)
			{
				ApplySlow(attInstance.slow);
			}
			if (attInstance.dotted)
			{
				ApplyDot(attInstance.dot);
			}
		}

		public void ApplyStun(float duration)
		{
			stunDuration = duration;
			if (!stunned)
			{
				StartCoroutine(StunRoutine());
			}
		}

		private IEnumerator StunRoutine()
		{
			stunned = true;
			while (stunDuration > 0f)
			{
				stunDuration -= Time.deltaTime;
				yield return null;
			}
			stunned = false;
		}

		public void ApplySlow(Slow slow)
		{
			StartCoroutine(SlowRoutine(slow));
		}

		private IEnumerator SlowRoutine(Slow slow)
		{
			slowEffectList.Add(slow);
			ResetSlowMultiplier();
			yield return new WaitForSeconds(slow.duration);
			slowEffectList.Remove(slow);
			ResetSlowMultiplier();
		}

		private void ResetSlowMultiplier()
		{
			if (slowEffectList.Count == 0)
			{
				slowMultiplier = 1f;
				return;
			}
			for (int i = 0; i < slowEffectList.Count; i++)
			{
				if (slowEffectList[i].slowMultiplier < slowMultiplier)
				{
					slowMultiplier = slowEffectList[i].slowMultiplier;
				}
			}
			slowMultiplier = Mathf.Max(0f, slowMultiplier);
		}

		private void ResetSlow()
		{
			slowEffectList = new List<Slow>();
			ResetSlowMultiplier();
		}

		public void ApplyDot(Dot dot)
		{
			StartCoroutine(DotRoutine(dot));
		}

		private IEnumerator DotRoutine(Dot dot)
		{
			int count = (int)Mathf.Floor(dot.duration / dot.interval);
			int i = 0;
			while (true)
			{
				if (i < count)
				{
					if (dot.interval == 0f)
					{
						yield return null;
					}
					else
					{
						yield return new WaitForSeconds(dot.interval);
					}
					if (destroyed)
					{
						yield break;
					}
					DamageHP(dot.value);
					if (HP <= 0f)
					{
						break;
					}
					i++;
					continue;
				}
				yield break;
			}
			Destroyed();
		}

		public void ApplyDamage(float dmg)
		{
			DamageHP(dmg);
			if (HP <= 0f)
			{
				Destroyed();
			}
		}

		public void RestoreHP(float value)
		{
			new TextOverlay(GetTextOverlayPos(), value.ToString("f0"), new Color(0f, 1f, 0.4f, 1f));
			HP = Mathf.Clamp(HP + value, 0f, GetFullHP());
		}

		private void DamageHP(float dmg)
		{
			HP -= dmg;
			new TextOverlay(GetTextOverlayPos(), dmg.ToString("f0"), new Color(1f, 1f, 1f, 1f));
			TDTK.OnUnitDamaged(this);
			PlayAnimHit();
			currentHPStagger = HPStaggerDuration;
			currentShieldStagger = shieldStaggerDuration;
		}

		private IEnumerator SupportEffectRoutine()
		{
			while (true)
			{
				yield return new WaitForSeconds(GetCooldown());
				while (stunned || IsInConstruction())
				{
					yield return null;
				}
				SpawnEffectObject();
			}
		}

		protected IEnumerator SupportRoutine()
		{
			supportRoutineRunning = true;
			StartCoroutine(SupportEffectRoutine());
			while (true)
			{
				yield return new WaitForSeconds(0.1f);
				if (destroyed)
				{
					continue;
				}
				List<Unit> list = new List<Unit>();
				Collider[] array = Physics.OverlapSphere(thisT.position, GetRange(), maskTarget);
				if (array.Length > 0)
				{
					for (int i = 0; i < array.Length; i++)
					{
						Unit component = array[i].gameObject.GetComponent<Unit>();
						if (!component.IsDestroyed())
						{
							list.Add(component);
						}
					}
				}
				for (int j = 0; j < buffedUnit.Count; j++)
				{
					Unit unit = buffedUnit[j];
					if (unit == null || unit.IsDestroyed())
					{
						buffedUnit.RemoveAt(j);
						j--;
					}
					else if (!list.Contains(unit))
					{
						unit.UnBuff(GetBuff());
						buffedUnit.RemoveAt(j);
						j--;
					}
				}
				for (int k = 0; k < list.Count; k++)
				{
					Unit unit2 = list[k];
					if (!buffedUnit.Contains(unit2))
					{
						unit2.Buff(GetBuff());
						buffedUnit.Add(unit2);
					}
				}
			}
		}

		public void UnbuffAll()
		{
			for (int i = 0; i < buffedUnit.Count; i++)
			{
				buffedUnit[i].UnBuff(GetBuff());
			}
		}

		public void Buff(Buff buff)
		{
			if (!activeBuffList.Contains(buff))
			{
				activeBuffList.Add(buff);
				UpdateBuffStat();
			}
		}

		public void UnBuff(Buff buff)
		{
			if (activeBuffList.Contains(buff))
			{
				activeBuffList.Remove(buff);
				UpdateBuffStat();
			}
		}

		private void UpdateBuffStat()
		{
			ClearBuffStats();
			for (int i = 0; i < activeBuffList.Count; i++)
			{
				Buff buff = activeBuffList[i];
				if (damageBuffMul < buff.damageBuff)
				{
					damageBuffMul = buff.damageBuff;
				}
				if (cooldownBuffMul < buff.cooldownBuff)
				{
					cooldownBuffMul = buff.cooldownBuff;
				}
				if (!supportRoutineRunning && rangeBuffMul < buff.rangeBuff)
				{
					rangeBuffMul = buff.rangeBuff;
				}
				if (criticalBuffMod < buff.criticalBuff)
				{
					criticalBuffMod = buff.criticalBuff;
				}
				if (hitBuffMod < buff.hitBuff)
				{
					hitBuffMod = buff.hitBuff;
				}
				if (dodgeBuffMod < buff.dodgeBuff)
				{
					dodgeBuffMod = buff.dodgeBuff;
				}
				if (regenHPBuff < buff.regenHP)
				{
					regenHPBuff = buff.regenHP;
				}
			}
		}

		private void ResetBuff()
		{
			activeBuffList = new List<Buff>();
			ClearBuffStats();
		}

		private void ClearBuffStats()
		{
			damageBuffMul = 0f;
			cooldownBuffMul = 0f;
			rangeBuffMul = 0f;
			criticalBuffMod = 0f;
			hitBuffMod = 0f;
			dodgeBuffMod = 0f;
			regenHPBuff = 0f;
		}

		public virtual void Destroyed(float delay = 0f)
		{
			destroyed = true;
			if (destroyedEffObj != null)
			{
				if (!autoDestroyEff)
				{
					ObjectPoolManager.Spawn(destroyedEffObj, targetPoint.position, thisT.rotation);
				}
				else
				{
					ObjectPoolManager.Spawn(destroyedEffObj, targetPoint.position, thisT.rotation, destroyEffDuration);
				}
			}
			if (supportRoutineRunning)
			{
				UnbuffAll();
			}
			StartCoroutine(_Destroyed(delay));
		}

		protected IEnumerator _Destroyed(float delay)
		{
			yield return new WaitForSeconds(delay);
			ObjectPoolManager.Unspawn(thisObj);
		}

		public Transform GetTargetT()
		{
			return (!(targetPoint != null)) ? thisT : targetPoint;
		}

		public Vector3 GetTextOverlayPos()
		{
			return GetTargetT().position + new Vector3(0f, 0.05f, 0f);
		}

		private float GetPerkMulHP()
		{
			return (!IsTower()) ? 0f : PerkManager.GetTowerHP(prefabID);
		}

		private float GetPerkMulHPRegen()
		{
			return (!IsTower()) ? 0f : PerkManager.GetTowerHPRegen(prefabID);
		}

		private float GetPerkMulHPStagger()
		{
			return (!IsTower()) ? 0f : PerkManager.GetTowerHPStagger(prefabID);
		}

		private float GetPerkMulShield()
		{
			return (!IsTower()) ? 0f : PerkManager.GetTowerShield(prefabID);
		}

		private float GetPerkMulShieldRegen()
		{
			return (!IsTower()) ? 0f : PerkManager.GetTowerShieldRegen(prefabID);
		}

		private float GetPerkMulShieldStagger()
		{
			return (!IsTower()) ? 0f : PerkManager.GetTowerShieldStagger(prefabID);
		}

		private float GetPerkMulDamage()
		{
			return (!IsTower()) ? 0f : PerkManager.GetTowerDamage(prefabID);
		}

		private float GetPerkMulCooldown()
		{
			return (!IsTower()) ? 0f : PerkManager.GetTowerCD(prefabID);
		}

		private float GetPerkMulClipSize()
		{
			return (!IsTower()) ? 0f : PerkManager.GetTowerClipSize(prefabID);
		}

		private float GetPerkMulReloadDuration()
		{
			return (!IsTower()) ? 0f : PerkManager.GetTowerReloadDuration(prefabID);
		}

		private float GetPerkMulRange()
		{
			return (!IsTower()) ? 0f : PerkManager.GetTowerRange(prefabID);
		}

		private float GetPerkMulAOERadius()
		{
			return (!IsTower()) ? 0f : PerkManager.GetTowerAOERadius(prefabID);
		}

		private float GetPerkModHit()
		{
			return (!IsTower()) ? 0f : PerkManager.GetTowerHit(prefabID);
		}

		private float GetPerkModDodge()
		{
			return (!IsTower()) ? 0f : PerkManager.GetTowerDodge(prefabID);
		}

		private float GetPerkModCritChance()
		{
			return (!IsTower()) ? 0f : PerkManager.GetTowerCritChance(prefabID);
		}

		private float GetPerkModCritMul()
		{
			return (!IsTower()) ? 0f : PerkManager.GetTowerCritMultiplier(prefabID);
		}

		private float GetPerkModShieldBreak()
		{
			return (!IsTower()) ? 0f : PerkManager.GetTowerShieldBreakMultiplier(prefabID);
		}

		private float GetPerkModShieldPierce()
		{
			return (!IsTower()) ? 0f : PerkManager.GetTowerShieldPierceMultiplier(prefabID);
		}

		private Stun ModifyStunWithPerkBonus(Stun stun)
		{
			return (!IsTower()) ? stun : PerkManager.ModifyStunWithPerkBonus(stun.Clone(), prefabID);
		}

		private Slow ModifySlowWithPerkBonus(Slow slow)
		{
			return (!IsTower()) ? slow : PerkManager.ModifySlowWithPerkBonus(slow.Clone(), prefabID);
		}

		private Dot ModifyDotWithPerkBonus(Dot dot)
		{
			return (!IsTower()) ? dot : PerkManager.ModifyDotWithPerkBonus(dot.Clone(), prefabID);
		}

		private InstantKill ModifyInstantKillWithPerkBonus(InstantKill instKill)
		{
			return (!IsTower()) ? instKill : PerkManager.ModifyInstantKillWithPerkBonus(instKill.Clone(), prefabID);
		}

		public float GetFullHP()
		{
			return defaultHP * (1f + GetPerkMulHP());
		}

		public float GetFullShield()
		{
			return defaultShield * (1f + GetPerkMulShield());
		}

		private float GetHPRegenRate()
		{
			return HPRegenRate * (1f + GetPerkMulHPRegen());
		}

		private float GetShieldRegenRate()
		{
			return shieldRegenRate * (1f + GetPerkMulShieldRegen());
		}

		private float GetHPStaggerDuration()
		{
			return HPStaggerDuration * (1f - GetPerkMulHPStagger());
		}

		private float GetShieldStaggerDuration()
		{
			return shieldStaggerDuration * (1f - GetPerkMulShieldStagger());
		}

		public float GetDamageMin()
		{
			return Mathf.Max(0f, stats[currentActiveStat].damageMin * (1f + damageBuffMul + dmgABMul + GetPerkMulDamage()));
		}

		public float GetDamageMax()
		{
			return Mathf.Max(0f, stats[currentActiveStat].damageMax * (1f + damageBuffMul + dmgABMul + GetPerkMulDamage()));
		}

		public float GetCooldown()
		{
			return Mathf.Max(0.05f, stats[currentActiveStat].cooldown * (1f - cooldownBuffMul - cdABMul - GetPerkMulCooldown()));
		}

		public float GetRangeMin()
		{
			return stats[currentActiveStat].minRange;
		}

		public float GetRange()
		{
			return Mathf.Max(0f, stats[currentActiveStat].range * (1f + rangeBuffMul + rangeABMul + GetPerkMulRange()));
		}

		public float GetAOERadius()
		{
			return stats[currentActiveStat].aoeRadius * (1f + GetPerkMulAOERadius());
		}

		public float GetHit()
		{
			return stats[currentActiveStat].hit + hitBuffMod + GetPerkModHit();
		}

		public float GetDodge()
		{
			return dodgeChance + dodgeBuffMod + GetPerkModDodge();
		}

		public float GetCritChance()
		{
			return stats[currentActiveStat].crit.chance + criticalBuffMod + GetPerkModCritChance();
		}

		public float GetCritMultiplier()
		{
			return stats[currentActiveStat].crit.dmgMultiplier + GetPerkModCritMul();
		}

		public float GetShieldBreak()
		{
			return stats[currentActiveStat].shieldBreak + GetPerkModShieldBreak();
		}

		public float GetShieldPierce()
		{
			return stats[currentActiveStat].shieldPierce + GetPerkModShieldPierce();
		}

		public bool DamageShieldOnly()
		{
			return stats[currentActiveStat].damageShieldOnly;
		}

		public Stun GetStun()
		{
			return ModifyStunWithPerkBonus(stats[currentActiveStat].stun);
		}

		public Slow GetSlow()
		{
			return ModifySlowWithPerkBonus(stats[currentActiveStat].slow);
		}

		public Dot GetDot()
		{
			return ModifyDotWithPerkBonus(stats[currentActiveStat].dot);
		}

		public InstantKill GetInstantKill()
		{
			return ModifyInstantKillWithPerkBonus(stats[currentActiveStat].instantKill);
		}

		public int GetShootPointCount()
		{
			return shootPoints.Count;
		}

		public ShootObject GetShootObject()
		{
			return (!(stats[currentActiveStat].shootObject != null)) ? localShootObject : stats[currentActiveStat].shootObject;
		}

		public GameObject GetEffectObject()
		{
			return stats[currentActiveStat].effectObject;
		}

		protected void SpawnEffectObject()
		{
			GameObject effectObject = GetEffectObject();
			if (effectObject != null)
			{
				if (!stats[currentActiveStat].autoDestroyEffect)
				{
					ObjectPoolManager.Spawn(effectObject, thisT.position, thisT.rotation);
				}
				else
				{
					ObjectPoolManager.Spawn(effectObject, thisT.position, thisT.rotation, stats[currentActiveStat].effectDuration);
				}
			}
		}

		public List<int> GetResourceGain()
		{
			return stats[currentActiveStat].rscGain;
		}

		public Buff GetBuff()
		{
			return stats[currentActiveStat].buff;
		}

		public string GetDespGeneral()
		{
			return desp;
		}

		public virtual float GetDistFromDestination()
		{
			return 0f;
		}

		public virtual bool IsInConstruction()
		{
			return false;
		}

		public void ABBuffDamage(float value, float duration)
		{
			StartCoroutine(ABBuffDamageRoutine(value, duration));
		}

		private IEnumerator ABBuffDamageRoutine(float value, float duration)
		{
			dmgABMul += value;
			yield return new WaitForSeconds(duration);
			dmgABMul -= value;
		}

		public void ABBuffRange(float value, float duration)
		{
			StartCoroutine(ABBuffDamageRoutine(value, duration));
		}

		private IEnumerator ABBuffRangeRoutine(float value, float duration)
		{
			rangeABMul += value;
			yield return new WaitForSeconds(duration);
			rangeABMul -= value;
		}

		public void ABBuffCooldown(float value, float duration)
		{
			StartCoroutine(ABBuffCooldownRoutine(value, duration));
		}

		private IEnumerator ABBuffCooldownRoutine(float value, float duration)
		{
			cdABMul += value;
			yield return new WaitForSeconds(duration);
			cdABMul -= value;
		}

		private void OnDrawGizmos()
		{
			if (target != null && IsCreep())
			{
				Gizmos.DrawLine(base.transform.position, target.transform.position);
			}
		}

		public void SetUnitAnimation(UnitAnimation uAnim)
		{
			uAnimation = uAnim;
		}

		public void PlayAnimMove(float speed)
		{
			if (uAnimation != null)
			{
				uAnimation.PlayMove(speed);
			}
		}

		public void PlayAnimSpawn()
		{
			if (uAnimation != null)
			{
				uAnimation.PlaySpawn();
			}
		}

		public void PlayAnimHit()
		{
			if (uAnimation != null)
			{
				uAnimation.PlayHit();
			}
		}

		public float PlayAnimDestroyed()
		{
			return (!(uAnimation != null)) ? 0f : uAnimation.PlayDestroyed();
		}

		public float PlayAnimDestination()
		{
			return (!(uAnimation != null)) ? 0f : uAnimation.PlayDestination();
		}

		public void PlayAnimConstruct()
		{
			if (uAnimation != null)
			{
				uAnimation.PlayConstruct();
			}
		}

		public void PlayAnimDeconstruct()
		{
			if (uAnimation != null)
			{
				uAnimation.PlayDeconstruct();
			}
		}

		public float PlayAnimAttack()
		{
			return (!(uAnimation != null)) ? 0f : uAnimation.PlayAttack();
		}
	}
}
