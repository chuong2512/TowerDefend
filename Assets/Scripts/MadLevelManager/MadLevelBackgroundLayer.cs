using UnityEngine;

namespace MadLevelManager
{
	[RequireComponent(typeof(MadSprite))]
	public class MadLevelBackgroundLayer : MonoBehaviour
	{
		public enum ScaleMode
		{
			Manual,
			Fill
		}

		public enum Align
		{
			None,
			Top,
			Middle,
			Bottom
		}

		public const float ScrollSpeedMultiplier = 0.01f;

		public Texture2D texture;

		public Color tint = Color.white;

		public Vector2 scale = Vector2.one;

		public ScaleMode scaleMode;

		public Align align = Align.Middle;

		public bool repeatX = true;

		public bool repeatY;

		public float fillMarginLeft = -2f;

		public float fillMarginTop = -2f;

		public float fillMarginRight = -2f;

		public float fillMarginBottom = -2f;

		public bool dontStretch = true;

		public Vector2 position = Vector2.zero;

		public float followSpeed = 1f;

		public Vector2 scrollSpeed;

		public bool ignoreTimeScale;

		private Vector2 scrollAccel;

		private MadRootNode _root;

		private MadLevelBackground _parent;

		private MadSprite _sprite;

		private float lastTime;

		private MadRootNode root
		{
			get
			{
				if (_root == null)
				{
					_root = MadTransform.FindParent<MadRootNode>(base.transform);
				}
				return _root;
			}
		}

		public MadLevelBackground parent
		{
			get
			{
				if (_parent == null)
				{
					_parent = MadTransform.FindParent<MadLevelBackground>(base.transform);
				}
				return _parent;
			}
		}

		private MadSprite sprite
		{
			get
			{
				if (_sprite == null)
				{
					_sprite = GetComponent<MadSprite>();
				}
				return _sprite;
			}
		}

		private float deltaTime
		{
			get
			{
				if (!ignoreTimeScale)
				{
					return Time.deltaTime;
				}
				if (Mathf.Approximately(lastTime, 0f))
				{
					return 0f;
				}
				return Time.realtimeSinceStartup - lastTime;
			}
		}

		private void Start()
		{
			SetDirty();
		}

		public void SetDirty()
		{
			sprite.texture = texture;
			sprite.tint = tint;
			int num = parent.IndexOf(this);
			base.name = string.Format("{0:D2} layer ({1})", num, (!(texture != null)) ? "empty" : texture.name);
		}

		public void Cleanup()
		{
			if (sprite != null)
			{
				if (Application.isPlaying)
				{
					UnityEngine.Object.Destroy(base.gameObject);
				}
				else
				{
					MadUndo.DestroyObjectImmediate(base.gameObject);
				}
			}
		}

		public void Update()
		{
			if (sprite == null || sprite.texture == null)
			{
				return;
			}
			float num = 0f;
			float num2 = 0f;
			float num3 = root.screenWidth;
			float num4 = root.screenHeight;
			if (scaleMode == ScaleMode.Fill && !repeatX && !repeatY)
			{
				num = fillMarginLeft;
				num2 = fillMarginBottom;
				num3 -= fillMarginRight;
				num4 -= fillMarginTop;
			}
			float num5 = num3 - num;
			float num6 = num4 - num2;
			float num7 = scale.x;
			float num8 = scale.y;
			float x = num5;
			float y = num6;
			switch (scaleMode)
			{
			case ScaleMode.Fill:
				if (repeatX && repeatY)
				{
					num7 = (float)sprite.texture.width / num5;
					num8 = (float)sprite.texture.height / num6;
				}
				else if (repeatX || repeatY || dontStretch)
				{
					num7 = num6 / (float)sprite.texture.height * ((float)sprite.texture.width / num5);
					num8 = num5 / (float)sprite.texture.width * ((float)sprite.texture.height / num6);
				}
				break;
			case ScaleMode.Manual:
				y = (float)sprite.texture.height * num8;
				num7 *= (float)sprite.texture.width / num5;
				break;
			}
			bool flag = true;
			bool flag2 = true;
			if (dontStretch && !repeatX && !repeatY)
			{
				float num9 = (float)sprite.texture.width / (float)sprite.texture.height / (num5 / num6);
				if (num9 > 1f)
				{
					flag = false;
				}
				else if (num9 < 1f)
				{
					flag2 = false;
				}
			}
			sprite.size = new Vector2(x, y);
			if (scaleMode == ScaleMode.Manual)
			{
				switch (align)
				{
				case Align.None:
				{
					Transform transform4 = sprite.transform;
					float y5 = (float)Screen.height * position.y;
					Vector3 localPosition4 = sprite.transform.localPosition;
					MadTransform.SetLocalPosition(transform4, new Vector3(0f, y5, localPosition4.z));
					break;
				}
				case Align.Middle:
				{
					Transform transform3 = sprite.transform;
					float y4 = position.y;
					Vector3 localPosition3 = sprite.transform.localPosition;
					MadTransform.SetLocalPosition(transform3, new Vector3(0f, y4, localPosition3.z));
					break;
				}
				case Align.Bottom:
				{
					Transform transform2 = sprite.transform;
					float y3 = num6 * -0.5f + position.y + 0.5f * num8 * (float)sprite.texture.height;
					Vector3 localPosition2 = sprite.transform.localPosition;
					MadTransform.SetLocalPosition(transform2, new Vector3(0f, y3, localPosition2.z));
					break;
				}
				case Align.Top:
				{
					Transform transform = sprite.transform;
					float y2 = num6 * 0.5f + position.y + -0.5f * num8 * (float)sprite.texture.height;
					Vector3 localPosition = sprite.transform.localPosition;
					MadTransform.SetLocalPosition(transform, new Vector3(0f, y2, localPosition.z));
					break;
				}
				}
			}
			else
			{
				Transform transform5 = sprite.transform;
				Vector3 localPosition5 = sprite.transform.localPosition;
				MadTransform.SetLocalPosition(transform5, new Vector3(0f, 0f, localPosition5.z));
			}
			float num10 = 1f;
			float num11 = 1f;
			sprite.textureRepeat = new Vector2((!repeatX && flag) ? num10 : (num10 * (1f / num7)), (!repeatY && flag2) ? num11 : (num11 * (1f / num8)));
			Vector2 userPosition = parent.UserPosition;
			float num12 = (0f - userPosition.x) * followSpeed;
			float num13 = (0f - userPosition.y) * followSpeed;
			num12 /= root.screenWidth;
			num13 /= root.screenHeight;
			float num14 = 0f;
			float num15 = 0f;
			if (!flag)
			{
				num14 = (0f - (sprite.textureRepeat.x - 1f)) / 2f;
			}
			else if (!flag2)
			{
				num15 = (0f - (sprite.textureRepeat.y - 1f)) / 2f;
			}
			sprite.textureOffset = new Vector2((!repeatX && flag) ? num12 : (num12 * (1f / num7) + position.x + num14), (!repeatY && flag2) ? num13 : (num13 * (1f / num8) - position.y + num15));
			if (Application.isPlaying && scrollSpeed != Vector2.zero)
			{
				scrollAccel += new Vector2(scrollSpeed.x * 0.01f / scale.x, scrollSpeed.y * 0.01f / scale.y) * deltaTime;
				scrollAccel = new Vector2(scrollAccel.x % 1f, scrollAccel.y % 1f);
				sprite.textureOffset += scrollAccel;
			}
			UpdateFillMargin();
			lastTime = Time.realtimeSinceStartup;
		}

		private void UpdateFillMargin()
		{
			if (scaleMode == ScaleMode.Fill && !repeatX && !repeatY)
			{
				sprite.transform.localPosition += new Vector3(fillMarginLeft - fillMarginRight, fillMarginBottom - fillMarginTop, 0f) / 2f;
			}
		}
	}
}
