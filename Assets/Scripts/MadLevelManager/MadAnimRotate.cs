using UnityEngine;

namespace MadLevelManager
{
	public class MadAnimRotate : MadAnim
	{
		public enum ValueType
		{
			Current,
			Origin,
			Value,
			CurrentAdd,
			OriginAdd
		}

		public ValueType rotateFrom;

		public Vector3 rotateFromValue;

		public ValueType rotateTo = ValueType.Value;

		public Vector3 rotateToValue;

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
			originLocal = base.transform.localRotation.eulerAngles;
		}

		protected override void StartAnim()
		{
			startLocal = base.transform.localRotation.eulerAngles;
		}

		protected override void Anim(float progress)
		{
			Vector3 from = GetFrom();
			Vector3 to = GetTo();
			float num = Mathf.DeltaAngle(from.x, to.x);
			float num2 = Mathf.DeltaAngle(from.y, to.y);
			float num3 = Mathf.DeltaAngle(from.z, to.z);
			float x = easingFunction(from.x, from.x + num, progress);
			float y = easingFunction(from.y, from.y + num2, progress);
			float z = easingFunction(from.z, from.z + num3, progress);
			Vector3 euler = new Vector3(x, y, z);
			base.transform.localRotation = Quaternion.Euler(euler);
		}

		private Vector3 GetFrom()
		{
			return GetLocalRotation(rotateFrom, rotateFromValue);
		}

		private Vector3 GetTo()
		{
			return GetLocalRotation(rotateTo, rotateToValue);
		}

		private Vector3 GetLocalRotation(ValueType valueType, Vector3 modifier)
		{
			switch (valueType)
			{
			case ValueType.Current:
				return startLocal;
			case ValueType.Origin:
				return originLocal;
			case ValueType.Value:
				return modifier;
			case ValueType.CurrentAdd:
				return new Vector3(startLocal.x + modifier.x, startLocal.y + modifier.y, startLocal.z + modifier.z);
			case ValueType.OriginAdd:
				return new Vector3(originLocal.x + modifier.x, originLocal.y + modifier.y, originLocal.z + modifier.z);
			default:
				UnityEngine.Debug.LogError("Unknown option: " + valueType);
				return startLocal;
			}
		}
	}
}
