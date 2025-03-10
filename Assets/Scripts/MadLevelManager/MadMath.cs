using System.Linq;
using UnityEngine;

namespace MadLevelManager
{
	public class MadMath
	{
		public static readonly Vector3 InfinityVector3 = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);

		public static Vector2 SmoothDampVector2(Vector2 current, Vector2 target, ref Vector2 currentVelocity, float time)
		{
			float currentVelocity2 = currentVelocity.x;
			float currentVelocity3 = currentVelocity.y;
			float x = Mathf.SmoothDamp(current.x, target.x, ref currentVelocity2, time);
			float y = Mathf.SmoothDamp(current.y, target.y, ref currentVelocity3, time);
			currentVelocity.x = currentVelocity2;
			currentVelocity.y = currentVelocity3;
			return new Vector2(x, y);
		}

		public static Vector2 ClosestPoint(Rect rect, Vector2 point)
		{
			if (rect.Contains(point))
			{
				return point;
			}
			float num = point.x;
			float num2 = point.y;
			if (num < rect.xMin)
			{
				num = rect.xMin;
			}
			else if (num > rect.xMax)
			{
				num = rect.xMax;
			}
			if (num2 < rect.yMin)
			{
				num2 = rect.yMin;
			}
			else if (num2 > rect.yMax)
			{
				num2 = rect.yMax;
			}
			return new Vector2(num, num2);
		}

		public static Vector2 ClosestPoint(Bounds bounds, Vector2 point)
		{
			if (bounds.Contains(point))
			{
				return point;
			}
			float x = point.x;
			float y = point.y;
			float num = x;
			Vector3 min = bounds.min;
			if (num < min.x)
			{
				Vector3 min2 = bounds.min;
				x = min2.x;
			}
			else
			{
				float num2 = x;
				Vector3 max = bounds.max;
				if (num2 > max.x)
				{
					Vector3 max2 = bounds.max;
					x = max2.x;
				}
			}
			float num3 = y;
			Vector3 min3 = bounds.min;
			if (num3 < min3.y)
			{
				Vector3 min4 = bounds.min;
				y = min4.y;
			}
			else
			{
				float num4 = y;
				Vector3 max3 = bounds.max;
				if (num4 > max3.y)
				{
					Vector3 max4 = bounds.max;
					y = max4.y;
				}
			}
			return new Vector2(x, y);
		}

		public static Vector3 Round(Vector3 v)
		{
			return new Vector3(Mathf.Round(v.x), Mathf.Round(v.y), Mathf.Round(v.z));
		}

		public static Rect Expand(Rect a, Rect b)
		{
			return new Rect(Mathf.Min(a.x, b.y), Mathf.Min(a.y, b.y), Mathf.Max(a.xMax, b.xMax) - Mathf.Min(a.xMin, b.xMin), Mathf.Max(a.yMax, b.yMax) - Mathf.Min(a.yMin, b.yMin));
		}

		public static Rect Translate(Rect r, Vector2 delta)
		{
			return new Rect(r.x + delta.x, r.y + delta.y, r.width, r.height);
		}

		public static Rect Scale(Rect r, Vector2 scale)
		{
			return new Rect(r.x * scale.x, r.y * scale.y, r.width * scale.x, r.height * scale.y);
		}

		public static bool Overlaps(Rect a, Rect b)
		{
			return a.xMin < b.xMax && a.xMax > b.xMin && a.yMin < b.yMax && a.yMax > b.yMin;
		}

		public static bool Approximately(Vector3 a, Vector3 b)
		{
			return Mathf.Approximately(a.x, b.x) && Mathf.Approximately(a.y, b.y) && Mathf.Approximately(a.z, b.z);
		}

		public static bool Approximately(Vector4 a, Vector4 b)
		{
			return Mathf.Approximately(a.x, b.x) && Mathf.Approximately(a.y, b.y) && Mathf.Approximately(a.z, b.z) && Mathf.Approximately(a.w, b.w);
		}

		public static bool Approximately(Quaternion a, Quaternion b)
		{
			return Mathf.Approximately(a.x, b.x) && Mathf.Approximately(a.y, b.y) && Mathf.Approximately(a.z, b.z) && Mathf.Approximately(a.w, b.w);
		}

		public static string ToRoman(int number)
		{
			string[][] array = new string[4][]
			{
				new string[10]
				{
					string.Empty,
					"I",
					"II",
					"III",
					"IV",
					"V",
					"VI",
					"VII",
					"VIII",
					"IX"
				},
				new string[10]
				{
					string.Empty,
					"X",
					"XX",
					"XXX",
					"XL",
					"L",
					"LX",
					"LXX",
					"LXXX",
					"XC"
				},
				new string[10]
				{
					string.Empty,
					"C",
					"CC",
					"CCC",
					"CD",
					"D",
					"DC",
					"DCC",
					"DCCC",
					"CM"
				},
				new string[4]
				{
					string.Empty,
					"M",
					"MM",
					"MMM"
				}
			};
			char[] array2 = number.ToString().Reverse().ToArray();
			int num = array2.Length;
			string text = string.Empty;
			int num2 = num;
			while (num2-- > 0)
			{
				text += array[num2][int.Parse(array2[num2].ToString())];
			}
			return text;
		}
	}
}
