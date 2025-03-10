using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TDTK
{
	public class UnitCreep : Unit
	{
		[Header("Creep Setting")]
		public _CreepType type;

		[HideInInspector]
		public int waveID;

		public bool flying;

		public bool stopToAttack;

		public float rotateSpd = 12f;

		public float moveSpeed = 3f;

		public bool rotateTowardsDestination = true;

		public bool rotateTowardsDestinationX = true;

		[Header("Player Lost Upon Creep Scored")]
		public int lifeCost = 1;

		[Header("Player Gain Upon Creep Destroyed")]
		public int lifeValue;

		public List<int> valueRscMin = new List<int>();

		public List<int> valueRscMax = new List<int>();

		public int valueEnergyGain;

		[Header("Spawn Upon Destroyed")]
		public UnitCreep spawnUponDestroyed;

		public int spawnUponDestroyedCount;

		public float spawnUnitHPMultiplier = 0.5f;

		private Vector3 pathDynamicOffset;

		[Header("Visual Effects")]
		public GameObject destinationEffObj;

		public bool autoDestroydestinationEff = true;

		public float destinationEffDuration = 1f;

		[HideInInspector]
		public PathTD path;

		[HideInInspector]
		public List<Vector3> subPath = new List<Vector3>();

		[HideInInspector]
		public int waypointID = 1;

		[HideInInspector]
		public int subWaypointID;

		public SubPath okpath;

		private static Transform dummyT;

		private float distFromDestination;

		public Vector3 GetPathDynamicOffset()
		{
			return pathDynamicOffset;
		}

		public override void Awake()
		{
			isCreep = true;
			if (!flying)
			{
				base.gameObject.layer = TDTK.GetLayerCreep();
			}
			else
			{
				base.gameObject.layer = TDTK.GetLayerCreepF();
			}
			base.Awake();
			if (thisObj.GetComponent<Collider>() == null)
			{
				thisObj.AddComponent<SphereCollider>();
			}
		}

		public override void Start()
		{
			base.Start();
		}

		public void Init(PathTD p, int ID, int wID, UnitCreep parentUnit = null)
		{
			Init();
			path = p;
			instanceID = ID;
			waveID = wID;
			float x = Random.Range(0f - path.dynamicOffset, path.dynamicOffset);
			float z = Random.Range(0f - path.dynamicOffset, path.dynamicOffset);
			pathDynamicOffset = new Vector3(x, 0f, z);
			thisT.position += pathDynamicOffset;
			if (parentUnit == null)
			{
				waypointID = 1;
				subWaypointID = 0;
				SetSubPath(path.GetWPSectionPath(waypointID));
			}
			else
			{
				waypointID = parentUnit.waypointID;
				subWaypointID = parentUnit.subWaypointID;
				SetSubPath(parentUnit.subPath);
				defaultHP = parentUnit.defaultHP * parentUnit.spawnUnitHPMultiplier;
				HP = GetFullHP();
				defaultShield = parentUnit.defaultShield * parentUnit.spawnUnitHPMultiplier;
				shield = GetFullShield();
			}
			distFromDestination = CalculateDistFromDestination();
			if (type == _CreepType.Offense)
			{
				maskTarget = 1 << TDTK.GetLayerTower();
				StartCoroutine(ScanForTargetRoutine());
				StartCoroutine(TurretRoutine());
			}
			if (type == _CreepType.Support)
			{
				LayerMask mask = 1 << TDTK.GetLayerCreep();
				LayerMask mask2 = 1 << TDTK.GetLayerCreepF();
				maskTarget = ((int)mask | (int)mask2);
				StartCoroutine(SupportRoutine());
			}
			int resourceCount = ResourceManager.GetResourceCount();
			while (valueRscMin.Count < resourceCount)
			{
				valueRscMin.Add(0);
			}
			while (valueRscMin.Count > resourceCount)
			{
				valueRscMin.RemoveAt(valueRscMin.Count - 1);
			}
			while (valueRscMax.Count < resourceCount)
			{
				valueRscMax.Add(0);
			}
			while (valueRscMax.Count > resourceCount)
			{
				valueRscMax.RemoveAt(valueRscMax.Count - 1);
			}
			PlayAnimSpawn();
		}

		public override void Update()
		{
			base.Update();
		}

		public override void OnEnable()
		{
			SubPath.onPathChangedE += OnSubPathChanged;
			BuildManager.OnbuildTower += OnBuild;
		}

		public override void OnDisable()
		{
			SubPath.onPathChangedE -= OnSubPathChanged;
			BuildManager.OnbuildTower += OnBuild;
		}

		private IEnumerator ResetPath(SubPath PathChange)
		{
			yield return new WaitForSeconds(0.2f);
			if (!IsDestroyed() && PathChange.parentPath == path && PathChange.wpIDPlatform == waypointID)
			{
				ResetSubPath(PathChange);
				okpath = null;
			}
		}

		public void OnBuild()
		{
			if (okpath != null && !IsDestroyed() && this != null)
			{
				StartCoroutine(ResetPath(okpath));
			}
		}

		private void OnSubPathChanged(SubPath platformSubPath)
		{
			if (!flying && platformSubPath.parentPath == path && platformSubPath.wpIDPlatform == waypointID)
			{
				ResetSubPath(platformSubPath);
				okpath = platformSubPath;
			}
		}

		private void ResetSubPath(SubPath platformSubPath)
		{
			if (dummyT == null)
			{
				dummyT = new GameObject().transform;
			}
			Quaternion rotation = Quaternion.LookRotation(subPath[subWaypointID] - thisT.position);
			dummyT.rotation = rotation;
			dummyT.position = thisT.position;
			Vector3 point = dummyT.TransformPoint(0f, 0f, BuildManager.GetGridSize() / 2f);
			NodeTD nearestNode = PathFinder.GetNearestNode(point, platformSubPath.parentPlatform.GetNodeGraph());
			PathFinder.GetPath(nearestNode, platformSubPath.endN, platformSubPath.parentPlatform.GetNodeGraph(), PathFinderCallback);
		}

		private void PathFinderCallback(List<Vector3> pathList)
		{
			SetSubPath(pathList);
			subWaypointID = 0;
			distFromDestination = CalculateDistFromDestination();
		}

		private void SetSubPath(List<Vector3> pathList)
		{
			subPath = pathList;
			if (flying && subPath.Count > 1)
			{
				subPath = new List<Vector3>
				{
					subPath[0],
					subPath[subPath.Count - 1]
				};
			}
		}

		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (stunned || destroyed || !MoveToPoint(subPath[subWaypointID]))
			{
				return;
			}
			subWaypointID++;
			if (subWaypointID >= subPath.Count)
			{
				subWaypointID = 0;
				waypointID++;
				if (waypointID >= path.GetPathWPCount())
				{
					ReachDestination();
				}
				else
				{
					SetSubPath(path.GetWPSectionPath(waypointID));
				}
			}
		}

		public bool MoveToPoint(Vector3 point)
		{
			if (type == _CreepType.Offense && stopToAttack && target != null)
			{
				return false;
			}
			point += pathDynamicOffset;
			float num = Vector3.Distance(point, thisT.position);
			if (num < 0.005f)
			{
				return true;
			}
			if (rotateTowardsDestination && moveSpeed > 0f)
			{
				if (rotateTowardsDestinationX)
				{
					Quaternion b = Quaternion.LookRotation(point - thisT.position);
					thisT.rotation = Quaternion.Slerp(thisT.rotation, b, rotateSpd * Time.fixedDeltaTime);
				}
				else
				{
					float x = point.x;
					Vector3 position = thisT.position;
					Vector3 a = new Vector3(x, position.y, point.z);
					Quaternion b2 = Quaternion.LookRotation(a - thisT.position);
					thisT.rotation = Quaternion.Slerp(thisT.rotation, b2, rotateSpd * Time.fixedDeltaTime);
				}
			}
			Vector3 normalized = (point - thisT.position).normalized;
			thisT.Translate(normalized * Mathf.Min(num, moveSpeed * slowMultiplier * Time.fixedDeltaTime), Space.World);
			distFromDestination -= moveSpeed * slowMultiplier * Time.fixedDeltaTime;
			return false;
		}

		private void LateUpdate()
		{
			PlayAnimMove(moveSpeed * slowMultiplier);
		}

		private void ReachDestination()
		{
			if (destinationEffObj != null)
			{
				if (!autoDestroydestinationEff)
				{
					ObjectPoolManager.Spawn(destinationEffObj, thisT.position, thisT.rotation);
				}
				else
				{
					ObjectPoolManager.Spawn(destinationEffObj, thisT.position, thisT.rotation, destinationEffDuration);
				}
			}
			GameControl.OnCreepReachDestination(this);
			SpawnManager.OnCreepReachDestination(this);
			TDTK.OnCreepDestination(this);
			if (path.loop)
			{
				if (!path.IsLinearPath())
				{
					subWaypointID = 0;
					waypointID = path.GetLoopPoint();
					SetSubPath(path.GetWPSectionPath(waypointID));
				}
				else
				{
					waypointID = path.GetLoopPoint();
				}
			}
			else
			{
				destroyed = true;
				StartCoroutine(_ReachDestination(PlayAnimDestination()));
			}
		}

		private IEnumerator _ReachDestination(float duration)
		{
			yield return new WaitForSeconds(duration);
			ObjectPoolManager.Unspawn(thisObj);
		}

		public override void Destroyed(float delay = 0f)
		{
			if (destroyed)
			{
				return;
			}
			destroyed = true;
			List<int> list = new List<int>();
			for (int i = 0; i < valueRscMin.Count; i++)
			{
				list.Add(Random.Range(valueRscMin[i], valueRscMax[i]));
			}
			ResourceManager.GainResource(list, PerkManager.GetRscCreepKilled());
			AbilityManager.GainEnergy(valueEnergyGain + (int)PerkManager.GetEnergyWaveClearedModifier());
			if (lifeValue > 0)
			{
				GameControl.GainLife(lifeValue);
			}
			if (spawnUponDestroyed != null && spawnUponDestroyedCount > 0)
			{
				for (int j = 0; j < spawnUponDestroyedCount; j++)
				{
					Vector3 b = new Vector3(Random.Range(-0.5f, 0.5f), 0f, Random.Range(-0.5f, 0.5f));
					GameObject gameObject = ObjectPoolManager.Spawn(spawnUponDestroyed.gameObject, thisT.position + b, thisT.rotation);
					UnitCreep component = gameObject.GetComponent<UnitCreep>();
					component.waveID = waveID;
					int iD = SpawnManager.AddDestroyedSpawn(component);
					component.Init(path, iD, waveID, this);
				}
			}
			SpawnManager.OnUnitDestroyed(this);
			TDTK.OnUnitCreepDestroyed(this);
			base.Destroyed(PlayAnimDestroyed());
		}

		public float GetMoveSpeed()
		{
			return moveSpeed * slowMultiplier;
		}

		public override float GetDistFromDestination()
		{
			return distFromDestination;
		}

		public float CalculateDistFromDestination()
		{
			float num = Vector3.Distance(thisT.position, subPath[subWaypointID]);
			for (int i = subWaypointID + 1; i < subPath.Count; i++)
			{
				num += Vector3.Distance(subPath[i - 1], subPath[i]);
			}
			return num + path.GetPathDistance(waypointID + 1);
		}
	}
}
