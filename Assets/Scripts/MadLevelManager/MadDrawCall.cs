using UnityEngine;

namespace MadLevelManager
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
	public class MadDrawCall : MonoBehaviour
	{
		public Mesh mesh;

		private void OnEnable()
		{
			MeshFilter component = base.transform.GetComponent<MeshFilter>();
			if (mesh == null)
			{
				mesh = new Mesh();
				mesh.hideFlags = HideFlags.DontSave;
				component.mesh = mesh;
			}
			mesh.MarkDynamic();
		}

		private void Update()
		{
		}

		private void OnDestroy()
		{
			if (Application.isEditor)
			{
				UnityEngine.Object.DestroyImmediate(mesh);
			}
			else
			{
				UnityEngine.Object.Destroy(mesh);
			}
		}

		public void SetMaterial(Material material)
		{
			Renderer component = GetComponent<Renderer>();
			if (GetComponent<Renderer>().sharedMaterials.Length != 1)
			{
				component.sharedMaterials = new Material[1]
				{
					material
				};
			}
			else
			{
				component.sharedMaterial = material;
			}
		}

		public void SetMaterials(Material[] materials)
		{
			Material[] sharedMaterials = GetComponent<Renderer>().sharedMaterials;
			if (sharedMaterials.Length != materials.Length)
			{
				GetComponent<Renderer>().sharedMaterials = materials;
				return;
			}
			int num = 0;
			while (true)
			{
				if (num < sharedMaterials.Length)
				{
					Material x = sharedMaterials[num];
					Material y = materials[num];
					if (x != y)
					{
						break;
					}
					num++;
					continue;
				}
				return;
			}
			GetComponent<Renderer>().sharedMaterials = materials;
		}

		public void Destroy()
		{
			MadGameObject.SetActive(base.gameObject, active: false);
			MadGameObject.SafeDestroy(base.gameObject);
		}

		public static MadDrawCall Create()
		{
			GameObject gameObject = new GameObject("_draw_call");
			gameObject.hideFlags = HideFlags.HideAndDontSave;
			gameObject.AddComponent<MadDrawCall>();
			return gameObject.GetComponent<MadDrawCall>();
		}
	}
}
