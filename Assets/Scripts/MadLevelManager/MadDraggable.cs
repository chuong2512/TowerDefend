using System;
using System.Collections.Generic;
using UnityEngine;

namespace MadLevelManager
{
	[ExecuteInEditMode]
	public class MadDraggable : MadNode
	{
		private enum DragMode
		{
			Free,
			DragStop
		}

		public MadiTween.EaseType moveEasingType = MadiTween.EaseType.easeOutQuad;

		public float moveEasingDuration = 0.5f;

		protected Vector2 lastPosition;

		protected Vector2 inertiaForce = Vector2.zero;

		private LinkedList<Vector2> lastDeltas = new LinkedList<Vector2>();

		private int lastDeltasCount = 5;

		protected float dragDistance;

		protected float deadDistance = 50f;

		protected Vector2 cachedCamPos;

		protected Vector2 estaminatedPos;

		private Touch? singleTouch;

		private bool singleTouchEnded;

		protected List<Touch> multiTouches = new List<Touch>();

		protected MadRootNode root;

		[SerializeField]
		private DragMode dragMode = DragMode.DragStop;

		[SerializeField]
		private Rect freeDragArea = new Rect(-200f, -200f, 400f, 400f);

		[SerializeField]
		private bool allowScaling;

		[SerializeField]
		private float scaleMax = 2f;

		[SerializeField]
		private float scaleMin = 0.25f;

		protected float lastDoubleTouchDistance;

		private bool addInteriaForce;

		protected float lastTouchTime
		{
			get;
			set;
		}

		protected Vector2 lastTouchCameraPos
		{
			get;
			set;
		}

		public bool dragging
		{
			get;
			protected set;
		}

		protected Vector2 cameraPos
		{
			get
			{
				return -base.transform.localPosition;
			}
			set
			{
				base.transform.localPosition = -value;
			}
		}

		public virtual Vector2 progress
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		protected virtual void OnEnable()
		{
			Upgrade();
			if (Time.timeScale == 0f)
			{
				UnityEngine.Debug.LogWarning("Level selector may not work when Time.timeScale == 0. Setting it to 1.");
				Time.timeScale = 1f;
			}
		}

		private void Upgrade()
		{
			if (GetType() == typeof(MadDraggable))
			{
				UnityEngine.Debug.Log("Upgrading Draggable object... Please save your scene afterwards.");
				switch (dragMode)
				{
				case DragMode.Free:
				{
					MadFreeDraggable madFreeDraggable = base.gameObject.AddComponent<MadFreeDraggable>();
					madFreeDraggable.dragArea = freeDragArea;
					madFreeDraggable.scaling = allowScaling;
					madFreeDraggable.scalingMax = scaleMax;
					madFreeDraggable.scalingMin = scaleMin;
					break;
				}
				case DragMode.DragStop:
					base.gameObject.AddComponent<MadDragStopDraggable>();
					break;
				}
				UnityEngine.Object.DestroyImmediate(this);
			}
		}

		protected virtual void Start()
		{
			root = MadTransform.FindParent<MadRootNode>(base.transform);
			cachedCamPos = cameraPos;
			lastTouchTime = -1000f;
		}

		protected virtual void Update()
		{
			UpdateTouchClassification();
			if (IsTouchingSingle())
			{
				addInteriaForce = true;
			}
			else
			{
				addInteriaForce = false;
			}
		}

		protected virtual void FixedUpdate()
		{
			if (addInteriaForce)
			{
				ComputeInertiaForce();
			}
		}

		protected virtual void LateUpdate()
		{
			if (IsTouchingSingle())
			{
				lastTouchTime = Time.time;
				lastTouchCameraPos = cameraPos;
			}
		}

		private void UpdateTouchClassification()
		{
			singleTouchEnded = false;
			Touch[] touches = Input.touches;
			for (int i = 0; i < touches.Length; i++)
			{
				Touch touch = touches[i];
				switch (touch.phase)
				{
				case TouchPhase.Began:
				{
					Touch? touch2 = singleTouch;
					if (!touch2.HasValue && multiTouches.Count == 0)
					{
						singleTouch = touch;
						continue;
					}
					Touch? touch3 = singleTouch;
					if (touch3.HasValue)
					{
						multiTouches.Add(singleTouch.Value);
					}
					multiTouches.Add(touch);
					singleTouch = null;
					continue;
				}
				case TouchPhase.Ended:
				case TouchPhase.Canceled:
				{
					Touch? touch4 = singleTouch;
					if (touch4.HasValue)
					{
						singleTouch = null;
					}
					else
					{
						multiTouches.Clear();
					}
					singleTouchEnded = true;
					continue;
				}
				}
				Touch? touch5 = singleTouch;
				if (touch5.HasValue && singleTouch.Value.fingerId == touch.fingerId)
				{
					singleTouch = touch;
					continue;
				}
				for (int j = 0; j < multiTouches.Count; j++)
				{
					if (multiTouches[j].fingerId == touch.fingerId)
					{
						multiTouches[j] = touch;
					}
				}
			}
		}

		protected bool IsTouchingSingle()
		{
			if (!Application.isEditor)
			{
				Touch? touch = singleTouch;
				return touch.HasValue;
			}
			return Input.GetMouseButton(0);
		}

		protected bool IsTouchingJustStarted()
		{
			if (!Application.isEditor)
			{
				if (!IsTouchingSingle())
				{
					return false;
				}
				return singleTouch.Value.phase == TouchPhase.Began;
			}
			return Input.GetMouseButtonDown(0);
		}

		protected bool IsTouchingJustEnded()
		{
			if (!Application.isEditor)
			{
				return singleTouchEnded;
			}
			return Input.GetMouseButtonUp(0);
		}

		protected Vector2 TouchPosition()
		{
			MadDebug.Assert(IsTouchingSingle(), "Not touching anything");
			if (!Application.isEditor)
			{
				return singleTouch.Value.position;
			}
			return UnityEngine.Input.mousePosition;
		}

		protected void RegisterDelta(Vector2 delta)
		{
			lastDeltas.AddLast(delta / Time.deltaTime);
			dragDistance += delta.magnitude;
			if (lastDeltas.Count > lastDeltasCount)
			{
				lastDeltas.RemoveFirst();
			}
		}

		protected Vector2 ApplyScreenAspect(Vector2 delta)
		{
			float num = root.screenWidth / (float)Screen.width;
			float num2 = root.screenHeight / (float)Screen.height;
			delta.x *= num;
			delta.y *= num2;
			return delta;
		}

		protected void Clear()
		{
			lastDeltas.Clear();
			dragDistance = 0f;
			inertiaForce = Vector3.zero;
			estaminatedPos = cameraPos;
		}

		private void ComputeInertiaForce()
		{
			if (lastDeltas.Count > 0)
			{
				inertiaForce = Vector2.zero;
				foreach (Vector2 lastDelta in lastDeltas)
				{
					inertiaForce += lastDelta;
				}
				inertiaForce /= (float)lastDeltas.Count;
			}
			estaminatedPos = cameraPos + inertiaForce * 0.12f;
		}

		protected Vector2 Ease(MadiTween.EaseType type, Vector2 start, Vector2 end, float percentage)
		{
			MadiTween.EasingFunction easingFunction = MadiTween.GetEasingFunction(type);
			float x = easingFunction(start.x, end.x, percentage);
			float y = easingFunction(start.y, end.y, percentage);
			return new Vector2(x, y);
		}

		protected Vector3 Ease(MadiTween.EaseType type, Vector3 start, Vector3 end, float percentage)
		{
			MadiTween.EasingFunction easingFunction = MadiTween.GetEasingFunction(type);
			float x = easingFunction(start.x, end.x, percentage);
			float y = easingFunction(start.y, end.y, percentage);
			float z = easingFunction(start.z, end.z, percentage);
			return new Vector3(x, y, z);
		}
	}
}
