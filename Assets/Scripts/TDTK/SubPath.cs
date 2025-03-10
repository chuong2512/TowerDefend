using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace TDTK
{
	public class SubPath
	{
		public delegate void PathChangedHandler(SubPath subPath);

		public PathTD parentPath;

		public PlatformTD parentPlatform;

		public int wpIDPlatform;

		public Transform connectStart;

		public Transform connectEnd;

		public NodeTD startN;

		public NodeTD endN;

		public List<Vector3> path = new List<Vector3>();

		public List<Vector3> altPath = new List<Vector3>();

		public static event PathChangedHandler onPathChangedE;

		public void Init(PlatformTD platform)
		{
			parentPlatform = platform;
			startN = PathFinder.GetNearestNode(connectStart.position, platform.GetNodeGraph());
			endN = PathFinder.GetNearestNode(connectEnd.position, platform.GetNodeGraph());
			path.Add((connectStart.position + connectEnd.position) / 2f);
			SearchNewPath(platform.GetNodeGraph());
		}

		public bool IsNodeInPath(NodeTD node)
		{
			float gridSize = BuildManager.GetGridSize();
			for (int i = 0; i < path.Count; i++)
			{
				float num = Vector3.Distance(node.pos, path[i]);
				if (num < gridSize * 0.85f)
				{
					return true;
				}
			}
			return false;
		}

		public void SearchNewPath(NodeTD[] nodeGraph)
		{
			PathFinder.GetPath(startN, endN, nodeGraph, SetPath);
		}

		public void SetPath(List<Vector3> wpList)
		{
			path = wpList;
			if (SubPath.onPathChangedE != null)
			{
				SubPath.onPathChangedE(this);
			}
		}

		public void SwitchToSubPath()
		{
			SetPath(PathFinder.SmoothPath(altPath));
			altPath = new List<Vector3>();
		}
	}
}
