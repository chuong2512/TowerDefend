using System.Collections.Generic;
using UnityEngine;

namespace TDTK
{
	public class PathTD : MonoBehaviour
	{
		[HideInInspector]
		private bool isLinearPath = true;

		public List<Transform> wpList = new List<Transform>();

		public List<WPSection> wpSectionList = new List<WPSection>();

		public bool createPathLine = true;

		public float dynamicOffset = 1f;

		public bool loop;

		public int loopPoint;

		public bool showGizmo = true;

		public Color gizmoColor = Color.blue;

		public bool IsLinearPath()
		{
			return isLinearPath;
		}

		public void Init()
		{
			wpSectionList = new List<WPSection>();
			for (int i = 0; i < wpList.Count; i++)
			{
				Transform transform = wpList[i];
				if (transform != null)
				{
					WPSection wPSection = new WPSection(transform);
					if (transform.gameObject.layer == TDTK.GetLayerPlatform())
					{
						wPSection.isPlatform = true;
						wPSection.platform = transform.gameObject.GetComponent<PlatformTD>();
						wPSection.pathIDOnPlatform = wPSection.platform.AddSubPath(this, i, wpList[i - 1], wpList[i + 1]);
						if (isLinearPath)
						{
							isLinearPath = false;
						}
					}
					else
					{
						WPSubPath component = transform.gameObject.GetComponent<WPSubPath>();
						if (component != null)
						{
							wPSection.SetPosList(new List<Vector3>(component.posList));
						}
						else
						{
							wPSection.SetPosList(new List<Vector3>
							{
								transform.position
							});
						}
					}
					wpSectionList.Add(wPSection);
				}
				else
				{
					wpList.RemoveAt(i);
					i--;
				}
			}
			if (loop)
			{
				loopPoint = Mathf.Min(wpList.Count - 1, loopPoint);
			}
		}

		public List<Vector3> GetWPSectionPath(int ID)
		{
			if (wpSectionList[ID].isPlatform)
			{
				WPSection wPSection = wpSectionList[ID];
				return wPSection.platform.GetSubPathPath(wPSection.pathIDOnPlatform);
			}
			return new List<Vector3>(wpSectionList[ID].GetPosList());
		}

		public int GetPathWPCount()
		{
			return wpList.Count;
		}

		public Vector3 GetSpawnPoint()
		{
			return wpSectionList[0].GetStartPos();
		}

		public Quaternion GetSpawnDirection()
		{
			return wpSectionList[0].waypointT.rotation;
		}

		public bool ReachEndOfPath(int ID)
		{
			return ID >= wpSectionList.Count;
		}

		public int GetLoopPoint()
		{
			return loopPoint;
		}

		public float GetPathDistance(int wpID = 1)
		{
			if (wpList.Count == 0)
			{
				return 0f;
			}
			float num = 0f;
			if (Application.isPlaying)
			{
				Vector3 a = wpSectionList[wpID - 1].GetEndPos();
				for (int i = wpID; i < wpSectionList.Count; i++)
				{
					List<Vector3> wPSectionPath = GetWPSectionPath(i);
					num += Vector3.Distance(a, wPSectionPath[0]);
					for (int j = 1; j < wPSectionPath.Count; j++)
					{
						num += Vector3.Distance(wPSectionPath[j - 1], wPSectionPath[j]);
					}
					a = wPSectionPath[wPSectionPath.Count - 1];
				}
			}
			else
			{
				for (int k = wpID; k < wpList.Count; k++)
				{
					num += Vector3.Distance(wpList[k - 1].position, wpList[k].position);
				}
			}
			return num;
		}

		private void Start()
		{
			if (createPathLine)
			{
				CreatePathLine();
			}
		}

		private void CreatePathLine()
		{
			Transform transform = new GameObject().transform;
			transform.position = base.transform.position;
			transform.parent = base.transform;
			transform.gameObject.name = "PathLine";
			GameObject original = (GameObject)Resources.Load("ScenePrefab/PathLine");
			GameObject original2 = (GameObject)Resources.Load("ScenePrefab/PathPoint");
			Vector3 position = Vector3.zero;
			Vector3 zero = Vector3.zero;
			SubPath subPath = null;
			for (int i = 0; i < wpSectionList.Count; i++)
			{
				WPSection wPSection = wpSectionList[i];
				if (!wPSection.isPlatform)
				{
					List<Vector3> posList = wPSection.GetPosList();
					for (int j = 0; j < posList.Count - 1; j++)
					{
						GameObject gameObject = UnityEngine.Object.Instantiate(original, posList[j], Quaternion.identity);
						LineRenderer component = gameObject.GetComponent<LineRenderer>();
						component.SetPosition(0, posList[j]);
						component.SetPosition(1, posList[j + 1]);
						gameObject.transform.parent = transform;
					}
					zero = wPSection.GetStartPos();
				}
				else
				{
					subPath = wPSection.platform.GetSubPath(wPSection.pathIDOnPlatform);
					GameObject gameObject2 = UnityEngine.Object.Instantiate(original2, subPath.startN.pos, Quaternion.identity);
					GameObject gameObject3 = UnityEngine.Object.Instantiate(original2, subPath.endN.pos, Quaternion.identity);
					zero = subPath.startN.pos;
					gameObject2.transform.parent = transform;
					gameObject3.transform.parent = transform;
				}
				if (i > 0)
				{
					GameObject gameObject4 = UnityEngine.Object.Instantiate(original, position, Quaternion.identity);
					LineRenderer component2 = gameObject4.GetComponent<LineRenderer>();
					component2.SetPosition(0, position);
					component2.SetPosition(1, zero);
					gameObject4.transform.parent = transform;
				}
				position = ((!wPSection.isPlatform) ? wPSection.GetEndPos() : subPath.endN.pos);
			}
		}

		private void OnDrawGizmos()
		{
			if (!showGizmo)
			{
				return;
			}
			Gizmos.color = gizmoColor;
			if (Application.isPlaying)
			{
				List<Vector3> wPSectionPath = GetWPSectionPath(0);
				for (int i = 1; i < wPSectionPath.Count; i++)
				{
					Gizmos.DrawLine(wPSectionPath[i - 1], wPSectionPath[i]);
				}
				for (int j = 1; j < wpSectionList.Count; j++)
				{
					List<Vector3> wPSectionPath2 = GetWPSectionPath(j - 1);
					List<Vector3> wPSectionPath3 = GetWPSectionPath(j);
					Gizmos.DrawLine(wPSectionPath2[wPSectionPath2.Count - 1], wPSectionPath3[0]);
					for (int k = 1; k < wPSectionPath3.Count; k++)
					{
						Gizmos.DrawLine(wPSectionPath3[k - 1], wPSectionPath3[k]);
					}
				}
				return;
			}
			for (int l = 0; l < wpList.Count - 1; l++)
			{
				if (!(wpList[l] != null) || !(wpList[l + 1] != null))
				{
					continue;
				}
				WPSubPath component = wpList[l].gameObject.GetComponent<WPSubPath>();
				WPSubPath component2 = wpList[l + 1].gameObject.GetComponent<WPSubPath>();
				if (component != null && component2 != null)
				{
					for (int m = 0; m < component.posList.Count - 1; m++)
					{
						Gizmos.DrawLine(component.posList[m], component.posList[m + 1]);
					}
					Gizmos.DrawLine(component.posList[component.posList.Count - 1], component2.posList[0]);
				}
				else if (component != null && component2 == null)
				{
					for (int n = 0; n < component.posList.Count - 1; n++)
					{
						Gizmos.DrawLine(component.posList[n], component.posList[n + 1]);
					}
					Gizmos.DrawLine(component.posList[component.posList.Count - 1], wpList[l + 1].position);
				}
				else if (component == null && component2 != null)
				{
					Gizmos.DrawLine(wpList[l].position, component2.posList[0]);
				}
				else
				{
					Gizmos.DrawLine(wpList[l].position, wpList[l + 1].position);
				}
			}
		}
	}
}
