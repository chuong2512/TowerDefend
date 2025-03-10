using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TDTK
{
	public class PathFinder : MonoBehaviour
	{
		public enum _PathSmoothing
		{
			Off,
			On
		}

		private List<SearchQueue> searchQueueList = new List<SearchQueue>();

		private bool searching;

		private static PathFinder instance;

		public _PathSmoothing pathSmoothing = _PathSmoothing.On;

		private int ScanNodeLimitPerFrame = 1000;

		public void Awake()
		{
			if (!(instance != null))
			{
				instance = this;
			}
		}

		public static void Init()
		{
			if (!(instance != null))
			{
				GameObject gameObject = new GameObject();
				instance = gameObject.AddComponent<PathFinder>();
				gameObject.name = "PathFinder";
			}
		}

		private void Update()
		{
			if (searchQueueList.Count > 0 && !searching)
			{
				SearchQueue searchQueue = searchQueueList[0];
				StartCoroutine(_SearchRoutine(searchQueue.startNode, searchQueue.endNode, searchQueue.graph, searchQueue.callBackFunc));
				searchQueueList.RemoveAt(0);
			}
		}

		public static NodeTD GetNearestNode(Vector3 point, NodeTD[] graph)
		{
			return GetNearestNode(point, graph, 1);
		}

		public static NodeTD GetNearestNode(Vector3 point, NodeTD[] graph, int searchMode)
		{
			float num = float.PositiveInfinity;
			float num2 = float.PositiveInfinity;
			NodeTD result = null;
			foreach (NodeTD nodeTD in graph)
			{
				switch (searchMode)
				{
				case 0:
					num = Vector3.Distance(point, nodeTD.pos);
					if (num < num2)
					{
						num2 = num;
						result = nodeTD;
					}
					break;
				case 1:
					if (nodeTD.walkable)
					{
						num = Vector3.Distance(point, nodeTD.pos);
						if (num < num2)
						{
							num2 = num;
							result = nodeTD;
						}
					}
					break;
				case 2:
					if (!nodeTD.walkable)
					{
						num = Vector3.Distance(point, nodeTD.pos);
						if (num < num2)
						{
							num2 = num;
							result = nodeTD;
						}
					}
					break;
				}
			}
			return result;
		}

		public static void GetPath(NodeTD startN, NodeTD endN, NodeTD[] graph, SetPathCallbackTD callBackFunc)
		{
			if (instance == null)
			{
				Init();
			}
			instance._GetPath(startN, endN, graph, callBackFunc);
		}

		public void _GetPath(NodeTD startN, NodeTD endN, NodeTD[] graph, SetPathCallbackTD callBackFunc)
		{
			if (!searching)
			{
				StartCoroutine(_SearchRoutine(startN, endN, graph, callBackFunc));
				return;
			}
			SearchQueue item = new SearchQueue(startN, endN, graph, callBackFunc);
			searchQueueList.Add(item);
		}

		private IEnumerator _SearchRoutine(NodeTD startN, NodeTD endN, NodeTD[] graph, SetPathCallbackTD callBackFunc)
		{
			searching = true;
			bool pathFound = true;
			int searchCounter = 0;
			int loopCounter = 0;
			List<NodeTD> closeList = new List<NodeTD>();
			NodeTD[] openList = new NodeTD[graph.Length];
			List<int> openListRemoved = new List<int>();
			int openListCounter = 0;
			NodeTD currentNode = startN;
			while (currentNode != endN)
			{
				closeList.Add(currentNode);
				currentNode.listState = _ListStateTD.Close;
				currentNode.ProcessNeighbour(endN);
				NodeTD[] neighbourNode = currentNode.neighbourNode;
				foreach (NodeTD nodeTD in neighbourNode)
				{
					if (nodeTD.listState == _ListStateTD.Unassigned && nodeTD.walkable)
					{
						nodeTD.listState = _ListStateTD.Open;
						if (openListRemoved.Count > 0)
						{
							openList[openListRemoved[0]] = nodeTD;
							openListRemoved.RemoveAt(0);
						}
						else
						{
							openList[openListCounter] = nodeTD;
							openListCounter++;
						}
					}
				}
				currentNode = null;
				float currentLowestF = float.PositiveInfinity;
				int id = 0;
				for (int i = 0; i < openListCounter; i++)
				{
					if (openList[i] != null && openList[i].scoreF < currentLowestF)
					{
						currentLowestF = openList[i].scoreF;
						currentNode = openList[i];
						id = i;
					}
				}
				if (currentNode == null)
				{
					pathFound = false;
					break;
				}
				openList[id] = null;
				openListRemoved.Add(id);
				searchCounter++;
				loopCounter++;
				if (loopCounter > ScanNodeLimitPerFrame)
				{
					loopCounter = 0;
					yield return null;
				}
			}
			List<Vector3> p2 = new List<Vector3>();
			if (pathFound)
			{
				while (currentNode != null)
				{
					p2.Add(currentNode.pos);
					currentNode = currentNode.parent;
				}
				p2 = InvertArray(p2);
				p2 = SmoothPath(p2);
			}
			callBackFunc(p2);
			searching = false;
			ResetGraph(graph);
		}

		public static List<Vector3> ForceSearch(NodeTD startN, NodeTD endN, NodeTD blockN, NodeTD[] graph, int footprint = -1)
		{
			if (blockN != null)
			{
				blockN.walkable = false;
			}
			bool flag = true;
			int num = 0;
			List<NodeTD> list = new List<NodeTD>();
			NodeTD[] array = new NodeTD[graph.Length];
			List<int> list2 = new List<int>();
			int num2 = 0;
			NodeTD nodeTD = startN;
			float num3 = float.PositiveInfinity;
			int num4 = 0;
			int num5 = 0;
			while (nodeTD != endN)
			{
				list.Add(nodeTD);
				nodeTD.listState = _ListStateTD.Close;
				nodeTD.ProcessNeighbour(endN);
				NodeTD[] neighbourNode = nodeTD.neighbourNode;
				foreach (NodeTD nodeTD2 in neighbourNode)
				{
					if (nodeTD2.listState == _ListStateTD.Unassigned && nodeTD2.walkable)
					{
						nodeTD2.listState = _ListStateTD.Open;
						if (list2.Count > 0)
						{
							array[list2[0]] = nodeTD2;
							list2.RemoveAt(0);
						}
						else
						{
							array[num2] = nodeTD2;
							num2++;
						}
					}
				}
				nodeTD = null;
				num3 = float.PositiveInfinity;
				num4 = 0;
				for (num5 = 0; num5 < num2; num5++)
				{
					if (array[num5] != null && array[num5].scoreF < num3)
					{
						num3 = array[num5].scoreF;
						nodeTD = array[num5];
						num4 = num5;
					}
				}
				if (nodeTD == null)
				{
					flag = false;
					break;
				}
				array[num4] = null;
				list2.Add(num4);
				num++;
			}
			List<Vector3> list3 = new List<Vector3>();
			if (flag)
			{
				while (nodeTD != null)
				{
					list3.Add(nodeTD.pos);
					nodeTD = nodeTD.parent;
				}
				list3 = InvertArray(list3);
				list3 = SmoothPath(list3);
			}
			if (blockN != null)
			{
				blockN.walkable = true;
			}
			ResetGraph(graph);
			return list3;
		}

		public static bool IsPathSmoothingOn()
		{
			if (instance.pathSmoothing != 0)
			{
				return true;
			}
			return false;
		}

		public static List<Vector3> SmoothPath(List<Vector3> p)
		{
			if (instance.pathSmoothing == _PathSmoothing.On)
			{
				p = MeanSmoothPath(p);
			}
			return p;
		}

		private static List<Vector3> InvertArray(List<Vector3> p)
		{
			List<Vector3> list = new List<Vector3>();
			for (int i = 0; i < p.Count; i++)
			{
				list.Add(p[p.Count - (i + 1)]);
			}
			return list;
		}

		public static List<Vector3> MeanSmoothPath(List<Vector3> p)
		{
			if (p.Count <= 2)
			{
				return p;
			}
			for (int i = 0; i < p.Count; i++)
			{
				if (i != 0 && i != p.Count - 1)
				{
					p[i] = (p[i - 1] + p[i] + p[i + 1]) / 3f;
				}
			}
			return p;
		}

		public static List<Vector3> MeanSmoothPath5(List<Vector3> p)
		{
			for (int i = 1; i < p.Count - 1; i++)
			{
				switch (i)
				{
				case 0:
					if (p.Count >= 3)
					{
						p[i] = (p[i] + p[i + 1] + p[i + 2]) / 3f;
						continue;
					}
					break;
				case 1:
					if (p.Count == 3)
					{
						p[i] = (p[i - 1] + p[i] + p[i + 1]) / 3f;
					}
					if (p.Count >= 4)
					{
						p[i] = (p[i - 1] + p[i] + p[i + 1] + p[i + 2]) / 4f;
						continue;
					}
					break;
				default:
					if (i == p.Count - 2)
					{
						if (p.Count == 3)
						{
							p[i] = (p[i - 1] + p[i] + p[i + 1]) / 3f;
						}
						if (p.Count >= 4)
						{
							p[i] = (p[i - 2] + p[i - 1] + p[i] + p[i + 1]) / 4f;
							continue;
						}
						break;
					}
					if (i == p.Count - 1)
					{
						if (p.Count < 3)
						{
							break;
						}
						p[i] = (p[i - 2] + p[i - 1] + p[i]) / 3f;
						continue;
					}
					p[i] = (p[i - 2] + p[i - 1] + p[i] + p[i + 1] + p[i + 2]) / 5f;
					continue;
				}
				break;
			}
			return p;
		}

		private List<Vector3> LOSPathSmoothingForward(List<Vector3> p)
		{
			float gridSize = BuildManager.GetGridSize();
			int num = 0;
			float radius = gridSize * 0.4f;
			while (num + 2 < p.Count)
			{
				bool flag = false;
				Vector3 vector = p[num];
				Vector3 a = p[num + 2];
				Vector3 direction = a - vector;
				LayerMask mask = 1 << TDTK.GetLayerTerrain();
				if (!Physics.SphereCast(vector, radius, direction, out RaycastHit _, Vector3.Distance(a, vector), ~(int)mask))
				{
					if (vector.y == a.y)
					{
						p.RemoveAt(num + 1);
					}
					else
					{
						flag = true;
					}
				}
				else
				{
					flag = true;
				}
				if (flag)
				{
					num++;
				}
			}
			return p;
		}

		private List<Vector3> LOSPathSmoothingBackward(List<Vector3> p)
		{
			float gridSize = BuildManager.GetGridSize();
			int num = p.Count - 1;
			float radius = gridSize * 0.4f;
			while (num > 1)
			{
				bool flag = false;
				Vector3 vector = p[num];
				Vector3 a = p[num - 2];
				Vector3 direction = a - vector;
				if (!Physics.SphereCast(vector, radius, direction, out RaycastHit _, Vector3.Distance(a, vector)))
				{
					if (vector.y == a.y)
					{
						p.RemoveAt(num - 1);
					}
					else
					{
						flag = true;
					}
				}
				else
				{
					flag = true;
				}
				num--;
				if (flag)
				{
					num--;
				}
			}
			return p;
		}

		public static void ResetGraph(NodeTD[] nodeGraph)
		{
			foreach (NodeTD nodeTD in nodeGraph)
			{
				nodeTD.listState = _ListStateTD.Unassigned;
				nodeTD.parent = null;
			}
		}
	}
}
