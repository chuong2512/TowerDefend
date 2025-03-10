using System.Collections.Generic;
using UnityEngine;

namespace TDTK
{
	public class PlatformTD : MonoBehaviour
	{
		public Vector2 size;

		public List<int> unavailableTowerIDList = new List<int>();

		[HideInInspector]
		public List<int> availableTowerIDList = new List<int>();

		public List<SubPath> subPathList = new List<SubPath>();

		private bool walkable;

		[HideInInspector]
		public GameObject thisObj;

		[HideInInspector]
		public Transform thisT;

		private NodeTD nextBuildNode;

		private NodeTD[] nodeGraph;

		public bool GizmoShowNodes = true;

		public void Init(float gridSize, bool autoAdjustTextureToGrid, List<UnitTower> towerList)
		{
			thisObj = base.gameObject;
			thisT = base.transform;
			thisObj.layer = TDTK.GetLayerPlatform();
			Format(gridSize, autoAdjustTextureToGrid);
			VerifyTowers(towerList);
			if (walkable)
			{
				GenerateNode();
				for (int i = 0; i < subPathList.Count; i++)
				{
					subPathList[i].Init(this);
				}
			}
		}

		public void Format(float gridSize, bool autoAdjustTextureToGrid)
		{
			Transform transform = thisT;
			Vector3 eulerAngles = thisT.rotation.eulerAngles;
			transform.eulerAngles = new Vector3(0f, eulerAngles.y, 0f);
			Vector3 worldScale = Utility.GetWorldScale(thisT);
			float num = Mathf.Max(1f, Mathf.Round(worldScale.x / gridSize)) * gridSize;
			Vector3 worldScale2 = Utility.GetWorldScale(thisT);
			float num2 = Mathf.Max(1f, Mathf.Round(worldScale2.z / gridSize)) * gridSize;
			thisT.localScale = new Vector3(num, 1f, num2);
			size = new Vector2((int)(num / gridSize), (int)(num2 / gridSize));
			if (autoAdjustTextureToGrid)
			{
				Material material = thisT.GetComponent<Renderer>().material;
				Vector3 worldScale3 = Utility.GetWorldScale(thisT);
				float x = worldScale3.x / gridSize;
				Vector3 worldScale4 = Utility.GetWorldScale(thisT);
				float y = worldScale4.z / gridSize;
				material.mainTextureOffset = new Vector2(0.5f, 0.5f);
				material.mainTextureScale = new Vector2(x, y);
			}
		}

		public void VerifyTowers(List<UnitTower> towerList)
		{
			List<int> list = new List<int>();
			for (int i = 0; i < towerList.Count; i++)
			{
				if (!unavailableTowerIDList.Contains(towerList[i].prefabID))
				{
					list.Add(towerList[i].prefabID);
				}
			}
			availableTowerIDList = list;
		}

		public int AddSubPath(PathTD pathInstance, int wpID, Transform startP, Transform endP)
		{
			walkable = true;
			int count = subPathList.Count;
			SubPath subPath = new SubPath();
			subPath.parentPath = pathInstance;
			subPath.wpIDPlatform = wpID;
			subPath.connectStart = startP;
			subPath.connectEnd = endP;
			subPathList.Add(subPath);
			return count;
		}

		public List<Vector3> GetSubPathPath(int ID)
		{
			return subPathList[ID].path;
		}

		public SubPath GetSubPath(int ID)
		{
			return subPathList[ID];
		}

		public void BuildTower(Vector3 pos, UnitTower tower)
		{
			if (!walkable || tower.type == _TowerType.Mine)
			{
				return;
			}
			NodeTD nearestNode = PathFinder.GetNearestNode(pos, nodeGraph);
			nearestNode.walkable = false;
			tower.SetPlatform(this, nearestNode);
			if (nearestNode == nextBuildNode)
			{
				for (int i = 0; i < subPathList.Count; i++)
				{
					if (subPathList[i].IsNodeInPath(nearestNode))
					{
						subPathList[i].SwitchToSubPath();
					}
				}
				return;
			}
			for (int j = 0; j < subPathList.Count; j++)
			{
				if (subPathList[j].IsNodeInPath(nearestNode))
				{
					subPathList[j].SearchNewPath(nodeGraph);
				}
			}
		}

		public void UnbuildTower(NodeTD node)
		{
			node.walkable = true;
			for (int i = 0; i < subPathList.Count; i++)
			{
				subPathList[i].SearchNewPath(nodeGraph);
			}
		}

		public bool CheckForBlock(Vector3 pos)
		{
			float gridSize = BuildManager.GetGridSize();
			NodeTD nearestNode = PathFinder.GetNearestNode(pos, nodeGraph);
			for (int i = 0; i < subPathList.Count; i++)
			{
				SubPath subPath = subPathList[i];
				if (Vector3.Distance(pos, subPath.startN.pos) < gridSize / 2f)
				{
					return true;
				}
				if (Vector3.Distance(pos, subPath.endN.pos) < gridSize / 2f)
				{
					return true;
				}
				if (subPath.IsNodeInPath(nearestNode))
				{
					subPath.altPath = PathFinder.ForceSearch(subPath.startN, subPath.endN, nearestNode, nodeGraph);
					if (subPath.altPath.Count == 0)
					{
						return true;
					}
				}
			}
			nextBuildNode = nearestNode;
			return false;
		}

		public void SetWalkable(bool flag)
		{
			walkable = flag;
		}

		public bool IsWalkable()
		{
			return walkable;
		}

		public NodeTD[] GetNodeGraph()
		{
			return nodeGraph;
		}

		public void GenerateNode(float heightOffset = 0f)
		{
			nodeGraph = NodeGenerator.GenerateNode(this, 0f);
		}

		public NodeTD GetNearestNode(Vector3 point)
		{
			return PathFinder.GetNearestNode(point, nodeGraph);
		}

		private void OnDrawGizmos()
		{
			if (!GizmoShowNodes || nodeGraph == null || nodeGraph.Length <= 0)
			{
				return;
			}
			NodeTD[] array = nodeGraph;
			foreach (NodeTD nodeTD in array)
			{
				if (!nodeTD.walkable)
				{
					Gizmos.color = Color.red;
					Gizmos.DrawSphere(nodeTD.pos, BuildManager.GetGridSize() * 0.15f);
				}
				else
				{
					Gizmos.color = Color.white;
					Gizmos.DrawSphere(nodeTD.pos, BuildManager.GetGridSize() * 0.15f);
				}
			}
		}
	}
}
