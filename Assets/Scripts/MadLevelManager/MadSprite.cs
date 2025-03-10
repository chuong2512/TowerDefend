using System;
using System.Collections.Generic;
using UnityEngine;

namespace MadLevelManager
{
	[ExecuteInEditMode]
	public class MadSprite : MadNode
	{
		private class Quad
		{
			public enum Point
			{
				TopLeft,
				TopRight,
				BottomRight,
				BottomLeft
			}

			public Point anchor;

			public float offset;

			public float progress;

			public bool invert;

			public Quad(bool invert)
			{
				this.invert = invert;
			}

			public Quad(Quad other)
			{
				anchor = other.anchor;
				offset = other.offset;
				progress = other.progress;
				invert = other.invert;
			}

			public Vector2[] Points(float left, float top, float right, float bottom)
			{
				if (progress == 0f)
				{
					return new Vector2[0];
				}
				if (progress == 1f)
				{
					return new Vector2[4]
					{
						new Vector2(left, bottom),
						new Vector2(left, top),
						new Vector2(right, top),
						new Vector2(right, bottom)
					};
				}
				float num = Y(progress + offset);
				float num2 = left + (right - left) * num;
				float num3 = bottom + (top - bottom) * num;
				float num4 = offset + progress;
				float num5 = Y(offset);
				float num6 = left + (right - left) * num5;
				float num7 = bottom + (top - bottom) * num5;
				switch (anchor)
				{
				case Point.BottomLeft:
					if (!invert)
					{
						if (!(num4 < 0.5f))
						{
							if (!(offset < 0.5f))
							{
								return new Vector2[3]
								{
									new Vector2(left, bottom),
									new Vector2(right, num7),
									new Vector2(right, num3)
								};
							}
							return new Vector2[4]
							{
								new Vector2(left, bottom),
								new Vector2(num6, top),
								new Vector2(right, top),
								new Vector2(right, num3)
							};
						}
						return new Vector2[3]
						{
							new Vector2(left, bottom),
							new Vector2(num6, top),
							new Vector2(num2, top)
						};
					}
					if (!(num4 < 0.5f))
					{
						if (!(offset < 0.5f))
						{
							return new Vector2[3]
							{
								new Vector2(left, bottom),
								new Vector2(num6, top),
								new Vector2(num2, top)
							};
						}
						return new Vector2[4]
						{
							new Vector2(left, bottom),
							new Vector2(right, num7),
							new Vector2(right, top),
							new Vector2(num2, top)
						};
					}
					return new Vector2[3]
					{
						new Vector2(left, bottom),
						new Vector2(right, num7),
						new Vector2(right, num3)
					};
				case Point.TopLeft:
					if (!invert)
					{
						if (!(num4 < 0.5f))
						{
							if (!(offset < 0.5f))
							{
								return new Vector2[3]
								{
									new Vector2(left, top),
									new Vector2(num6, bottom),
									new Vector2(num2, bottom)
								};
							}
							return new Vector2[4]
							{
								new Vector2(left, top),
								new Vector2(right, top - num7),
								new Vector2(right, bottom),
								new Vector2(num2, bottom)
							};
						}
						return new Vector2[3]
						{
							new Vector2(left, top),
							new Vector2(right, top - num7),
							new Vector2(right, top - num3)
						};
					}
					if (!(num4 < 0.5f))
					{
						if (!(offset < 0.5f))
						{
							return new Vector2[3]
							{
								new Vector2(left, top),
								new Vector2(right, top - num7),
								new Vector2(right, top - num3)
							};
						}
						return new Vector2[4]
						{
							new Vector2(left, top),
							new Vector2(num6, bottom),
							new Vector2(right, bottom),
							new Vector2(right, top - num3)
						};
					}
					return new Vector2[3]
					{
						new Vector2(left, top),
						new Vector2(num6, bottom),
						new Vector2(num2, bottom)
					};
				case Point.TopRight:
					if (!invert)
					{
						if (!(num4 < 0.5f))
						{
							if (!(offset < 0.5f))
							{
								return new Vector2[3]
								{
									new Vector2(right, top),
									new Vector2(left, top - num7),
									new Vector2(left, top - num3)
								};
							}
							return new Vector2[4]
							{
								new Vector2(right, top),
								new Vector2(right - num6, bottom),
								new Vector2(left, bottom),
								new Vector2(left, top - num3)
							};
						}
						return new Vector2[3]
						{
							new Vector2(right, top),
							new Vector2(right - num6, bottom),
							new Vector2(right - num2, bottom)
						};
					}
					if (!(num4 < 0.5f))
					{
						if (!(offset < 0.5f))
						{
							return new Vector2[3]
							{
								new Vector2(right, top),
								new Vector2(right - num6, bottom),
								new Vector2(right - num2, bottom)
							};
						}
						return new Vector2[4]
						{
							new Vector2(right, top),
							new Vector2(left, top - num7),
							new Vector2(left, bottom),
							new Vector2(right - num2, bottom)
						};
					}
					return new Vector2[3]
					{
						new Vector2(right, top),
						new Vector2(left, top - num7),
						new Vector2(left, top - num3)
					};
				case Point.BottomRight:
					if (!invert)
					{
						if (!(num4 < 0.5f))
						{
							if (!(offset < 0.5f))
							{
								return new Vector2[3]
								{
									new Vector2(right, bottom),
									new Vector2(right - num6, top),
									new Vector2(right - num2, top)
								};
							}
							return new Vector2[4]
							{
								new Vector2(right, bottom),
								new Vector2(left, num7),
								new Vector2(left, top),
								new Vector2(right - num2, top)
							};
						}
						return new Vector2[3]
						{
							new Vector2(right, bottom),
							new Vector2(left, num7),
							new Vector2(left, num3)
						};
					}
					if (!(num4 < 0.5f))
					{
						if (!(offset < 0.5f))
						{
							return new Vector2[3]
							{
								new Vector2(right, bottom),
								new Vector2(left, num7),
								new Vector2(left, num3)
							};
						}
						return new Vector2[4]
						{
							new Vector2(right, bottom),
							new Vector2(right - num6, top),
							new Vector2(left, top),
							new Vector2(left, num3)
						};
					}
					return new Vector2[3]
					{
						new Vector2(right, bottom),
						new Vector2(right - num6, top),
						new Vector2(right - num2, top)
					};
				default:
					UnityEngine.Debug.LogError("Should not be here");
					return new Vector2[0];
				}
			}

			private float Y(float val)
			{
				float num = 1f;
				float num2 = (!(val < 0.5f)) ? (1f - val) : val;
				float f = num2 * 90f * ((float)Math.PI / 180f);
				return Mathf.Tan(f) * num;
			}
		}

		public enum InputType
		{
			SingleTexture,
			TextureAtlas
		}

		public enum PivotPoint
		{
			BottomLeft,
			TopLeft,
			TopRight,
			BottomRight,
			Left,
			Top,
			Right,
			Bottom,
			Center,
			Custom
		}

		public enum FillType
		{
			None,
			LeftToRight,
			RightToLeft,
			BottomToTop,
			TopToBottom,
			ExpandHorizontal,
			ExpandVertical,
			RadialCW,
			RadialCCW
		}

		public enum EventFlags
		{
			None = 0,
			MouseHover = 1,
			MouseClick = 2,
			Touch = 4,
			Focus = 8,
			All = 0xF
		}

		public delegate void Action(MadSprite sprite);

		public delegate void SetupShader(Material material);

		public MadPanel panel;

		private MadPanel cachedPanel;

		public bool visible = true;

		public bool editorSelectable = true;

		public PivotPoint pivotPoint = PivotPoint.Center;

		public Vector2 customPivotPoint = new Vector2(0f, 0f);

		public Color tint = Color.white;

		public InputType inputType;

		public Texture2D texture;

		public Texture2D lastTexture;

		public MadAtlas textureAtlas;

		public string textureAtlasSpriteGUID;

		public string lastTextureAtlasSpriteGUID;

		public Vector2 textureOffset;

		public Vector2 textureRepeat = new Vector2(1f, 1f);

		public bool hasPremultipliedAlpha;

		public int guiDepth;

		public EventFlags eventFlags = EventFlags.All;

		protected float left;

		protected float top;

		protected float right;

		protected float bottom;

		public float liveLeft;

		public float liveBottom;

		public float liveRight = 1f;

		public float liveTop = 1f;

		public bool hasLiveBounds;

		public bool renderLiveBoundsOnly;

		private bool triedToGetLiveBounds;

		public FillType fillType;

		public float fillValue = 1f;

		public float radialFillOffset;

		public float radialFillLength = 1f;

		private bool actionsInitialized;

		private string shaderName;

		private SetupShader setupShaderFunction;

		private int materialVariation;

		private Vector2 _initialSize;

		private bool _hasFocus;

		private Action _onMouseEnter = delegate
		{
		};

		private Action _onMouseExit = delegate
		{
		};

		private Action _onTouchEnter = delegate
		{
		};

		private Action _onTouchExit = delegate
		{
		};

		private Action _onMouseDown = delegate
		{
		};

		private Action _onMouseUp = delegate
		{
		};

		private Action _onTap = delegate
		{
		};

		private Action _onFocus = delegate
		{
		};

		private Action _onFocusLost = delegate
		{
		};

		public Texture2D currentTexture
		{
			get
			{
				switch (inputType)
				{
				case InputType.SingleTexture:
					return texture;
				case InputType.TextureAtlas:
					if (textureAtlas != null)
					{
						return textureAtlas.atlasTexture;
					}
					return null;
				default:
					UnityEngine.Debug.LogError("Unknown input type: " + inputType);
					return null;
				}
			}
		}

		public int currentTextureWidth
		{
			get
			{
				switch (inputType)
				{
				case InputType.SingleTexture:
					return texture.width;
				case InputType.TextureAtlas:
					return textureAtlas.GetItem(textureAtlasSpriteGUID).pixelsWidth;
				default:
					UnityEngine.Debug.LogError("Unknown input type: " + inputType);
					return 0;
				}
			}
		}

		public int currentTextureHeight
		{
			get
			{
				switch (inputType)
				{
				case InputType.SingleTexture:
					return texture.height;
				case InputType.TextureAtlas:
					return textureAtlas.GetItem(textureAtlasSpriteGUID).pixelsHeight;
				default:
					UnityEngine.Debug.LogError("Unknown input type: " + inputType);
					return 0;
				}
			}
		}

		public Vector2 initialSize
		{
			get
			{
				if (_initialSize == Vector2.zero)
				{
					if (currentTexture != null)
					{
						ResizeToTexture();
					}
					else
					{
						UnityEngine.Debug.LogError("Requesting size of sprite without texture.", this);
					}
				}
				return _initialSize;
			}
			set
			{
				_initialSize = value;
			}
		}

		public Vector2 size
		{
			get
			{
				Vector3 localScale = base.transform.localScale;
				float x = localScale.x;
				Vector2 initialSize = this.initialSize;
				float x2 = x * initialSize.x;
				Vector3 localScale2 = base.transform.localScale;
				float y = localScale2.y;
				Vector2 initialSize2 = this.initialSize;
				return new Vector2(x2, y * initialSize2.y);
			}
			set
			{
				Transform transform = base.transform;
				float x = value.x;
				Vector2 initialSize = this.initialSize;
				float x2 = x / initialSize.x;
				float y = value.y;
				Vector2 initialSize2 = this.initialSize;
				MadTransform.SetLocalScale(transform, x2, y / initialSize2.y, 1f);
			}
		}

		public bool hasFocus
		{
			get
			{
				return _hasFocus;
			}
			set
			{
				if (!_hasFocus && value)
				{
					if (panel.focusedSprite != null && panel.focusedSprite != this)
					{
						panel.focusedSprite._hasFocus = false;
						panel.focusedSprite.onFocusLost(panel.focusedSprite);
					}
					panel.focusedSprite = this;
					_hasFocus = true;
					onFocus(this);
				}
				else if (_hasFocus && !value)
				{
					panel.focusedSprite = null;
					onFocusLost(this);
					_hasFocus = false;
				}
			}
		}

		public Action onMouseEnter
		{
			get
			{
				if ((eventFlags & EventFlags.MouseHover) != 0)
				{
					return _onMouseEnter;
				}
				return NullAction;
			}
			set
			{
				InitActions();
				_onMouseEnter = value;
			}
		}

		public Action onMouseExit
		{
			get
			{
				if ((eventFlags & EventFlags.MouseHover) != 0)
				{
					return _onMouseExit;
				}
				return NullAction;
			}
			set
			{
				InitActions();
				_onMouseExit = value;
			}
		}

		public Action onTouchEnter
		{
			get
			{
				if ((eventFlags & EventFlags.Touch) != 0)
				{
					return _onTouchEnter;
				}
				return NullAction;
			}
			set
			{
				InitActions();
				_onTouchEnter = value;
			}
		}

		public Action onTouchExit
		{
			get
			{
				if ((eventFlags & EventFlags.Touch) != 0)
				{
					return _onTouchExit;
				}
				return NullAction;
			}
			set
			{
				InitActions();
				_onTouchExit = value;
			}
		}

		public Action onMouseDown
		{
			get
			{
				if ((eventFlags & EventFlags.MouseClick) != 0)
				{
					return _onMouseDown;
				}
				return NullAction;
			}
			set
			{
				InitActions();
				_onMouseDown = value;
			}
		}

		public Action onMouseUp
		{
			get
			{
				MadDraggable madDraggable = FindParent<MadDraggable>();
				if ((madDraggable == null || !madDraggable.dragging) && (eventFlags & EventFlags.MouseClick) != 0)
				{
					return _onMouseUp;
				}
				return NullAction;
			}
			set
			{
				InitActions();
				_onMouseUp = value;
			}
		}

		public Action onTap
		{
			get
			{
				if ((eventFlags & EventFlags.Touch) != 0)
				{
					return _onTap;
				}
				return NullAction;
			}
			set
			{
				InitActions();
				_onTap = value;
			}
		}

		public Action onFocus
		{
			get
			{
				if ((eventFlags & EventFlags.Focus) != 0)
				{
					return _onFocus;
				}
				return NullAction;
			}
			set
			{
				InitActions();
				_onFocus = value;
			}
		}

		public Action onFocusLost
		{
			get
			{
				if ((eventFlags & EventFlags.Focus) != 0)
				{
					return _onFocusLost;
				}
				return NullAction;
			}
			set
			{
				InitActions();
				_onFocusLost = value;
			}
		}

		private void NullAction(MadSprite s)
		{
		}

		public virtual Rect GetBounds()
		{
			UpdatePivotPoint();
			float num = left;
			Vector2 initialSize = this.initialSize;
			float x = num * initialSize.x;
			float num2 = bottom;
			Vector2 initialSize2 = this.initialSize;
			float y = num2 * initialSize2.y;
			Vector2 initialSize3 = this.initialSize;
			float x2 = initialSize3.x;
			Vector2 initialSize4 = this.initialSize;
			Rect result = new Rect(x, y, x2, initialSize4.y);
			return result;
		}

		public virtual Rect GetTransformedBounds()
		{
			UpdatePivotPoint();
			Rect bounds = GetBounds();
			bounds = MadMath.Scale(bounds, base.transform.localScale);
			return MadMath.Translate(bounds, base.transform.localPosition);
		}

		private Rect GetLiveBounds()
		{
			UpdatePivotPoint();
			float num = left + liveLeft;
			Vector2 initialSize = this.initialSize;
			float x = num * initialSize.x;
			float num2 = bottom + liveBottom;
			Vector2 initialSize2 = this.initialSize;
			float y = num2 * initialSize2.y;
			float num3 = liveRight - liveLeft;
			Vector2 initialSize3 = this.initialSize;
			float width = num3 * initialSize3.x;
			float num4 = liveTop - liveBottom;
			Vector2 initialSize4 = this.initialSize;
			return new Rect(x, y, width, num4 * initialSize4.y);
		}

		public void GetWorldCorners(ref Vector3[] arr)
		{
			Rect bounds = GetBounds();
			Transform transform = base.transform;
			float xMin = bounds.xMin;
			float yMax = bounds.yMax;
			Vector3 position = base.transform.position;
			Vector3 vector = transform.TransformPoint(new Vector3(xMin, yMax, position.z));
			Transform transform2 = base.transform;
			float xMax = bounds.xMax;
			float yMax2 = bounds.yMax;
			Vector3 position2 = base.transform.position;
			Vector3 vector2 = transform2.TransformPoint(new Vector3(xMax, yMax2, position2.z));
			Transform transform3 = base.transform;
			float xMax2 = bounds.xMax;
			float yMin = bounds.yMin;
			Vector3 position3 = base.transform.position;
			Vector3 vector3 = transform3.TransformPoint(new Vector3(xMax2, yMin, position3.z));
			Transform transform4 = base.transform;
			float xMin2 = bounds.xMin;
			float yMin2 = bounds.yMin;
			Vector3 position4 = base.transform.position;
			Vector3 vector4 = transform4.TransformPoint(new Vector3(xMin2, yMin2, position4.z));
			arr[0] = vector;
			arr[1] = vector2;
			arr[2] = vector3;
			arr[3] = vector4;
		}

		protected virtual void OnEnable()
		{
			if (panel == null)
			{
				panel = MadPanel.FirstOrNull(base.transform);
				cachedPanel = panel;
			}
			else
			{
				cachedPanel = null;
			}
			if (!MadTransform.instantiating)
			{
				RegisterSprite();
			}
			UpdateTexture();
		}

		public void TryFocus()
		{
			if ((eventFlags & EventFlags.Focus) != 0)
			{
				hasFocus = true;
			}
		}

		private void OnDisable()
		{
			UnregisterSprite(panel);
		}

		protected virtual void Start()
		{
			RegisterSprite();
		}

		private void OnDestroy()
		{
			UnregisterSprite(panel);
		}

		private void RegisterSpriteIfNeeded()
		{
			if (panel != cachedPanel)
			{
				if (cachedPanel != null)
				{
					UnregisterSprite(cachedPanel);
				}
				cachedPanel = panel;
				if (panel != null)
				{
					RegisterSprite();
				}
			}
		}

		private void RegisterSprite()
		{
			if (panel != null)
			{
				panel.sprites.Add(this);
			}
			else
			{
				UnityEngine.Debug.LogError("Panel not set or cannot find any panel on scene.");
			}
		}

		private void UnregisterSprite(MadPanel panel)
		{
			if (panel != null)
			{
				panel.sprites.Remove(this);
			}
		}

		protected virtual void Update()
		{
			RegisterSpriteIfNeeded();
			UpdateRenderMode();
			UpdateTexture();
			if (panel == null)
			{
			}
			if (NeedLiveBounds())
			{
				RecalculateLiveBounds();
			}
		}

		private void UpdateRenderMode()
		{
			if (panel.renderMode == MadPanel.RenderMode.DepthBased)
			{
				Vector3 localPosition = base.transform.localPosition;
				MadTransform.SetLocalPosition(base.transform, new Vector3(localPosition.x, localPosition.y, -guiDepth * 10));
			}
		}

		private void UpdateTexture()
		{
			if (texture != lastTexture || textureAtlasSpriteGUID != lastTextureAtlasSpriteGUID)
			{
				liveLeft = (liveBottom = 0f);
				liveRight = (liveTop = 1f);
				hasLiveBounds = false;
				triedToGetLiveBounds = false;
				if (NeedLiveBounds())
				{
					RecalculateLiveBounds();
				}
				lastTexture = texture;
				lastTextureAtlasSpriteGUID = textureAtlasSpriteGUID;
			}
		}

		private bool NeedLiveBounds()
		{
			return fillType != 0 && !hasLiveBounds && !triedToGetLiveBounds && currentTexture != null && (inputType != InputType.TextureAtlas || !string.IsNullOrEmpty(textureAtlasSpriteGUID));
		}

		public void SetMaterial(string shader, SetupShader setupShader)
		{
			shaderName = shader;
			setupShaderFunction = setupShader;
		}

		private void InitActions()
		{
			if (Application.isPlaying && !actionsInitialized)
			{
				BoxCollider boxCollider = base.gameObject.GetComponent<BoxCollider>();
				if (boxCollider == null)
				{
					boxCollider = base.gameObject.AddComponent<BoxCollider>();
				}
				Rect rect = (!hasLiveBounds) ? GetBounds() : GetLiveBounds();
				boxCollider.center = rect.center;
				boxCollider.size = new Vector3(rect.width, rect.height, 0.01f);
				actionsInitialized = true;
			}
		}

		public virtual bool CanDraw()
		{
			if (inputType == InputType.TextureAtlas && (textureAtlas == null || string.IsNullOrEmpty(textureAtlasSpriteGUID) || textureAtlas.GetItem(textureAtlasSpriteGUID) == null))
			{
				return false;
			}
			return currentTexture != null;
		}

		public virtual Material GetMaterial()
		{
			Material material;
			if (string.IsNullOrEmpty(shaderName) || setupShaderFunction == null)
			{
				material = (hasPremultipliedAlpha ? ((panel.renderMode != MadPanel.RenderMode.DepthBased) ? panel.materialStore.MaterialFor(currentTexture, "Tools/Mad Level Manager/Unlit/Transparent Tint Pre") : panel.materialStore.MaterialFor(currentTexture, "Tools/Mad Level Manager/Unlit/Transparent Tint Pre Depth Based")) : ((panel.renderMode != MadPanel.RenderMode.DepthBased) ? panel.materialStore.MaterialFor(currentTexture, "Tools/Mad Level Manager/Unlit/Transparent Tint") : panel.materialStore.MaterialFor(currentTexture, "Tools/Mad Level Manager/Unlit/Transparent Tint Depth Based")));
			}
			else
			{
				material = ((materialVariation != 0) ? panel.materialStore.MaterialFor(currentTexture, shaderName, materialVariation) : panel.materialStore.CreateUnique(currentTexture, shaderName, out materialVariation));
				setupShaderFunction(material);
			}
			return material;
		}

		public virtual void DrawOn(ref MadList<Vector3> vertices, ref MadList<Color32> colors, ref MadList<Vector2> uv, ref MadList<int> triangles, out Material material)
		{
			UpdatePivotPoint();
			if (fillType == FillType.None || fillValue > 0f)
			{
				if ((fillType == FillType.RadialCW || fillType == FillType.RadialCCW) && (fillValue != 1f || radialFillLength != 1f))
				{
					DrawOnQuad(ref vertices, ref colors, ref uv, ref triangles);
				}
				else
				{
					DrawOnRegular(ref vertices, ref colors, ref uv, ref triangles);
				}
			}
			material = GetMaterial();
		}

		public void DrawOnRegular(ref MadList<Vector3> vertices, ref MadList<Color32> colors, ref MadList<Vector2> uv, ref MadList<int> triangles)
		{
			Matrix4x4 matrix4x = TransformMatrix();
			Rect bounds = GetBounds();
			float num = 0f;
			Vector2 initialSize = this.initialSize;
			float num2 = initialSize.y;
			Vector2 initialSize2 = this.initialSize;
			float num3 = initialSize2.x;
			float num4 = 0f;
			float num5 = textureOffset.x;
			float num6 = textureOffset.y + textureRepeat.y;
			float num7 = textureOffset.x + textureRepeat.x;
			float num8 = textureOffset.y;
			fillValue = Mathf.Clamp01(fillValue);
			Rect rect = new Rect(0f, 0f, 1f, 1f);
			if (renderLiveBoundsOnly)
			{
				num5 += LiveCoordX(0f);
				float num9 = num;
				float num10 = LiveCoordX(0f);
				Vector2 initialSize3 = this.initialSize;
				num = num9 + num10 * initialSize3.x;
				num3 *= LiveCoordX(1f);
				num7 += LiveCoordX(1f) - 1f;
				num2 *= LiveCoordY(1f);
				num6 += LiveCoordY(1f) - 1f;
				float num11 = num4;
				float num12 = LiveCoordY(0f);
				Vector2 initialSize4 = this.initialSize;
				num4 = num11 + num12 * initialSize4.y;
				num8 += LiveCoordY(0f);
				rect = new Rect(LiveCoordX(0f), LiveCoordY(0f), LiveCoordX(1f) - LiveCoordX(0f), LiveCoordY(1f) - LiveCoordY(0f));
			}
			if (fillValue != 1f)
			{
				switch (fillType)
				{
				case FillType.LeftToRight:
				{
					num7 = rect.xMin + rect.width * fillValue;
					float num22 = num7;
					Vector2 initialSize12 = this.initialSize;
					num3 = num22 * initialSize12.x;
					break;
				}
				case FillType.RightToLeft:
				{
					num5 = rect.xMax - rect.width * fillValue;
					float num21 = num5;
					Vector2 initialSize11 = this.initialSize;
					num = num21 * initialSize11.x;
					break;
				}
				case FillType.BottomToTop:
				{
					num6 = rect.yMin + rect.height * fillValue;
					float num20 = num6;
					Vector2 initialSize10 = this.initialSize;
					num2 = num20 * initialSize10.y;
					break;
				}
				case FillType.TopToBottom:
				{
					num8 = rect.yMax - rect.height * fillValue;
					float num19 = num8;
					Vector2 initialSize9 = this.initialSize;
					num4 = num19 * initialSize9.y;
					break;
				}
				case FillType.ExpandHorizontal:
				{
					float num16 = 0.5f + fillValue / 2f;
					num7 = rect.xMin + rect.width * num16;
					float num17 = num7;
					Vector2 initialSize7 = this.initialSize;
					num3 = num17 * initialSize7.x;
					num5 = rect.xMax - rect.width * num16;
					float num18 = num5;
					Vector2 initialSize8 = this.initialSize;
					num = num18 * initialSize8.x;
					break;
				}
				case FillType.ExpandVertical:
				{
					float num13 = 0.5f + fillValue / 2f;
					num6 = rect.yMin + rect.height * num13;
					float num14 = num6;
					Vector2 initialSize5 = this.initialSize;
					num2 = num14 * initialSize5.y;
					num8 = rect.yMax - rect.height * num13;
					float num15 = num8;
					Vector2 initialSize6 = this.initialSize;
					num4 = num15 * initialSize6.y;
					break;
				}
				}
			}
			vertices.Add(matrix4x.MultiplyPoint(PivotPointTranslate(new Vector3(num, num4, 0f), bounds)));
			vertices.Add(matrix4x.MultiplyPoint(PivotPointTranslate(new Vector3(num, num2, 0f), bounds)));
			vertices.Add(matrix4x.MultiplyPoint(PivotPointTranslate(new Vector3(num3, num2, 0f), bounds)));
			vertices.Add(matrix4x.MultiplyPoint(PivotPointTranslate(new Vector3(num3, num4, 0f), bounds)));
			Color32 e = tint;
			colors.Add(e);
			colors.Add(e);
			colors.Add(e);
			colors.Add(e);
			uv.Add(FixUV(new Vector2(num5, num8)));
			uv.Add(FixUV(new Vector2(num5, num6)));
			uv.Add(FixUV(new Vector2(num7, num6)));
			uv.Add(FixUV(new Vector2(num7, num8)));
			int num23 = vertices.Count - 4;
			triangles.Add(num23);
			triangles.Add(1 + num23);
			triangles.Add(2 + num23);
			triangles.Add(num23);
			triangles.Add(2 + num23);
			triangles.Add(3 + num23);
		}

		public void DrawOnQuad(ref MadList<Vector3> vertices, ref MadList<Color32> colors, ref MadList<Vector2> uv, ref MadList<int> triangles)
		{
			bool flag = fillType == FillType.RadialCCW;
			Matrix4x4 matrix4x = TransformMatrix();
			Rect bounds = GetBounds();
			Quad quad = new Quad(flag);
			Quad quad2 = new Quad(flag);
			Quad quad3 = new Quad(flag);
			Quad quad4 = new Quad(flag);
			quad.anchor = Quad.Point.BottomRight;
			quad2.anchor = Quad.Point.BottomLeft;
			quad3.anchor = Quad.Point.TopLeft;
			quad4.anchor = Quad.Point.TopRight;
			Quad quad5 = new Quad(quad);
			Quad quad6 = new Quad(quad2);
			Quad quad7 = new Quad(quad3);
			Quad quad8 = new Quad(quad4);
			Quad[] array = new Quad[8];
			if (!flag)
			{
				array[0] = quad2;
				array[1] = quad3;
				array[2] = quad4;
				array[3] = quad;
				array[4] = quad6;
				array[5] = quad7;
				array[6] = quad8;
				array[7] = quad5;
			}
			else
			{
				array[7] = quad6;
				array[6] = quad7;
				array[5] = quad8;
				array[4] = quad5;
				array[3] = quad2;
				array[2] = quad3;
				array[1] = quad4;
				array[0] = quad;
			}
			float num = radialFillOffset % 1f;
			if (num < 0f)
			{
				num += 1f;
			}
			float num2 = Mathf.Clamp01(fillValue) * radialFillLength;
			float num3 = num * 4f;
			float num4 = (num + num2) * 4f;
			for (int i = Mathf.FloorToInt(num3); i < Mathf.CeilToInt(num4); i++)
			{
				Quad quad9 = array[i % 8];
				if ((float)i < num3)
				{
					quad9.offset = num3 - (float)i;
				}
				else
				{
					quad9.offset = 0f;
				}
				if (num4 > (float)(i + 1))
				{
					quad9.progress = 1f - quad9.offset;
				}
				else
				{
					quad9.progress = num4 - (float)i - quad9.offset;
				}
			}
			Vector2 initialSize = this.initialSize;
			float x = initialSize.x;
			Vector2 initialSize2 = this.initialSize;
			float y = initialSize2.y;
			float num5 = x / 2f;
			float num6 = y / 2f;
			List<Vector2[]> list = new List<Vector2[]>();
			List<Vector2[]> list2 = new List<Vector2[]>();
			list.Add(quad2.Points(num5, y, x, num6));
			list2.Add(quad2.Points(0.5f, 1f, 1f, 0.5f));
			list.Add(quad6.Points(num5, y, x, num6));
			list2.Add(quad6.Points(0.5f, 1f, 1f, 0.5f));
			list.Add(quad3.Points(num5, num6, x, 0f));
			list2.Add(quad3.Points(0.5f, 0.5f, 1f, 0f));
			list.Add(quad7.Points(num5, num6, x, 0f));
			list2.Add(quad7.Points(0.5f, 0.5f, 1f, 0f));
			list.Add(quad4.Points(0f, num6, num5, 0f));
			list2.Add(quad4.Points(0f, 0.5f, 0.5f, 0f));
			list.Add(quad8.Points(0f, num6, num5, 0f));
			list2.Add(quad8.Points(0f, 0.5f, 0.5f, 0f));
			list.Add(quad.Points(0f, y, num5, num6));
			list2.Add(quad.Points(0f, 1f, 0.5f, 0.5f));
			list.Add(quad5.Points(0f, y, num5, num6));
			list2.Add(quad5.Points(0f, 1f, 0.5f, 0.5f));
			Color32 e = tint;
			for (int j = 0; j < 8; j++)
			{
				Vector2[] array2 = list[j];
				Vector2[] array3 = list2[j];
				if (array2.Length != 0)
				{
					int count = vertices.Count;
					for (int k = 0; k < array2.Length; k++)
					{
						vertices.Add(matrix4x.MultiplyPoint(PivotPointTranslate(array2[k], bounds)));
						uv.Add(FixUV(array3[k]));
						colors.Add(e);
					}
					triangles.Add(count);
					triangles.Add(1 + count);
					triangles.Add(2 + count);
					if (array2.Length > 3)
					{
						triangles.Add(count);
						triangles.Add(2 + count);
						triangles.Add(3 + count);
					}
				}
			}
		}

		private float LiveCoordX(float pos)
		{
			return liveLeft + (liveRight - liveLeft) * pos;
		}

		private float LiveCoordY(float pos)
		{
			return liveBottom + (liveTop - liveBottom) * pos;
		}

		private Vector2 FixUV(Vector2 uv)
		{
			if (inputType == InputType.TextureAtlas)
			{
				MadAtlas.Item item = textureAtlas.GetItem(textureAtlasSpriteGUID);
				Rect region = item.region;
				return new Vector2(region.x + region.width * uv.x, region.y + region.height * uv.y);
			}
			return uv;
		}

		protected Matrix4x4 TransformMatrix()
		{
			return panel.transform.worldToLocalMatrix * base.transform.localToWorldMatrix;
		}

		protected void UpdatePivotPoint()
		{
			switch (pivotPoint)
			{
			case PivotPoint.BottomLeft:
				left = 0f;
				bottom = 0f;
				break;
			case PivotPoint.TopLeft:
				left = 0f;
				bottom = -1f;
				break;
			case PivotPoint.TopRight:
				left = -1f;
				bottom = -1f;
				break;
			case PivotPoint.BottomRight:
				left = -1f;
				bottom = 0f;
				break;
			case PivotPoint.Left:
				left = 0f;
				bottom = -0.5f;
				break;
			case PivotPoint.Top:
				left = -0.5f;
				bottom = -1f;
				break;
			case PivotPoint.Right:
				left = -1f;
				bottom = -0.5f;
				break;
			case PivotPoint.Bottom:
				left = -0.5f;
				bottom = 0f;
				break;
			case PivotPoint.Center:
				left = -0.5f;
				bottom = -0.5f;
				break;
			case PivotPoint.Custom:
				left = 0f - customPivotPoint.x;
				bottom = 0f - customPivotPoint.y;
				break;
			default:
				UnityEngine.Debug.LogError("Unkwnown pivot point: " + pivotPoint);
				break;
			}
			top = bottom + 1f;
			right = left + 1f;
		}

		protected Vector3 PivotPointTranslate(Vector3 p, Rect bounds)
		{
			return new Vector3(p.x + bounds.width * left, p.y + bounds.height * bottom, p.z);
		}

		public void ResizeToTexture()
		{
			switch (inputType)
			{
			case InputType.SingleTexture:
				if (texture != null)
				{
					initialSize = new Vector2(texture.width, texture.height);
				}
				break;
			case InputType.TextureAtlas:
			{
				MadAtlas.Item item = textureAtlas.GetItem(textureAtlasSpriteGUID);
				if (item != null)
				{
					initialSize = new Vector2(item.pixelsWidth, item.pixelsHeight);
				}
				break;
			}
			default:
				UnityEngine.Debug.LogError("Unknown input type: " + inputType);
				break;
			}
		}

		public void MinMaxDepthRecursively(out int min, out int max)
		{
			min = guiDepth;
			max = guiDepth;
			List<MadSprite> list = MadTransform.FindChildren<MadSprite>(base.transform);
			foreach (MadSprite item in list)
			{
				min = Mathf.Min(min, item.guiDepth);
				max = Mathf.Max(max, item.guiDepth);
			}
		}

		public void RecalculateLiveBounds()
		{
			triedToGetLiveBounds = true;
			int width = currentTexture.width;
			int num;
			int num2;
			int num3;
			int num4;
			if (inputType == InputType.TextureAtlas)
			{
				MadAtlas.Item item = textureAtlas.GetItem(textureAtlasSpriteGUID);
				Rect region = item.region;
				num = Mathf.RoundToInt(region.x * (float)currentTexture.width);
				num2 = Mathf.RoundToInt(region.y * (float)currentTexture.height);
				num3 = Mathf.RoundToInt(region.width * (float)currentTexture.width);
				num4 = Mathf.RoundToInt(region.height * (float)currentTexture.height);
			}
			else
			{
				num = (num2 = 0);
				num3 = currentTexture.width;
				num4 = currentTexture.height;
			}
			Color32[] pixels = currentTexture.GetPixels32();
			int num5 = -1;
			int num6 = -1;
			int num7 = -1;
			int num8 = -1;
			int num9 = 0;
			for (int i = 0; i < num4; i++)
			{
				for (int j = 0; j < num3; j++)
				{
					num9 = (num2 + i) * width + (num + j);
					Color32 color = pixels[num9];
					if (IsOpaque(color))
					{
						if (num5 == -1 || j < num5)
						{
							num5 = j;
						}
						if (num6 == -1)
						{
							num6 = i;
						}
						if (j > num7)
						{
							num7 = j;
						}
						num8 = i;
					}
				}
			}
			liveLeft = (float)num5 / (float)num3;
			liveBottom = (float)num6 / (float)num4;
			liveRight = (float)num7 / (float)num3;
			liveTop = (float)num8 / (float)num4;
			hasLiveBounds = true;
		}

		private bool IsOpaque(Color32 color)
		{
			return color.a != 0;
		}

		public void AnimScale(Vector3 from, Vector3 to, float time, MadiTween.EaseType easing)
		{
			base.transform.localScale = from;
			AnimScaleTo(to, time, easing);
		}

		public void AnimScaleTo(Vector3 scale, float time, MadiTween.EaseType easing)
		{
			MadiTween.ScaleTo(base.gameObject, MadiTween.Hash("scale", scale, "time", time, "easetype", easing));
		}

		public void AnimMove(Vector3 from, Vector3 to, float time, MadiTween.EaseType easing)
		{
			base.transform.localPosition = from;
			AnimMoveTo(to, time, easing);
		}

		public void AnimMoveTo(Vector3 target, float time, MadiTween.EaseType easing)
		{
			MadiTween.MoveTo(base.gameObject, MadiTween.Hash("position", target, "time", time, "easetype", easing, "islocal", true));
		}

		public void AnimRotate(Vector3 from, Vector3 to, float time, MadiTween.EaseType easing)
		{
			base.transform.localScale = from;
			AnimScaleTo(to, time, easing);
		}

		public void AnimRotateTo(Vector3 rotation, float time, MadiTween.EaseType easing)
		{
			MadiTween.RotateTo(base.gameObject, MadiTween.Hash("rotation", rotation, "time", time, "easetype", easing, "islocal", true));
		}

		public void AnimColor(Color from, Color to, float time, MadiTween.EaseType easing)
		{
			tint = from;
			AnimColorTo(to, time, easing);
		}

		public void AnimColorTo(Color color, float time, MadiTween.EaseType easing)
		{
			MadiTween.ValueTo(base.gameObject, MadiTween.Hash("from", tint, "to", color, "time", time, "onupdate", "OnTintChange", "easetype", easing));
		}

		public void OnTintChange(Color color)
		{
			tint = color;
		}
	}
}
