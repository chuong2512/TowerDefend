using UnityEngine;

namespace TDTK
{
	public class AutoAdjustGrid : MonoBehaviour
	{
		public float gridSize = 2f;

		private void Start()
		{
			Renderer component = base.transform.GetComponent<Renderer>();
			if (!(component == null))
			{
				Material material = component.material;
				Vector3 worldScale = Utility.GetWorldScale(base.transform);
				material.mainTextureScale = new Vector2(worldScale.x / gridSize, worldScale.z / gridSize);
			}
		}
	}
}
