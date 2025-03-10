using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TDTK
{
	public class ShootObject : MonoBehaviour
	{
		public _ShootObjectType type;

		public float speed = 5f;

		public float beamDuration = 0.5f;

		private Transform shootPoint;

		private bool hit;

		public bool autoSearchLineRenderer = true;

		public List<LineRenderer> lineList = new List<LineRenderer>();

		private List<TrailRenderer> trailList = new List<TrailRenderer>();

		public GameObject shootEffect;

		public bool destroyShootEffect = true;

		public float destroyShootDuration = 1.5f;

		public GameObject hitEffect;

		public bool destroyHitEffect = true;

		public float destroyHitDuration = 1.5f;

		public GameObject destroyEffect;

		public bool destroyDestroyEffect = true;

		public float destroyDestroyDuration = 1.5f;

		private AttackInstance attInstance;

		private GameObject thisObj;

		private Transform thisT;

		public float hitRadius = 0.1f;

		private Unit target;

		private Vector3 targetPos;

		public float maxShootAngle = 30f;

		public float maxShootRange = 0.5f;

		private float hitThreshold = 0.15f;

		public float shootAngleY = 20f;

		private float missileSpeedModifier = 1f;

		private void Awake()
		{
			thisObj = base.gameObject;
			thisT = base.transform;
			thisObj.layer = TDTK.GetLayerShootObject();
			if (autoSearchLineRenderer)
			{
				LineRenderer[] componentsInChildren = thisObj.GetComponentsInChildren<LineRenderer>(includeInactive: true);
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					lineList.Add(componentsInChildren[i]);
				}
			}
			TrailRenderer[] componentsInChildren2 = thisObj.GetComponentsInChildren<TrailRenderer>(includeInactive: true);
			for (int j = 0; j < componentsInChildren2.Length; j++)
			{
				trailList.Add(componentsInChildren2[j]);
			}
			if (type == _ShootObjectType.FPSProjectile)
			{
				SphereCollider sphereCollider = GetComponent<SphereCollider>();
				if (sphereCollider == null)
				{
					sphereCollider = thisObj.AddComponent<SphereCollider>();
					sphereCollider.radius = 0.15f;
				}
				hitRadius = sphereCollider.radius;
			}
			if (shootEffect != null)
			{
				ObjectPoolManager.New(shootEffect);
			}
			if (hitEffect != null)
			{
				ObjectPoolManager.New(hitEffect);
			}
			if (destroyEffect != null)
			{
				ObjectPoolManager.New(destroyEffect);
			}
		}

		private void OnEnable()
		{
		}

		private void OnDisable()
		{
		}

		public void Shoot(AttackInstance attInst = null, Transform sp = null)
		{
			if (attInst.tgtUnit == null || attInst.tgtUnit.GetTargetT() == null)
			{
				ObjectPoolManager.Unspawn(thisObj);
				return;
			}
			attInstance = attInst;
			target = attInstance.tgtUnit;
			targetPos = target.GetTargetT().position;
			hitThreshold = Mathf.Max(0.1f, target.hitThreshold);
			shootPoint = sp;
			if (shootPoint != null)
			{
				thisT.rotation = shootPoint.rotation;
			}
			ShootEffect();
			hit = false;
			if (type == _ShootObjectType.Projectile)
			{
				StartCoroutine(ProjectileRoutine());
			}
			if (type == _ShootObjectType.Beam)
			{
				StartCoroutine(BeamRoutine());
			}
			if (type == _ShootObjectType.Missile)
			{
				StartCoroutine(MissileRoutine());
			}
			if (type == _ShootObjectType.Effect)
			{
				StartCoroutine(EffectRoutine());
			}
		}

		public void ShootFPS(AttackInstance attInst = null, Transform sp = null)
		{
			shootPoint = sp;
			if (shootPoint != null)
			{
				thisT.rotation = shootPoint.rotation;
			}
			ShootEffect();
			hit = false;
			attInstance = attInst;
			if (type == _ShootObjectType.FPSProjectile)
			{
				StartCoroutine(FPSProjectileRoutine());
			}
			if (type == _ShootObjectType.FPSBeam)
			{
				StartCoroutine(FPSBeamRoutine(sp));
			}
			if (type == _ShootObjectType.FPSEffect)
			{
				StartCoroutine(FPSEffectRoutine());
			}
		}

		private IEnumerator FPSEffectRoutine()
		{
			yield return new WaitForSeconds(0.05f);
			if (Physics.SphereCast(direction: thisT.TransformDirection(new Vector3(0f, 0f, 1f)), origin: thisT.position, radius: hitRadius / 2f, hitInfo: out RaycastHit raycastHit))
			{
				Unit component = raycastHit.transform.GetComponent<Unit>();
				FPSHit(component, raycastHit.point);
				HitEffect(raycastHit.point);
			}
			yield return new WaitForSeconds(0.1f);
			ObjectPoolManager.Unspawn(thisObj);
		}

		private IEnumerator FPSBeamRoutine(Transform sp)
		{
			thisT.parent = sp;
			float duration = 0f;
			while (duration < beamDuration)
			{
				if (Physics.SphereCast(direction: thisT.TransformDirection(new Vector3(0f, 0f, 1f)), origin: thisT.position, radius: hitRadius, hitInfo: out RaycastHit raycastHit) && !hit)
				{
					hit = true;
					Unit component = raycastHit.transform.GetComponent<Unit>();
					FPSHit(component, raycastHit.point);
					HitEffect(raycastHit.point);
				}
				float lineDist = (raycastHit.distance != 0f) ? raycastHit.distance : 9999f;
				for (int i = 0; i < lineList.Count; i++)
				{
					lineList[i].SetPosition(1, new Vector3(0f, 0f, lineDist));
				}
				duration += Time.fixedDeltaTime;
				yield return new WaitForSeconds(Time.fixedDeltaTime);
			}
			thisT.parent = null;
			ObjectPoolManager.Unspawn(thisObj);
		}

		private IEnumerator FPSProjectileRoutine()
		{
			float timeShot = Time.time;
			while (true)
			{
				Vector3 dir = thisT.TransformDirection(new Vector3(0f, 0f, 1f));
				float travelDist = speed * Time.fixedDeltaTime;
				if (Physics.SphereCast(thisT.position, hitRadius, dir, out RaycastHit raycastHit, travelDist))
				{
					travelDist = raycastHit.distance + hitRadius;
				}
				thisT.Translate(Vector3.forward * travelDist);
				if (Time.time - timeShot > 5f)
				{
					break;
				}
				yield return new WaitForSeconds(Time.fixedDeltaTime);
			}
			ObjectPoolManager.Unspawn(thisObj);
			yield return null;
		}

		private void OnTriggerEnter(Collider collider)
		{
			if (hit || type == _ShootObjectType.FPSProjectile)
			{
				hit = true;
				attInstance.impactPoint = thisT.position;
				Unit component = collider.gameObject.GetComponent<Unit>();
				FPSHit(component, thisT.position);
				HitEffect(thisT.position);
				ObjectPoolManager.Unspawn(thisObj);
			}
		}

		private void FPSHit(Unit hitUnit, Vector3 hitPoint)
		{
			if (attInstance.srcWeapon.GetAOERange() > 0f)
			{
				LayerMask mask = 1 << TDTK.GetLayerCreep();
				LayerMask mask2 = 1 << TDTK.GetLayerCreepF();
				LayerMask mask3 = (int)mask | (int)mask2;
				Collider[] array = Physics.OverlapSphere(hitPoint, attInstance.srcWeapon.GetAOERange(), mask3);
				if (array.Length <= 0)
				{
					return;
				}
				List<Unit> list = new List<Unit>();
				for (int i = 0; i < array.Length; i++)
				{
					Unit component = array[i].gameObject.GetComponent<Unit>();
					if (!component.IsDestroyed())
					{
						list.Add(component);
					}
				}
				if (list.Count > 0)
				{
					for (int j = 0; j < list.Count; j++)
					{
						AttackInstance attackInstance = new AttackInstance();
						attackInstance.srcWeapon = attInstance.srcWeapon;
						attackInstance.tgtUnit = list[j];
						attackInstance.Process();
						list[j].ApplyEffect(attackInstance);
					}
				}
			}
			else if (hitUnit != null && hitUnit.IsCreep())
			{
				attInstance.tgtUnit = hitUnit;
				attInstance.Process();
				hitUnit.ApplyEffect(attInstance);
			}
		}

		public float GetMaxShootRange()
		{
			if (type == _ShootObjectType.Projectile || type == _ShootObjectType.Missile)
			{
				return maxShootRange;
			}
			return 1f;
		}

		public float GetMaxShootAngle()
		{
			if (type == _ShootObjectType.Projectile || type == _ShootObjectType.Missile)
			{
				return maxShootAngle;
			}
			return 0f;
		}

		private IEnumerator EffectRoutine()
		{
			yield return new WaitForSeconds(0.125f);
			Hit();
		}

		private IEnumerator BeamRoutine()
		{
			float timeShot = Time.time;
			Vector3 shootP = shootPoint.position;
			while (true)
			{
				if (!hit)
				{
					if (target != null)
					{
						targetPos = target.GetTargetT().position;
					}
					if (shootPoint != null)
					{
						shootP = shootPoint.position;
					}
					float dist = Vector3.Distance(shootP, targetPos);
					Vector3 targetPosition = new Ray(shootP, targetPos - shootP).GetPoint(dist - hitThreshold);
					for (int i = 0; i < lineList.Count; i++)
					{
						lineList[i].SetPosition(0, shootP);
						lineList[i].SetPosition(1, targetPosition);
					}
					if (Time.time - timeShot > beamDuration)
					{
						break;
					}
					yield return null;
					continue;
				}
				yield break;
			}
			Hit();
		}

		private IEnumerator ProjectileRoutine()
		{
			ShootEffect();
			float timeShot = Time.time;
			thisT.LookAt(targetPos);
			float angle = Mathf.Min(1f, Vector3.Distance(thisT.position, targetPos) / maxShootRange) * maxShootAngle;
			thisT.rotation *= Quaternion.Euler(0f - angle, 0f, 0f);
			Vector3 startPos = thisT.position;
			Vector3 eulerAngles = thisT.rotation.eulerAngles;
			float iniRotX = eulerAngles.x;
			float y = Mathf.Min(targetPos.y, startPos.y);
			float totalDist = Vector3.Distance(startPos, targetPos);
			while (true)
			{
				if (hit)
				{
					yield break;
				}
				if (target != null)
				{
					targetPos = target.GetTargetT().position;
				}
				Vector3 curPos = thisT.position;
				curPos.y = y;
				float currentDist = Vector3.Distance(curPos, targetPos);
				float curDist2 = Vector3.Distance(thisT.position, targetPos);
				if (curDist2 < hitThreshold && !hit)
				{
					Hit();
					yield break;
				}
				if (Time.time - timeShot < 3.5f)
				{
					float t = 1f - Mathf.Min(1f, currentDist / totalDist);
					Vector3 vector = targetPos - thisT.position;
					if (vector != Vector3.zero)
					{
						Quaternion quaternion = Quaternion.LookRotation(vector);
						float a = iniRotX;
						Vector3 eulerAngles2 = quaternion.eulerAngles;
						float num = Mathf.LerpAngle(a, eulerAngles2.x, t);
						Transform transform = thisT;
						float x = num;
						Vector3 eulerAngles3 = quaternion.eulerAngles;
						float y2 = eulerAngles3.y;
						Vector3 eulerAngles4 = quaternion.eulerAngles;
						transform.rotation = Quaternion.Euler(x, y2, eulerAngles4.z);
					}
				}
				else
				{
					thisT.LookAt(targetPos);
				}
				thisT.Translate(Vector3.forward * Mathf.Min(speed * Time.deltaTime, curDist2));
				curDist2 = Vector3.Distance(thisT.position, targetPos);
				if (curDist2 < hitThreshold && !hit)
				{
					break;
				}
				yield return new WaitForSeconds(Time.fixedDeltaTime);
			}
			Hit();
		}

		private IEnumerator MissileSpeedRoutine()
		{
			missileSpeedModifier = 0.05f;
			float duration = 0f;
			while (duration < 1f)
			{
				missileSpeedModifier = Mathf.Sin(Mathf.Sin(duration * (float)Math.PI / 2f) * (float)Math.PI / 2f);
				duration += Time.deltaTime * 1f;
				yield return null;
			}
			missileSpeedModifier = 1f;
		}

		private IEnumerator MissileRoutine()
		{
			StartCoroutine(MissileSpeedRoutine());
			float angleX = UnityEngine.Random.Range(maxShootAngle / 2f, maxShootAngle);
			float angleY = UnityEngine.Random.Range(shootAngleY / 2f, maxShootAngle);
			if (UnityEngine.Random.Range(0f, 1f) > 0.5f)
			{
				angleY *= -1f;
			}
			thisT.LookAt(targetPos);
			thisT.rotation = thisT.rotation;
			Quaternion wantedRotation = thisT.rotation * Quaternion.Euler(0f - angleX, angleY, 0f);
			float rand = UnityEngine.Random.Range(4f, 10f);
			float totalDist = Vector3.Distance(thisT.position, targetPos);
			float estimateTime = totalDist / speed;
			float shootTime = Time.time;
			Vector3 startPos = thisT.position;
			while (true)
			{
				if (hit)
				{
					yield break;
				}
				if (target != null)
				{
					targetPos = target.GetTargetT().position;
				}
				float currentDist2 = Vector3.Distance(thisT.position, targetPos);
				float delta = totalDist - Vector3.Distance(startPos, targetPos);
				float eTime = estimateTime - delta / speed;
				if (Time.time - shootTime > eTime)
				{
					Vector3 vector = targetPos - thisT.position;
					if (vector != Vector3.zero)
					{
						wantedRotation = Quaternion.LookRotation(vector);
						float num = Time.time - shootTime - eTime;
						thisT.rotation = Quaternion.Slerp(thisT.rotation, wantedRotation, num / (eTime * currentDist2));
					}
				}
				else
				{
					thisT.rotation = Quaternion.Slerp(thisT.rotation, wantedRotation, Time.fixedDeltaTime * rand);
				}
				if (currentDist2 < hitThreshold)
				{
					Hit();
					yield break;
				}
				thisT.Translate(Vector3.forward * Mathf.Min(speed * Time.fixedDeltaTime * missileSpeedModifier, currentDist2));
				currentDist2 = Vector3.Distance(thisT.position, targetPos);
				if (currentDist2 < hitThreshold && !hit)
				{
					break;
				}
				yield return new WaitForSeconds(Time.fixedDeltaTime);
			}
			Hit();
		}

		private void Hit()
		{
			hit = true;
			HitEffect(targetPos);
			attInstance.impactPoint = targetPos;
			if (attInstance.srcUnit.GetAOERadius() > 0f)
			{
				LayerMask targetMask = attInstance.srcUnit.GetTargetMask();
				Collider[] array = Physics.OverlapSphere(targetPos, attInstance.srcUnit.GetAOERadius(), targetMask);
				if (array.Length > 0)
				{
					List<Unit> list = new List<Unit>();
					for (int i = 0; i < array.Length; i++)
					{
						Unit component = array[i].gameObject.GetComponent<Unit>();
						if (!component.IsDestroyed())
						{
							list.Add(component);
						}
					}
					if (list.Count > 0)
					{
						for (int j = 0; j < list.Count; j++)
						{
							if (list[j] == target)
							{
								target.ApplyEffect(attInstance);
								if (attInstance.destroy)
								{
									DestroyEffect(targetPos);
								}
								continue;
							}
							AttackInstance attackInstance = new AttackInstance();
							attackInstance.srcUnit = attInstance.srcUnit;
							attackInstance.tgtUnit = list[j];
							attackInstance.Process();
							list[j].ApplyEffect(attackInstance);
							if (attackInstance.destroy)
							{
								DestroyEffect(list[j].thisT.position);
							}
						}
					}
				}
			}
			else
			{
				if (target != null)
				{
					target.ApplyEffect(attInstance);
				}
				if (attInstance.destroy)
				{
					DestroyEffect(targetPos);
				}
			}
			ObjectPoolManager.Unspawn(thisObj);
		}

		private void ShootEffect()
		{
			if (shootEffect != null)
			{
				if (!destroyShootEffect)
				{
					ObjectPoolManager.Spawn(shootEffect, thisT.position, thisT.rotation);
				}
				else
				{
					ObjectPoolManager.Spawn(shootEffect, thisT.position, thisT.rotation, destroyShootDuration);
				}
			}
		}

		private void HitEffect(Vector3 tgtPos)
		{
			if (hitEffect != null)
			{
				if (!destroyHitEffect)
				{
					ObjectPoolManager.Spawn(hitEffect, tgtPos, thisT.rotation);
				}
				else
				{
					ObjectPoolManager.Spawn(hitEffect, tgtPos, thisT.rotation, destroyHitDuration);
				}
			}
		}

		private void DestroyEffect(Vector3 tgtPos)
		{
			if (destroyEffect != null)
			{
				if (!destroyDestroyEffect)
				{
					ObjectPoolManager.Spawn(destroyEffect, tgtPos, thisT.rotation);
				}
				else
				{
					ObjectPoolManager.Spawn(destroyEffect, tgtPos, thisT.rotation, destroyDestroyDuration);
				}
			}
		}

		private IEnumerator ClearTrail(TrailRenderer trail)
		{
			if (!(trail == null))
			{
				float trailDuration = trail.time;
				trail.time = -1f;
				yield return null;
				trail.time = trailDuration;
			}
		}
	}
}
