using System;
using System.Collections.Generic;
using UnityEngine;

namespace MadLevelManager
{
	public class MadDragStopDraggable : MadDraggable
	{
		public delegate void DragStopCallback(int index);

		public enum Direction
		{
			Horizontal,
			Vertical
		}

		public DragStopCallback dragStopCallback;

		private List<Vector2> dragStops = new List<Vector2>();

		private int forcedDragStopIndex = -1;

		public Direction direction;

		public bool directionInvert;

		private float avarageDistance;

		public float swipeVirtualDistanceModifier = 2f;

		public bool limitSwipeToSinglePage;

		public float switchAfterDistance = 0.5f;

		public int dragStopCount => dragStops.Count;

		public int dragStopCurrentIndex
		{
			get;
			private set;
		}

		public override Vector2 progress
		{
			get
			{
				if (dragStopCount == 0)
				{
					return Vector2.zero;
				}
				if (dragStopCount == 1)
				{
					return new Vector2(dragStopCurrentIndex, 0f);
				}
				return new Vector2((float)dragStopCurrentIndex / (float)(dragStopCount - 1), 0f);
			}
		}

		public bool animating
		{
			get;
			private set;
		}

		protected override void Update()
		{
			if (!Application.isPlaying)
			{
				return;
			}
			base.Update();
			if (dragStops.Count == 0)
			{
				return;
			}
			if (!IsTouchingSingle())
			{
				if (base.dragging)
				{
					int num = IntendedDragStopIndex();
					if (num != dragStopCurrentIndex)
					{
						dragStopCurrentIndex = num;
						dragStopCallback(num);
					}
					base.dragging = false;
				}
				if (forcedDragStopIndex != -1)
				{
					dragStopCurrentIndex = forcedDragStopIndex;
					dragStopCallback(dragStopCurrentIndex);
					forcedDragStopIndex = -1;
				}
				ReturnToDragStop();
				base.cameraPos = base.cachedCamPos;
				Clear();
				return;
			}
			forcedDragStopIndex = -1;
			int index = IntendedDragStopIndex();
			Vector2 cachedCamPos = base.cachedCamPos;
			Vector2 vector = TouchPosition();
			if (IsTouchingJustStarted())
			{
				lastPosition = vector;
			}
			else
			{
				Vector2 a = vector - lastPosition;
				float num2 = ComputeDrag();
				a *= 1f - num2;
				a = ApplyScreenAspect(a);
				base.cachedCamPos -= a;
				lastPosition = vector;
			}
			int num3 = ClosestNeighborTo(index);
			if (num3 != -1)
			{
				Vector2 vector2 = dragStops[num3] - dragStops[index];
				Vector2 lhs = base.cachedCamPos - dragStops[index];
				float d = Vector2.Dot(lhs, vector2.normalized);
				Vector2 b = vector2.normalized * d;
				base.cachedCamPos = dragStops[index] + b;
			}
			else
			{
				base.cachedCamPos = dragStops[index];
			}
			RegisterDelta(ApplyScreenAspect(base.cachedCamPos - cachedCamPos));
			if (dragDistance > deadDistance)
			{
				base.dragging = true;
				base.cameraPos = base.cachedCamPos;
			}
		}

		private float ComputeDrag()
		{
			int num = ClosestDragStopIndex();
			Vector2 vector = dragStops[num];
			if (num == 0)
			{
				Vector2 cameraPos = base.cameraPos;
				if (cameraPos.x < vector.x)
				{
					goto IL_005d;
				}
			}
			if (num == dragStopCount - 1)
			{
				Vector2 cameraPos2 = base.cameraPos;
				if (cameraPos2.x > vector.x)
				{
					goto IL_005d;
				}
			}
			return 0f;
			IL_005d:
			Vector2 cameraPos3 = base.cameraPos;
			float num2 = Mathf.Abs(cameraPos3.x - vector.x);
			return num2 / (float)Screen.width * 2f;
		}

		public void ClearDragStops()
		{
			dragStops.Clear();
			dragStopCurrentIndex = 0;
		}

		public int AddDragStop(float x, float y)
		{
			dragStops.Add(new Vector2(x, y));
			ComputeAvarageDistance();
			return dragStops.Count - 1;
		}

		private void ComputeAvarageDistance()
		{
			if (dragStops.Count >= 2)
			{
				float num = 0f;
				for (int i = 1; i < dragStops.Count; i++)
				{
					num += Vector2.Distance(dragStops[i - 1], dragStops[i]);
				}
				avarageDistance = num / (float)(dragStops.Count - 1);
			}
		}

		public void MoveTo(int dragStop, bool now)
		{
			forcedDragStopIndex = dragStop;
			if (!now)
			{
				base.lastTouchTime = Time.time;
				base.lastTouchCameraPos = base.cameraPos;
			}
		}

		private void ReturnToDragStop()
		{
			Vector3 v = dragStops[dragStopCurrentIndex];
			float num = Time.time - base.lastTouchTime;
			if (num < moveEasingDuration && base.cameraPos != (Vector2)v)
			{
				animating = true;
				cachedCamPos = Ease(moveEasingType, base.lastTouchCameraPos, (Vector2)v, num / moveEasingDuration);
			}
			else
			{
				animating = false;
				cachedCamPos = v;
			}
		}

		private int IntendedDragStopIndex()
		{
			if (dragStops.Count == 0)
			{
				return -1;
			}
			if (forcedDragStopIndex != -1)
			{
				return forcedDragStopIndex;
			}
			int dragStopCurrentIndex = this.dragStopCurrentIndex;
			float inertiaForce = GetInertiaForce();
			float num = avarageDistance * swipeVirtualDistanceModifier;
			int value = dragStopCurrentIndex;
			if (inertiaForce > num)
			{
				value = Mathf.Clamp(dragStopCurrentIndex + IndexChange(inertiaForce, num), 0, dragStops.Count);
			}
			else if (inertiaForce < 0f - num)
			{
				value = Mathf.Clamp(dragStopCurrentIndex - IndexChange(inertiaForce, num), 0, dragStops.Count);
			}
			value = Mathf.Clamp(value, 0, dragStops.Count - 1);
			if (limitSwipeToSinglePage)
			{
				value = Mathf.Clamp(value, dragStopCurrentIndex - 1, dragStopCurrentIndex + 1);
			}
			if (value == dragStopCurrentIndex)
			{
				Vector2 a = dragStops[dragStopCurrentIndex];
				if (Vector2.Distance(a, base.cameraPos) > avarageDistance * switchAfterDistance)
				{
					value = ClosestDragStopIndex(dragStopCurrentIndex);
					if (Vector2.Distance(dragStops[value], base.cameraPos) > avarageDistance)
					{
						value = dragStopCurrentIndex;
					}
				}
			}
			return value;
		}

		private int IndexChange(float force, float thredshold)
		{
			int num = (int)Mathf.Abs(force / thredshold);
			int num2 = 1;
			int num3 = 1;
			int num4 = 1;
			while (num > num2)
			{
				num2 += ++num3;
				num4++;
			}
			return num4;
		}

		private float GetInertiaForce()
		{
			switch (direction)
			{
			case Direction.Horizontal:
				return (!directionInvert) ? inertiaForce.x : (0f - inertiaForce.x);
			case Direction.Vertical:
				return (!directionInvert) ? (0f - inertiaForce.y) : inertiaForce.y;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		private int ClosestDragStopIndex(int skip = -1)
		{
			if (forcedDragStopIndex != -1 && forcedDragStopIndex != skip)
			{
				return forcedDragStopIndex;
			}
			Vector3 v = cachedCamPos;
			float num = float.PositiveInfinity;
			int result = -1;
			for (int i = 0; i < dragStops.Count; i++)
			{
				Vector2 b = dragStops[i];
				float num2 = Vector2.Distance(v, b);
				if (num2 < num && i != skip)
				{
					num = num2;
					result = i;
				}
			}
			return result;
		}

		private int ClosestNeighborTo(int index)
		{
			int result = -1;
			float num = float.PositiveInfinity;
			Vector2 a = dragStops[index];
			if (index - 1 >= 0)
			{
				Vector2 b = dragStops[index - 1];
				float num2 = Vector2.Distance(a, b);
				num = num2;
				result = index - 1;
			}
			if (index + 1 < dragStops.Count)
			{
				Vector2 b2 = dragStops[index + 1];
				float num3 = Vector2.Distance(a, b2);
				if (num3 < num)
				{
					result = index + 1;
				}
			}
			return result;
		}

		public float GetProgress()
		{
			if (!animating && !base.dragging)
			{
				return dragStopCurrentIndex;
			}
			int num = ClosestDragStopIndex();
			int num2 = ClosestDragStopIndex(num);
			if (num == -1 || num2 == -1)
			{
				return dragStopCurrentIndex;
			}
			int num3 = (num >= num2) ? num2 : num;
			int index = (num >= num2) ? num : num2;
			Vector2 b = dragStops[num3];
			Vector2 a = dragStops[index];
			return (float)num3 + Mathf.Abs((base.cameraPos - b).magnitude / (a - b).magnitude);
		}
	}
}
