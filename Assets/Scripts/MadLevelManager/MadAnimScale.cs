using UnityEngine;

namespace MadLevelManager
{
	public class MadAnimScale : MadAnim
	{
		public enum ValueType
		{
			Current,
			Origin,
			Value,
			CurrentMultiply,
			OriginMultiply
		}

		public ValueType scaleFrom;

		public Vector3 scaleFromValue;

		public ValueType scaleTo = ValueType.Value;

		public Vector3 scaleToValue;

		private Vector3 originLocal;

		private Vector3 startLocal;

		private MadiTween.EasingFunction easingFunction;

		protected override void Start()
		{
			easingFunction = GetEasingFunction();
			base.Start();
		}

		public override void UpdateOrigin()
		{
			base.UpdateOrigin();
			originLocal = base.transform.localScale;
		}

		protected override void StartAnim()
		{
			startLocal = base.transform.localScale;
		}

		protected override void Anim(float progress)
		{
			Vector3 from = GetFrom();
			Vector3 to = GetTo();
			float x = easingFunction(from.x, to.x, progress);
			float y = easingFunction(from.y, to.y, progress);
			float z = easingFunction(from.z, to.z, progress);
			Vector3 localScale = new Vector3(x, y, z);
			base.transform.localScale = localScale;
		}

		private Vector3 GetFrom()
		{
			return GetLocalScale(scaleFrom, scaleFromValue);
		}

		private Vector3 GetTo()
		{
			return GetLocalScale(scaleTo, scaleToValue);
		}

		private Vector3 GetLocalScale(ValueType valueType, Vector3 modifier)
		{
			switch (valueType)
			{
			case ValueType.Current:
				return startLocal;
			case ValueType.Origin:
				return originLocal;
			case ValueType.Value:
				return modifier;
			case ValueType.CurrentMultiply:
				return new Vector3(startLocal.x * modifier.x, startLocal.y * modifier.y, startLocal.z * modifier.z);
			case ValueType.OriginMultiply:
				return new Vector3(originLocal.x * modifier.x, originLocal.y * modifier.y, originLocal.z * modifier.z);
			default:
				UnityEngine.Debug.LogError("Unknown option: " + valueType);
				return startLocal;
			}
		}
	}
}
