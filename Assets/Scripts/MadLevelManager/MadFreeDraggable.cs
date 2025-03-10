using System;
using UnityEngine;

namespace MadLevelManager
{
	[ExecuteInEditMode]
	public class MadFreeDraggable : MadDraggable
	{
		public enum ScaleMode
		{
			None,
			FitToAreaWidth,
			FitToAreaHeight,
			Free
		}

		public Bounds dragBounds = new Bounds(Vector3.zero, new Vector3(400f, 400f));

		public ScaleMode scaleMode;

		public float scalingMax = 2f;

		public float scalingMin = 0.25f;

		public bool moveEasing = true;

		public bool scaleEasing = true;

		public MadiTween.EaseType scaleEasingType = MadiTween.EaseType.easeOutQuad;

		public float scaleEasingDuration = 0.5f;

		private Vector3 scaleSource;

		private Vector3 scaleTarget;

		private float scaleStartTime;

		private bool moveAnim;

		private Vector3 moveAnimStartPosition;

		private Vector3 moveAnimEndPosition;

		private float moveAnimStartTime;

		private float moveAnimDuration;

		private MadiTween.EaseType moveAnimEaseType;

		[Obsolete("Use dragBounds.")]
		public Rect dragArea = new Rect(0f, 0f, 0f, 0f);

		[Obsolete("Use scaleMode.")]
		public bool scaling;

		public override Vector2 progress
		{
			get
			{
				MadRootNode madRootNode = MadTransform.FindParent<MadRootNode>(base.transform);
				Vector3 min = dragBounds.min;
				float x = min.x;
				Vector3 min2 = dragBounds.min;
				Vector2 vector = new Vector2(x, min2.y);
				Vector3 max = dragBounds.max;
				float x2 = max.x;
				Vector3 max2 = dragBounds.max;
				Vector2 vector2 = new Vector2(x2, max2.y);
				Vector3 vector3 = base.transform.InverseTransformPoint(madRootNode.ScreenGlobal(0f, 0f));
				Vector3 vector4 = base.transform.InverseTransformPoint(madRootNode.ScreenGlobal(1f, 1f));
				float num = vector4.x - vector3.x;
				float num2 = vector4.y - vector3.y;
				float num3 = vector2.x - vector.x;
				float num4 = vector2.y - vector.y;
				return new Vector2((vector3.x - vector.x) / (num3 - num), (vector3.y - vector.y) / (num4 - num2));
			}
		}

		private void OnValidate()
		{
			scalingMin = Mathf.Min(scalingMin, scalingMax);
			scalingMax = Mathf.Max(scalingMin, scalingMax);
		}

		private void OnDrawGizmosSelected()
		{
			Vector3 center = base.transform.TransformPoint(dragBounds.center);
			Transform transform = base.transform;
			Vector3 max = dragBounds.max;
			float x = max.x;
			Vector3 max2 = dragBounds.max;
			Vector3 vector = transform.TransformPoint(new Vector3(x, max2.y, 0.01f));
			Transform transform2 = base.transform;
			Vector3 min = dragBounds.min;
			float x2 = min.x;
			Vector3 min2 = dragBounds.min;
			Vector3 vector2 = transform2.TransformPoint(new Vector3(x2, min2.y, 0.01f));
			Vector2 v = new Vector2(vector.x - vector2.x, vector.y - vector2.y);
			Gizmos.color = Color.red;
			Gizmos.DrawWireCube(center, v);
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			Upgrade();
		}

		private void Upgrade()
		{
			Rect rect = dragArea;
			bool flag = scaling;
			if (rect.width != 0f)
			{
				dragBounds = new Bounds(rect.center, new Vector2(rect.xMax - rect.xMin, rect.yMax - rect.yMin));
				dragArea = new Rect(0f, 0f, 0f, 0f);
			}
			if (flag)
			{
				scaleMode = ScaleMode.Free;
				scaling = false;
			}
		}

		protected override void Start()
		{
			base.Start();
			scaleSource = (scaleTarget = base.transform.localScale);
			StartScaleMode();
		}

		private void StartScaleMode()
		{
			if (scaleMode == ScaleMode.FitToAreaWidth || scaleMode == ScaleMode.FitToAreaHeight)
			{
				MadRootNode madRootNode = MadTransform.FindParent<MadRootNode>(base.transform);
				Vector3 localScale;
				switch (scaleMode)
				{
				case ScaleMode.FitToAreaWidth:
				{
					Vector3 size2 = dragBounds.size;
					float x = size2.x;
					float screenWidth = madRootNode.screenWidth;
					localScale = Vector3.one * screenWidth / x;
					break;
				}
				case ScaleMode.FitToAreaHeight:
				{
					Vector3 size = dragBounds.size;
					float y = size.y;
					float screenHeight = madRootNode.screenHeight;
					localScale = Vector3.one * screenHeight / y;
					break;
				}
				default:
					UnityEngine.Debug.Log("Unknown scale mode: " + scaleMode);
					localScale = Vector3.one;
					break;
				}
				base.transform.localScale = localScale;
			}
		}

		protected override void Update()
		{
			if (!Application.isPlaying)
			{
				return;
			}
			base.Update();
			base.cachedCamPos = base.cameraPos;
			UpdateScaling();
			UpdateMoving();
			if (!IsTouchingSingle())
			{
				base.dragging = false;
				if (scaleMode == ScaleMode.Free)
				{
					float num = ScaleModifier();
					if (num != 0f)
					{
						scaleSource = base.transform.localScale;
						scaleTarget += scaleTarget * num;
						scaleTarget = ClampLocalScale(scaleTarget);
						scaleStartTime = Time.time;
						float d = scaleTarget.x / scaleSource.x;
						Vector2 cachedCamPos = base.cachedCamPos;
						Vector2 position = cachedCamPos * d;
						MoveToLocal(position, scaleEasingType, scaleEasingDuration);
					}
				}
				float num2 = Time.time - base.lastTouchTime;
				if (moveEasing && num2 < moveEasingDuration && !moveAnim)
				{
					MoveToLocal(estaminatedPos, moveEasingType, moveEasingDuration);
				}
				else
				{
					Clear();
				}
				base.cameraPos = base.cachedCamPos;
				ClampPosition();
			}
			else
			{
				StopMoving();
				Vector2 cachedCamPos2 = base.cachedCamPos;
				Vector2 vector = TouchPosition();
				if (IsTouchingJustStarted())
				{
					lastPosition = vector;
				}
				else
				{
					base.cachedCamPos -= ApplyScreenAspect(vector - lastPosition);
					lastPosition = vector;
				}
				RegisterDelta(ApplyScreenAspect(base.cachedCamPos - cachedCamPos2));
				if (dragDistance > deadDistance)
				{
					base.dragging = true;
					base.cameraPos = base.cachedCamPos;
					ClampPosition();
				}
			}
		}

		private void UpdateScaling()
		{
			if (scaleMode == ScaleMode.Free)
			{
				float num = Time.time - scaleStartTime;
				if (scaleEasing && num < scaleEasingDuration)
				{
					base.transform.localScale = Ease(scaleEasingType, scaleSource, scaleTarget, num / scaleEasingDuration);
				}
				else
				{
					base.transform.localScale = scaleTarget;
				}
			}
		}

		private void UpdateMoving()
		{
			if (moveAnim)
			{
				if (moveAnimStartTime + moveAnimDuration > Time.time)
				{
					float percentage = (Time.time - moveAnimStartTime) / moveAnimDuration;
					cachedCamPos = Ease(moveAnimEaseType, moveAnimStartPosition, moveAnimEndPosition, percentage);
				}
				else
				{
					cachedCamPos = moveAnimEndPosition;
					moveAnim = false;
				}
			}
		}

		public void MoveToLocal(Vector2 position)
		{
			MoveToLocal(position, MadiTween.EaseType.easeInQuad, 0f);
		}

		public void MoveToLocal(Vector2 position, MadiTween.EaseType easeType, float time)
		{
			if (time == 0f)
			{
				base.cameraPos = MadMath.ClosestPoint(dragBounds, position);
				moveAnim = false;
				return;
			}
			moveAnimStartPosition = cachedCamPos;
			moveAnimEndPosition = position;
			moveAnimStartTime = Time.time;
			moveAnimDuration = time;
			moveAnimEaseType = easeType;
			moveAnim = true;
		}

		private void StopMoving()
		{
			moveAnim = false;
		}

		private void ClampPosition()
		{
			Vector2 cameraPos = base.cameraPos;
			MadRootNode madRootNode = MadTransform.FindParent<MadRootNode>(base.transform);
			Vector3 min = dragBounds.min;
			float x = min.x;
			Vector3 min2 = dragBounds.min;
			Vector2 vector = new Vector2(x, min2.y);
			Vector3 max = dragBounds.max;
			float x2 = max.x;
			Vector3 max2 = dragBounds.max;
			Vector2 vector2 = new Vector2(x2, max2.y);
			Vector3 vector3 = base.transform.InverseTransformPoint(madRootNode.ScreenGlobal(0f, 0f));
			Vector3 vector4 = base.transform.InverseTransformPoint(madRootNode.ScreenGlobal(1f, 1f));
			float num = vector3.x - vector.x;
			float num2 = vector4.x - vector2.x;
			float num3 = vector4.y - vector2.y;
			float num4 = vector3.y - vector.y;
			Vector3 localScale = base.transform.localScale;
			float x3 = localScale.x;
			num *= x3;
			num2 *= x3;
			num3 *= x3;
			num4 *= x3;
			Vector3 size = dragBounds.size;
			if (size.x < vector4.x - vector3.x)
			{
				cameraPos.x = (vector2.x + vector.x) / 2f;
			}
			else if (num < 0f)
			{
				cameraPos.x -= num;
			}
			else if (num2 > 0f)
			{
				cameraPos.x -= num2;
			}
			Vector3 size2 = dragBounds.size;
			if (size2.y < vector4.y - vector3.y)
			{
				cameraPos.y = (vector.y + vector2.y) / 2f;
			}
			else if (num4 < 0f)
			{
				cameraPos.y -= num4;
			}
			else if (num3 > 0f)
			{
				cameraPos.y -= num3;
			}
			base.cameraPos = cameraPos;
			switch (scaleMode)
			{
			case ScaleMode.FitToAreaWidth:
			{
				Vector3 position2 = base.transform.position;
				float x5 = position2.x;
				Vector3 center2 = dragBounds.center;
				float x6 = center2.x;
				Vector3 localScale3 = base.transform.localScale;
				float x7 = x5 + x6 * localScale3.x;
				Vector2 cameraPos3 = base.cameraPos;
				base.cameraPos = new Vector2(x7, cameraPos3.y);
				break;
			}
			case ScaleMode.FitToAreaHeight:
			{
				Vector2 cameraPos2 = base.cameraPos;
				float x4 = cameraPos2.x;
				Vector3 position = base.transform.position;
				float y = position.y;
				Vector3 center = dragBounds.center;
				float y2 = center.y;
				Vector3 localScale2 = base.transform.localScale;
				base.cameraPos = new Vector2(x4, y + y2 * localScale2.y);
				break;
			}
			}
		}

		private Vector3 ClampLocalScale(Vector3 scale)
		{
			if (scale.x < scalingMin)
			{
				return new Vector3(scalingMin, scalingMin, scalingMin);
			}
			if (scale.x > scalingMax)
			{
				return new Vector3(scalingMax, scalingMax, scalingMax);
			}
			return scale;
		}

		private float ScaleModifier()
		{
			if (!Application.isEditor)
			{
				if (multiTouches.Count == 2)
				{
					Vector2 position = multiTouches[0].position;
					Vector2 position2 = multiTouches[1].position;
					Vector2 vector = position2 - position;
					float magnitude = vector.magnitude;
					if (lastDoubleTouchDistance != 0f)
					{
						float num = lastDoubleTouchDistance - magnitude;
						lastDoubleTouchDistance = magnitude;
						Vector2 normalized = vector.normalized;
						float num2 = (float)Screen.width * Mathf.Abs(normalized.x) + (float)Screen.height * Mathf.Abs(normalized.y);
						return (0f - num) * 2f / num2;
					}
					lastDoubleTouchDistance = magnitude;
					return 0f;
				}
				lastDoubleTouchDistance = 0f;
				return 0f;
			}
			return UnityEngine.Input.GetAxis("Mouse ScrollWheel");
		}
	}
}
