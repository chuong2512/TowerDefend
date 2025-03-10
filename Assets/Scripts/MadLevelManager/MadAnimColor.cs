using UnityEngine;

namespace MadLevelManager
{
	public class MadAnimColor : MadAnim
	{
		public enum ValueType
		{
			Origin,
			Current,
			Value
		}

		public ValueType colorFrom = ValueType.Current;

		public Color colorFromValue = Color.white;

		public ValueType colorTo = ValueType.Value;

		public Color colorToValue = Color.white;

		private Color origin;

		private Color start;

		private MadiTween.EasingFunction easingFunction;

		private MadSprite sprite;

		protected override void Start()
		{
			sprite = GetComponent<MadSprite>();
			if (sprite == null)
			{
				UnityEngine.Debug.Log("Anim Color component requires MadSprite component!", this);
				return;
			}
			origin = sprite.tint;
			easingFunction = GetEasingFunction();
			base.Start();
		}

		protected override void StartAnim()
		{
			start = sprite.tint;
		}

		protected override void Anim(float progress)
		{
			Color from = GetFrom();
			Color to = GetTo();
			float r = easingFunction(from.r, to.r, progress);
			float g = easingFunction(from.g, to.g, progress);
			float b = easingFunction(from.b, to.b, progress);
			float a = easingFunction(from.a, to.a, progress);
			Color tint = new Color(r, g, b, a);
			sprite.tint = tint;
		}

		private Color GetFrom()
		{
			return GetColor(colorFrom, colorFromValue);
		}

		private Color GetTo()
		{
			return GetColor(colorTo, colorToValue);
		}

		private Color GetColor(ValueType valueType, Color modifier)
		{
			switch (valueType)
			{
			case ValueType.Origin:
				return origin;
			case ValueType.Current:
				return start;
			case ValueType.Value:
				return modifier;
			default:
				UnityEngine.Debug.LogError("Unknown option: " + valueType);
				return start;
			}
		}
	}
}
