using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TDTK
{
	public class PathIndicator : MonoBehaviour
	{
		public PathTD path;

		public ParticleSystem pSystem;

		private Transform indicatorT;

		public float delayBeforeStart = 2f;

		public float speed = 5f;

		public float updateRate = 0.1f;

		private List<Vector3> subPath = new List<Vector3>();

		private int waypointID = 1;

		private int subWaypointID;

		private bool moving;

		private void Start()
		{
			indicatorT = pSystem.transform;
			ParticleSystem.EmissionModule emission = pSystem.emission;
			ParticleSystem.MinMaxCurve rate = pSystem.emission.rate;
			rate.constantMax = 0f;
			emission.rate = rate;
			StartCoroutine(MoveRoutine());
		}

		private IEnumerator EmitRoutine()
		{
			while (moving)
			{
				yield return new WaitForSeconds(updateRate);
				ParticleSystem particleSystem = pSystem;
				Vector3 eulerAngles = indicatorT.rotation.eulerAngles;
				particleSystem.startRotation = eulerAngles.y * ((float)Math.PI / 180f);
				pSystem.Emit(1);
			}
		}

		private IEnumerator MoveRoutine()
		{
			Reset(initial: true);
			yield return new WaitForSeconds(delayBeforeStart);
			moving = true;
			StartCoroutine(EmitRoutine());
			while (true)
			{
				if (MoveToPoint(indicatorT, subPath[subWaypointID]))
				{
					subWaypointID++;
					if (subWaypointID >= subPath.Count)
					{
						subWaypointID = 0;
						waypointID++;
						if (waypointID >= path.GetPathWPCount())
						{
							Reset();
						}
						else
						{
							subPath = path.GetWPSectionPath(waypointID);
						}
					}
				}
				yield return null;
			}
		}

		public bool MoveToPoint(Transform particleT, Vector3 point)
		{
			float num = Vector3.Distance(point, indicatorT.position);
			indicatorT.LookAt(point);
			indicatorT.Translate(Vector3.forward * Mathf.Min(speed * Time.deltaTime, num));
			if (num < 0.1f)
			{
				return true;
			}
			return false;
		}

		private void OnEnable()
		{
			SubPath.onPathChangedE += OnSubPathChanged;
		}

		private void OnDisable()
		{
			SubPath.onPathChangedE -= OnSubPathChanged;
		}

		private void OnSubPathChanged(SubPath platformSubPath)
		{
			if (platformSubPath.parentPath == path && platformSubPath.wpIDPlatform == waypointID)
			{
				subPath = path.GetWPSectionPath(waypointID);
				subWaypointID = Mathf.Min(subWaypointID, subPath.Count - 1);
			}
		}

		private void Reset(bool initial = false)
		{
			if (path.loop && !initial)
			{
				waypointID = path.GetLoopPoint();
			}
			else
			{
				waypointID = 1;
			}
			subWaypointID = 0;
			subPath = path.GetWPSectionPath(waypointID);
			if (!path.loop || initial)
			{
				indicatorT.position = path.GetSpawnPoint();
			}
		}
	}
}
