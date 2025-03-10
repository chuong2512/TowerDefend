using System.Collections.Generic;
using UnityEngine;

namespace TDTK
{
	public class NodeTD
	{
		public int ID;

		public Vector3 pos;

		public NodeTD[] neighbourNode;

		public float[] neighbourCost;

		public NodeTD parent;

		public bool walkable = true;

		public float scoreG;

		public float scoreH;

		public float scoreF;

		public _ListStateTD listState;

		public float tempScoreG;

		public NodeTD()
		{
		}

		public NodeTD(Vector3 position, int id)
		{
			pos = position;
			ID = id;
		}

		public void SetNeighbour(List<NodeTD> arrNeighbour, List<float> arrCost)
		{
			neighbourNode = arrNeighbour.ToArray();
			neighbourCost = arrCost.ToArray();
		}

		public void ProcessNeighbour(NodeTD node)
		{
			ProcessNeighbour(node.pos);
		}

		public void ProcessNeighbour(Vector3 pos)
		{
			for (int i = 0; i < neighbourNode.Length; i++)
			{
				if (neighbourNode[i].listState == _ListStateTD.Unassigned)
				{
					neighbourNode[i].scoreG = scoreG + neighbourCost[i];
					neighbourNode[i].scoreH = Vector3.Distance(neighbourNode[i].pos, pos);
					neighbourNode[i].UpdateScoreF();
					neighbourNode[i].parent = this;
				}
				else if (neighbourNode[i].listState == _ListStateTD.Open)
				{
					tempScoreG = scoreG + neighbourCost[i];
					if (neighbourNode[i].scoreG > tempScoreG)
					{
						neighbourNode[i].parent = this;
						neighbourNode[i].scoreG = tempScoreG;
						neighbourNode[i].UpdateScoreF();
					}
				}
			}
		}

		private void UpdateScoreF()
		{
			scoreF = scoreG + scoreH;
		}
	}
}
