using System;
using System.Collections.Generic;
using UnityEngine;

namespace MadLevelManager
{
	[ExecuteInEditMode]
	public class MadBigMeshRenderer : MonoBehaviour
	{
		private MadPanel panel;

		private MadList<Vector3> vertices = new MadList<Vector3>();

		private MadList<Color32> colors = new MadList<Color32>();

		private MadList<Vector2> uv = new MadList<Vector2>();

		private MadList<MadList<int>> triangleList = new MadList<MadList<int>>();

		private MadObjectPool<MadList<int>> trianglesPool = new MadObjectPool<MadList<int>>(32);

		[SerializeField]
		private List<MadDrawCall> drawCalls = new List<MadDrawCall>(32);

		private int nextDrawCall;

		private Vector3[] cornersWorker = new Vector3[4];

		private void OnEnable()
		{
			panel = GetComponent<MadPanel>();
			for (int i = 0; i < drawCalls.Count; i++)
			{
				MadDrawCall madDrawCall = drawCalls[i];
				if (madDrawCall == null)
				{
					drawCalls.RemoveAt(i);
					i--;
				}
				else
				{
					MadGameObject.SetActive(madDrawCall.gameObject, active: true);
				}
			}
		}

		private MadDrawCall NextDrawCall()
		{
			MadDrawCall madDrawCall;
			if (nextDrawCall >= drawCalls.Count)
			{
				madDrawCall = MadDrawCall.Create();
				madDrawCall.gameObject.layer = base.gameObject.layer;
				MadTransform.SetLocalScale(madDrawCall.transform, base.transform.lossyScale);
				drawCalls.Add(madDrawCall);
				nextDrawCall++;
			}
			else
			{
				madDrawCall = drawCalls[nextDrawCall++];
				MadGameObject.SetActive(madDrawCall.gameObject, active: true);
			}
			return madDrawCall;
		}

		private void DrawCallsFinalize()
		{
			int count = drawCalls.Count;
			for (int i = nextDrawCall; i < count; i++)
			{
				int index = drawCalls.Count - 1;
				MadDrawCall madDrawCall = drawCalls[index];
				madDrawCall.Destroy();
				drawCalls.RemoveAt(index);
			}
			nextDrawCall = 0;
		}

		private void OnDisable()
		{
			for (int i = 0; i < drawCalls.Count; i++)
			{
				MadDrawCall madDrawCall = drawCalls[i];
				MadGameObject.SetActive(madDrawCall.gameObject, active: false);
			}
		}

		private void Start()
		{
		}

		private void Update()
		{
			for (int i = 0; i < drawCalls.Count; i++)
			{
				MadDrawCall madDrawCall = drawCalls[i];
				MadTransform.SetLocalScale(madDrawCall.transform, base.transform.lossyScale);
				madDrawCall.transform.position = base.transform.position;
				madDrawCall.transform.rotation = base.transform.rotation;
				madDrawCall.gameObject.layer = base.gameObject.layer;
			}
		}

		private void LateUpdate()
		{
			if (panel == null)
			{
				panel = GetComponent<MadPanel>();
			}
			List<MadSprite> sprites = VisibleSprites(panel.sprites);
			switch (panel.renderMode)
			{
			case MadPanel.RenderMode.Legacy:
			{
				SortByGUIDepth(sprites);
				List<List<MadSprite>> batchedSprites2 = Batch(sprites);
				DrawOnSingleDrawCall(batchedSprites2);
				break;
			}
			case MadPanel.RenderMode.DepthBased:
			{
				SortByZ(sprites);
				List<List<MadSprite>> batchedSprites = Batch(sprites);
				DrawOnMultipleDrawCalls(batchedSprites);
				break;
			}
			default:
				throw new ArgumentOutOfRangeException();
			}
			vertices.Clear();
			colors.Clear();
			uv.Clear();
			triangleList.Clear();
		}

		private void DrawOnMultipleDrawCalls(List<List<MadSprite>> batchedSprites)
		{
			try
			{
				for (int i = 0; i < batchedSprites.Count; i++)
				{
					List<MadSprite> list = batchedSprites[i];
					MadDrawCall madDrawCall = NextDrawCall();
					Mesh mesh = madDrawCall.mesh;
					mesh.Clear();
					mesh.subMeshCount = 1;
					if (!trianglesPool.CanTake())
					{
						trianglesPool.Add(new MadList<int>());
					}
					MadList<int> triangles = trianglesPool.Take();
					for (int j = 0; j < list.Count; j++)
					{
						MadSprite madSprite = list[j];
						madSprite.DrawOn(ref vertices, ref colors, ref uv, ref triangles, out Material material);
						if (j == 0)
						{
							madDrawCall.SetMaterial(material);
						}
					}
					triangles.Trim();
					vertices.Trim();
					colors.Trim();
					uv.Trim();
					mesh.vertices = vertices.Array;
					mesh.colors32 = colors.Array;
					mesh.uv = uv.Array;
					mesh.SetTriangles(triangles.Array, 0);
					mesh.RecalculateNormals();
					triangles.Clear();
					trianglesPool.Release(triangles);
					vertices.Clear();
					colors.Clear();
					uv.Clear();
					triangleList.Clear();
				}
			}
			finally
			{
				DrawCallsFinalize();
			}
		}

		private void DrawOnSingleDrawCall(List<List<MadSprite>> batchedSprites)
		{
			Material[] array = new Material[batchedSprites.Count];
			MadDrawCall madDrawCall = NextDrawCall();
			try
			{
				Mesh mesh = madDrawCall.mesh;
				mesh.Clear();
				mesh.subMeshCount = batchedSprites.Count;
				for (int i = 0; i < batchedSprites.Count; i++)
				{
					List<MadSprite> list = batchedSprites[i];
					if (!trianglesPool.CanTake())
					{
						trianglesPool.Add(new MadList<int>());
					}
					MadList<int> triangles = trianglesPool.Take();
					for (int j = 0; j < list.Count; j++)
					{
						MadSprite madSprite = list[j];
						madSprite.DrawOn(ref vertices, ref colors, ref uv, ref triangles, out Material material);
						array[i] = material;
					}
					triangles.Trim();
					triangleList.Add(triangles);
				}
				vertices.Trim();
				colors.Trim();
				uv.Trim();
				triangleList.Trim();
				mesh.vertices = vertices.Array;
				mesh.colors32 = colors.Array;
				mesh.uv = uv.Array;
				for (int k = 0; k < triangleList.Count; k++)
				{
					MadList<int> madList = triangleList[k];
					mesh.SetTriangles(madList.Array, k);
					madList.Clear();
					trianglesPool.Release(madList);
				}
				mesh.RecalculateNormals();
				madDrawCall.SetMaterials(array);
			}
			finally
			{
				DrawCallsFinalize();
			}
		}

		private void OnDestroy()
		{
			for (int i = 0; i < drawCalls.Count; i++)
			{
				MadDrawCall madDrawCall = drawCalls[i];
				madDrawCall.Destroy();
			}
			drawCalls.Clear();
		}

		private List<MadSprite> VisibleSprites(ICollection<MadSprite> sprites)
		{
			List<MadSprite> list = new List<MadSprite>();
			foreach (MadSprite sprite in sprites)
			{
				if (SpriteVisible(sprite))
				{
					list.Add(sprite);
				}
			}
			return list;
		}

		private bool SpriteVisible(MadSprite sprite)
		{
			if (!sprite.gameObject.activeInHierarchy)
			{
				return false;
			}
			if (!sprite.visible)
			{
				return false;
			}
			if (Mathf.Approximately(sprite.tint.a, 0f))
			{
				return false;
			}
			if (!sprite.CanDraw())
			{
				return false;
			}
			if (panel.hideInvisibleSprites && Application.isPlaying)
			{
				sprite.GetWorldCorners(ref cornersWorker);
				Camera currentCamera = panel.currentCamera;
				if (!VisibleOnCameraAny(cornersWorker, currentCamera))
				{
					return false;
				}
			}
			return true;
		}

		private bool VisibleOnCameraAny(Vector3[] corners, Camera cam)
		{
			Vector3 vector = cam.WorldToViewportPoint(corners[0]);
			Vector3 vector2 = cam.WorldToViewportPoint(corners[2]);
			float num = Mathf.Min(vector.x, vector2.x);
			float num2 = Mathf.Max(vector.y, vector2.y);
			float num3 = Mathf.Max(vector.x, vector2.x);
			float num4 = Mathf.Min(vector.y, vector2.y);
			if (num3 < 0f || num > 1f || num2 < 0f || num4 > 1f)
			{
				return false;
			}
			return true;
		}

		private bool VisibleOnCamera(Vector3 corner, Camera cam)
		{
			Vector3 vector = cam.WorldToViewportPoint(corner);
			return vector.x >= 0f && vector.x <= 1f && vector.y >= 0f && vector.y <= 1f;
		}

		private void SortByGUIDepth(List<MadSprite> sprites)
		{
			sprites.Sort((MadSprite x, MadSprite y) => x.guiDepth.CompareTo(y.guiDepth));
		}

		private void SortByZ(List<MadSprite> sprites)
		{
			sprites.Sort(delegate(MadSprite x, MadSprite y)
			{
				Vector3 position = x.transform.position;
				ref float z = ref position.z;
				Vector3 position2 = y.transform.position;
				return z.CompareTo(position2.z);
			});
		}

		private List<List<MadSprite>> Batch(List<MadSprite> sprites)
		{
			List<List<MadSprite>> list = new List<List<MadSprite>>();
			int count = sprites.Count;
			List<MadSprite> list2 = null;
			for (int i = 0; i < count; i++)
			{
				MadSprite madSprite = sprites[i];
				if (list2 == null)
				{
					list2 = new List<MadSprite>();
				}
				else if (!CanBatch(madSprite, list2[list2.Count - 1]))
				{
					list.Add(list2);
					list2 = new List<MadSprite>();
				}
				list2.Add(madSprite);
			}
			if (list2 != null)
			{
				list.Add(list2);
			}
			return list;
		}

		private bool CanBatch(MadSprite a, MadSprite b)
		{
			if (panel.renderMode == MadPanel.RenderMode.DepthBased && a.guiDepth != b.guiDepth)
			{
				return false;
			}
			Material material = a.GetMaterial();
			Material material2 = b.GetMaterial();
			return material.Equals(material2);
		}
	}
}
