using System;
using System.Collections.Generic;
using UnityEngine;

namespace MadLevelManager
{
	public class MadLevelAnimator : MadAnimator
	{
		[Serializable]
		public class Modifier
		{
			public delegate void Executor(MadAnim animation, float modifier);

			public delegate float ValueGetter(MadAnim animation);

			public delegate void ValueSetter(MadAnim animation, float value);

			public enum Operator
			{
				Add,
				Subtract,
				Multiply,
				Divide,
				Modulo
			}

			public enum Value
			{
				LevelIndex,
				GridLevelPageIndex,
				GridRow,
				GridColumn
			}

			public enum ModifierFunc
			{
				Custom,
				Predefined
			}

			public string animationName;

			public ModifierFunc modifierFunction = ModifierFunc.Predefined;

			public Operator baseOperator;

			public Value firstParameter;

			public Operator valueOperator = Operator.Multiply;

			public float secondParameter = 1f;

			[NonSerialized]
			public ModifierFunction customModifierFunction;

			public void Execute(MadLevelIcon icon, ValueGetter getter, ValueSetter setter)
			{
				List<MadAnim> list = MadAnim.FindAnimations(icon.gameObject, animationName);
				for (int i = 0; i < list.Count; i++)
				{
					MadAnim animation = list[i];
					float num = getter(animation);
					switch (modifierFunction)
					{
					case ModifierFunc.Custom:
						setter(animation, customModifierFunction(icon));
						break;
					case ModifierFunc.Predefined:
					{
						float firstParameterValue = GetFirstParameterValue(icon);
						float second = Compute(firstParameterValue, secondParameter, valueOperator);
						float value = Compute(num, second, baseOperator);
						setter(animation, value);
						break;
					}
					default:
						UnityEngine.Debug.LogError("Uknown modifier function:" + modifierFunction);
						setter(animation, num);
						break;
					}
				}
			}

			private float GetFirstParameterValue(MadLevelIcon icon)
			{
				MadLevelAbstractLayout current = MadLevelLayout.current;
				MadLevelGridLayout madLevelGridLayout = current as MadLevelGridLayout;
				switch (firstParameter)
				{
				case Value.LevelIndex:
					return icon.levelIndex;
				case Value.GridLevelPageIndex:
				{
					if (madLevelGridLayout == null)
					{
						return icon.levelIndex;
					}
					int num4 = madLevelGridLayout.gridWidth * madLevelGridLayout.gridHeight;
					return icon.levelIndex % num4;
				}
				case Value.GridRow:
				{
					if (madLevelGridLayout == null)
					{
						return icon.levelIndex;
					}
					int num5 = madLevelGridLayout.gridWidth * madLevelGridLayout.gridHeight;
					int num6 = icon.levelIndex % num5;
					int num7 = num6 / madLevelGridLayout.gridWidth;
					return num7;
				}
				case Value.GridColumn:
				{
					if (madLevelGridLayout == null)
					{
						return icon.levelIndex;
					}
					int num = madLevelGridLayout.gridWidth * madLevelGridLayout.gridHeight;
					int num2 = icon.levelIndex % num;
					int num3 = num2 % madLevelGridLayout.gridWidth;
					return num3;
				}
				default:
					UnityEngine.Debug.LogError("Unknown value: " + firstParameter);
					return 0f;
				}
			}

			private float Compute(float first, float second, Operator op)
			{
				switch (op)
				{
				case Operator.Add:
					return first + second;
				case Operator.Subtract:
					return first - second;
				case Operator.Multiply:
					return first * second;
				case Operator.Divide:
					return first / second;
				case Operator.Modulo:
					return first % second;
				default:
					UnityEngine.Debug.LogError("Unknown operator: " + op);
					return first;
				}
			}
		}

		public delegate float ModifierFunction(MadLevelIcon icon);

		public enum ApplyMethod
		{
			DoNotChange,
			Add,
			Multiply,
			Set
		}

		public Action onLevelLocked = new Action();

		public Action onLevelUnlocked = new Action();

		public List<Modifier> delayModifiers = new List<Modifier>();

		public List<Modifier> offsetModifiers = new List<Modifier>();

		public bool startupScaleForce;

		public ApplyMethod startupPositionApplyMethod;

		public Vector3 startupPosition = new Vector3(0f, 0f, 0f);

		public ApplyMethod startupRotationApplyMethod;

		public Vector3 startupRotation = new Vector3(0f, 0f, 0f);

		public ApplyMethod startupScaleApplyMethod;

		public Vector3 startupScale = new Vector3(1f, 1f, 1f);

		private bool modifiersApplied;

		private void OnEnable()
		{
			if (!modifiersApplied)
			{
				ApplyModifiers();
				modifiersApplied = true;
			}
		}

		private void ApplyModifiers()
		{
			MadLevelIcon component = GetComponent<MadLevelIcon>();
			if (!(component == null))
			{
				for (int i = 0; i < delayModifiers.Count; i++)
				{
					Modifier modifier = delayModifiers[i];
					modifier.Execute(component, (MadAnim a) => a.delay, delegate(MadAnim a, float v)
					{
						a.delay = v;
					});
				}
				for (int j = 0; j < offsetModifiers.Count; j++)
				{
					Modifier modifier2 = offsetModifiers[j];
					modifier2.Execute(component, (MadAnim a) => a.offset, delegate(MadAnim a, float v)
					{
						a.offset = v;
					});
				}
			}
		}

		protected override void Start()
		{
			base.Start();
			UpdateAnimOrigins();
			ApplyStartupPosition();
			ApplyStartupRotation();
			ApplyStartupScale();
			MadLevelIcon component = GetComponent<MadLevelIcon>();
			if (component != null && !component.isTemplate)
			{
				if (component.locked)
				{
					onLevelLocked.Execute(base.gameObject);
				}
				else
				{
					onLevelUnlocked.Execute(base.gameObject);
				}
			}
		}

		private void Update()
		{
		}

		private void UpdateAnimOrigins()
		{
			MadAnim[] components = GetComponents<MadAnim>();
			MadAnim[] array = components;
			foreach (MadAnim madAnim in array)
			{
				madAnim.UpdateOrigin();
			}
		}

		private void ApplyStartupPosition()
		{
			Vector3 localPosition = ApplyValue(startupPositionApplyMethod, base.transform.localPosition, startupPosition);
			base.transform.localPosition = localPosition;
		}

		private void ApplyStartupRotation()
		{
			Vector3 euler = ApplyValue(startupRotationApplyMethod, base.transform.localRotation.eulerAngles, startupRotation);
			base.transform.localRotation = Quaternion.Euler(euler);
		}

		private void ApplyStartupScale()
		{
			Vector3 localScale = ApplyValue(startupScaleApplyMethod, base.transform.localScale, startupScale);
			base.transform.localScale = localScale;
		}

		private Vector3 ApplyValue(ApplyMethod method, Vector3 originalValue, Vector3 applyValue)
		{
			switch (method)
			{
			case ApplyMethod.DoNotChange:
				return originalValue;
			case ApplyMethod.Add:
				return originalValue + applyValue;
			case ApplyMethod.Multiply:
				return new Vector3(originalValue.x * applyValue.x, originalValue.y * applyValue.y, originalValue.z * applyValue.z);
			case ApplyMethod.Set:
				return startupScale;
			default:
				UnityEngine.Debug.LogError("Unknown apply method: " + method);
				return originalValue;
			}
		}
	}
}
