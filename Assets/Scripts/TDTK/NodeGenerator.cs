using System.Collections.Generic;
using UnityEngine;

namespace TDTK
{
	public class NodeGenerator : MonoBehaviour
	{
		private bool connectDiagonalNeighbour;

		private static NodeGenerator instance;

		private static Transform thisT;

		public void Awake()
		{
			if (!(instance != null))
			{
				instance = this;
				thisT = base.transform;
			}
		}

		public static void Init()
		{
			if (!(instance != null))
			{
				GameObject gameObject = new GameObject();
				instance = gameObject.AddComponent<NodeGenerator>();
				gameObject.name = "NodeGenerator";
			}
		}

		public static NodeTD[] GenerateNode(PlatformTD platform, float heightOffset)
		{
			if (instance == null)
			{
				Init();
			}
			Transform transform = platform.thisT;
			float gridSize = BuildManager.GetGridSize();
			Vector3 localScale = platform.thisT.localScale;
			float x = localScale.x;
			Vector3 localScale2 = platform.thisT.localScale;
			float z = localScale2.z;
			int num = (int)(x / gridSize);
			int num2 = (int)(z / gridSize);
			float x2 = (0f - x) / 2f / x;
			float z2 = (0f - z) / 2f / z;
			Vector3 position = transform.TransformPoint(new Vector3(x2, 0f, z2));
			thisT.position = position;
			thisT.rotation = transform.rotation;
			thisT.position = thisT.TransformPoint(new Vector3(gridSize / 2f, heightOffset, gridSize / 2f));
			NodeTD[] array = new NodeTD[num2 * num];
			int num3 = 0;
			for (int i = 0; i < num2; i++)
			{
				for (int j = 0; j < num; j++)
				{
					Vector3 position2 = thisT.position;
					position2.y += 5000f;
					LayerMask mask = 1 << TDTK.GetLayerTower();
					if (Physics.Raycast(position2, Vector3.down, out RaycastHit hitInfo, float.PositiveInfinity, ~(int)mask))
					{
						NodeTD[] array2 = array;
						int num4 = num3;
						float x3 = position2.x;
						Vector3 point = hitInfo.point;
						array2[num4] = new NodeTD(new Vector3(x3, point.y + heightOffset, position2.z), num3);
					}
					else
					{
						array[num3] = new NodeTD(position2, num3);
						array[num3].walkable = false;
					}
					num3++;
					thisT.position = thisT.TransformPoint(new Vector3(gridSize, 0f, 0f));
				}
				thisT.position = thisT.TransformPoint(new Vector3((float)(-num) * gridSize, 0f, gridSize));
			}
			thisT.position = Vector3.zero;
			thisT.rotation = Quaternion.identity;
			num3 = 0;
			NodeTD[] array3 = array;
			foreach (NodeTD nodeTD in array3)
			{
				if (nodeTD.walkable)
				{
					LayerMask mask2 = 1 << TDTK.GetLayerPlatform();
					mask2 = ((int)mask2 | (1 << TDTK.GetLayerTower()));
					if (TDTK.GetLayerTerrain() >= 0)
					{
						mask2 = ((int)mask2 | (1 << TDTK.GetLayerTerrain()));
					}
					Collider[] array4 = Physics.OverlapSphere(nodeTD.pos, gridSize * 0.45f, ~(int)mask2);
					if (array4.Length > 0)
					{
						nodeTD.walkable = false;
						num3++;
					}
				}
			}
			float num5 = 0f;
			float num6 = (!instance.connectDiagonalNeighbour) ? (gridSize * 1.1f) : (gridSize * 1.5f);
			num3 = 0;
			NodeTD[] array5 = array;
			foreach (NodeTD nodeTD2 in array5)
			{
				if (nodeTD2.walkable)
				{
					List<NodeTD> list = new List<NodeTD>();
					List<float> list2 = new List<float>();
					NodeTD[] array6 = new NodeTD[8];
					int iD = nodeTD2.ID;
					if (iD > num - 1 && iD < num * num2 - num)
					{
						if (iD != num)
						{
							array6[0] = array[iD - num - 1];
						}
						array6[1] = array[iD - num];
						array6[2] = array[iD - num + 1];
						array6[3] = array[iD - 1];
						array6[4] = array[iD + 1];
						array6[5] = array[iD + num - 1];
						array6[6] = array[iD + num];
						if (iD != num * num2 - num - 1)
						{
							array6[7] = array[iD + num + 1];
						}
					}
					else if (iD <= num - 1)
					{
						if (iD != 0)
						{
							array6[0] = array[iD - 1];
						}
						if (array.Length > iD + 1)
						{
							array6[1] = array[iD + 1];
						}
						if (num2 > 0)
						{
							if (array.Length > iD + num - 1)
							{
								array6[2] = array[iD + num - 1];
							}
							if (array.Length > iD + num)
							{
								array6[3] = array[iD + num];
							}
							if (array.Length > iD + num + 1)
							{
								array6[4] = array[iD + num + 1];
							}
						}
					}
					else if (iD >= num * num2 - num)
					{
						array6[0] = array[iD - 1];
						if (iD != num * num2 - 1)
						{
							array6[1] = array[iD + 1];
						}
						if (iD != num * (num2 - 1))
						{
							array6[2] = array[iD - num - 1];
						}
						array6[3] = array[iD - num];
						array6[4] = array[iD - num + 1];
					}
					NodeTD[] array7 = array6;
					foreach (NodeTD nodeTD3 in array7)
					{
						if (nodeTD3 == null || !nodeTD3.walkable)
						{
							continue;
						}
						num5 = GetHorizontalDistance(nodeTD2.pos, nodeTD3.pos);
						if (num5 < num6)
						{
							LayerMask mask3 = 1 << TDTK.GetLayerPlatform();
							mask3 = ((int)mask3 | (1 << TDTK.GetLayerTower()));
							if (!Physics.Linecast(nodeTD2.pos, nodeTD3.pos, ~(int)mask3))
							{
								list.Add(nodeTD3);
								list2.Add(num5);
							}
						}
					}
					nodeTD2.SetNeighbour(list, list2);
				}
				num3++;
			}
			return array;
		}

		public static float GetHorizontalDistance(Vector3 p1, Vector3 p2)
		{
			p1.y = 0f;
			p2.y = 0f;
			return Vector3.Distance(p1, p2);
		}

		public static bool ConnectDiagonalNeighbour()
		{
			return instance.connectDiagonalNeighbour;
		}
	}
}
