using System.Collections.Generic;
using UnityEngine;

namespace MadLevelManager
{
	public class MadMaterialStore : MonoBehaviour
	{
		private class MaterialKey
		{
			private Texture texture;

			private string shaderName;

			private int variation;

			public MaterialKey(Texture texture, string shaderName, int variation)
			{
				this.texture = texture;
				this.shaderName = shaderName;
				this.variation = variation;
			}

			public bool Equals(object obj)
			{
				if (obj == null || !(obj is MaterialKey))
				{
					return false;
				}
				MaterialKey materialKey = obj as MaterialKey;
				return texture == materialKey.texture && shaderName == materialKey.shaderName && variation == materialKey.variation;
			}

			public int GetHashCode()
			{
				int num = 17;
				num = num * 23 + texture.GetHashCode();
				num = num * 23 + shaderName.GetHashCode();
				return num * 23 + variation.GetHashCode();
			}
		}

		private Dictionary<MaterialKey, Material> materials = new Dictionary<MaterialKey, Material>();

		private int nextVariation = 1;

		private void OnDestroy()
		{
			foreach (Material value in materials.Values)
			{
				UnityEngine.Object.DestroyImmediate(value);
			}
		}

		public Material CreateUnique(Texture texture, string shaderName, out int variation)
		{
			variation = nextVariation++;
			return CreateMaterial(texture, shaderName, variation);
		}

		public Material MaterialFor(Texture texture, string shaderName)
		{
			return MaterialFor(texture, shaderName, 0);
		}

		public Material MaterialFor(Texture texture, string shaderName, int variation)
		{
			if (texture == null)
			{
				UnityEngine.Debug.LogError("null texture", this);
				return null;
			}
			if (shaderName == null)
			{
				UnityEngine.Debug.LogError("null shader name", this);
				return null;
			}
			MaterialKey key = new MaterialKey(texture, shaderName, variation);
			if (materials.ContainsKey(key))
			{
				return materials[key];
			}
			return CreateMaterial(texture, shaderName, 0);
		}

		private Material CreateMaterial(Texture texture, string shaderName, int variation)
		{
			MaterialKey key = new MaterialKey(texture, shaderName, variation);
			Shader shader = Shader.Find(shaderName);
			if (shader == null)
			{
				UnityEngine.Debug.LogError("Shader not found: " + shaderName);
				return null;
			}
			Material material = new Material(shader);
			material.mainTexture = texture;
			material.hideFlags = HideFlags.DontSave;
			materials.Add(key, material);
			return material;
		}
	}
}
