using UnityEngine;

namespace TBTK2
{
	public class TextureScroll : MonoBehaviour
	{
		public Material mat;

		public Vector2 uvAnimationRate = new Vector2(1f, 0f);

		private Vector2 uvOffset = Vector2.zero;

		private void Awake()
		{
			mat = base.transform.GetComponent<Renderer>().material;
		}

		private void OnEnable()
		{
		}

		private void Update()
		{
			uvOffset += uvAnimationRate * Time.deltaTime;
			mat.SetTextureOffset("_MainTex", uvOffset);
		}
	}
}
