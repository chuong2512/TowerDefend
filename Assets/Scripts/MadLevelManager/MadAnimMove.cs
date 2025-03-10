using UnityEngine;

namespace MadLevelManager
{
	public class MadAnimMove : MadAnim
	{
		public enum ValueType
		{
			Origin,
			Current,
			Local,
			World,
			LocalOriginAdd,
			LocalCurrentAdd,
			WorldOriginAdd,
			WorldCurrentAdd
		}

		public ValueType moveFrom = ValueType.Current;

		public Vector3 moveFromPosition;

		public ValueType moveTo = ValueType.Local;

		public Vector3 moveToPosition;

		private Vector3 originWorld;

		private Vector3 originLocal;

		private Vector3 startWorld;

		private Vector3 startLocal;

		private MadiTween.EasingFunction _easingFunction;

		private MadiTween.EasingFunction easingFunction
		{
			get
			{
				if (_easingFunction == null)
				{
					_easingFunction = GetEasingFunction();
				}
				return _easingFunction;
			}
		}

		protected override void Start()
		{
			base.Start();
		}

		public override void UpdateOrigin()
		{
			base.UpdateOrigin();
			originWorld = base.transform.position;
			originLocal = base.transform.localPosition;
		}

		protected override void StartAnim()
		{
			startWorld = base.transform.position;
			startLocal = base.transform.localPosition;
		}

		protected override void Anim(float progress)
		{
			Vector3 from = GetFrom();
			Vector3 to = GetTo();
			float x = easingFunction(from.x, to.x, progress);
			float y = easingFunction(from.y, to.y, progress);
			float z = easingFunction(from.z, to.z, progress);
			Vector3 localPosition = new Vector3(x, y, z);
			base.transform.localPosition = localPosition;
		}

		private Vector3 GetFrom()
		{
			return GetPosition(moveFrom, moveFromPosition);
		}

		private Vector3 GetTo()
		{
			return GetPosition(moveTo, moveToPosition);
		}

		private Vector3 GetPosition(ValueType valueType, Vector3 modifier)
		{
			switch (valueType)
			{
			case ValueType.Origin:
				return originLocal;
			case ValueType.Current:
				return startLocal;
			case ValueType.Local:
				return modifier;
			case ValueType.World:
				if (base.transform.parent != null)
				{
					return base.transform.parent.InverseTransformPoint(modifier);
				}
				return modifier;
			case ValueType.LocalOriginAdd:
				return originLocal + modifier;
			case ValueType.LocalCurrentAdd:
				return startLocal + modifier;
			case ValueType.WorldOriginAdd:
				if (base.transform.parent != null)
				{
					return base.transform.parent.InverseTransformPoint(originWorld + modifier);
				}
				return originWorld + modifier;
			case ValueType.WorldCurrentAdd:
				if (base.transform.parent != null)
				{
					return base.transform.parent.InverseTransformPoint(startWorld + modifier);
				}
				return startWorld + modifier;
			default:
				UnityEngine.Debug.LogError("Unknown option: " + valueType);
				return startLocal;
			}
		}
	}
}
