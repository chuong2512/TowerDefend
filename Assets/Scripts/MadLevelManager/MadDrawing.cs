using System;
using UnityEngine;

namespace MadLevelManager
{
	public static class MadDrawing
	{
		private static Texture2D _aaLineTex;

		private static Texture2D _lineTex;

		private static Texture2D adLineTex
		{
			get
			{
				if (!_aaLineTex)
				{
					_aaLineTex = new Texture2D(1, 3, TextureFormat.ARGB32, mipChain: true);
					_aaLineTex.SetPixel(0, 0, new Color(1f, 1f, 1f, 0f));
					_aaLineTex.SetPixel(0, 1, Color.white);
					_aaLineTex.SetPixel(0, 2, new Color(1f, 1f, 1f, 0f));
					_aaLineTex.Apply();
				}
				return _aaLineTex;
			}
		}

		private static Texture2D lineTex
		{
			get
			{
				if (!_lineTex)
				{
					_lineTex = new Texture2D(1, 1, TextureFormat.ARGB32, mipChain: true);
					_lineTex.SetPixel(0, 1, Color.white);
					_lineTex.Apply();
				}
				return _lineTex;
			}
		}

		private static void DrawLineMac(Vector2 pointA, Vector2 pointB, Color color, float width, bool antiAlias)
		{
			Color color2 = GUI.color;
			Matrix4x4 matrix = GUI.matrix;
			float num = width;
			if (antiAlias)
			{
				width *= 3f;
			}
			float num2 = Vector3.Angle(pointB - pointA, Vector2.right) * (float)((pointA.y <= pointB.y) ? 1 : (-1));
			float magnitude = (pointB - pointA).magnitude;
			if (magnitude > 0.01f)
			{
				Vector3 vector = new Vector3(pointA.x, pointA.y, 0f);
				Vector3 b = new Vector3((pointB.x - pointA.x) * 0.5f, (pointB.y - pointA.y) * 0.5f, 0f);
				Vector3 zero = Vector3.zero;
				zero = (antiAlias ? new Vector3((0f - num) * 1.5f * Mathf.Sin(num2 * ((float)Math.PI / 180f)), num * 1.5f * Mathf.Cos(num2 * ((float)Math.PI / 180f))) : new Vector3((0f - num) * 0.5f * Mathf.Sin(num2 * ((float)Math.PI / 180f)), num * 0.5f * Mathf.Cos(num2 * ((float)Math.PI / 180f))));
				GUI.color = color;
				GUI.matrix = translationMatrix(vector) * GUI.matrix;
				GUIUtility.ScaleAroundPivot(new Vector2(magnitude, width), new Vector2(-0.5f, 0f));
				GUI.matrix = translationMatrix(-vector) * GUI.matrix;
				GUIUtility.RotateAroundPivot(num2, Vector2.zero);
				GUI.matrix = translationMatrix(vector - zero - b) * GUI.matrix;
				GUI.DrawTexture(new Rect(0f, 0f, 1f, 1f), (!antiAlias) ? lineTex : adLineTex);
			}
			GUI.matrix = matrix;
			GUI.color = color2;
		}

		private static void DrawLineWindows(Vector2 pointA, Vector2 pointB, Color color, float width, bool antiAlias)
		{
			Color color2 = GUI.color;
			Matrix4x4 matrix = GUI.matrix;
			if (antiAlias)
			{
				width *= 3f;
			}
			float num = Vector3.Angle(pointB - pointA, Vector2.right) * (float)((pointA.y <= pointB.y) ? 1 : (-1));
			float magnitude = (pointB - pointA).magnitude;
			Vector3 vector = new Vector3(pointA.x, pointA.y, 0f);
			GUI.color = color;
			GUI.matrix = translationMatrix(vector) * GUI.matrix;
			GUIUtility.ScaleAroundPivot(new Vector2(magnitude, width), new Vector2(-0.5f, 0f));
			GUI.matrix = translationMatrix(-vector) * GUI.matrix;
			GUIUtility.RotateAroundPivot(num, new Vector2(0f, 0f));
			GUI.matrix = translationMatrix(vector + new Vector3(width / 2f, (0f - magnitude) / 2f) * Mathf.Sin(num * ((float)Math.PI / 180f))) * GUI.matrix;
			GUI.DrawTexture(new Rect(0f, 0f, 1f, 1f), antiAlias ? adLineTex : lineTex);
			GUI.matrix = matrix;
			GUI.color = color2;
		}

		public static void DrawLine(Vector2 pointA, Vector2 pointB, Color color, float width, bool antiAlias)
		{
			if (Application.platform == RuntimePlatform.WindowsEditor)
			{
				DrawLineWindows(pointA, pointB, color, width, antiAlias);
			}
			else if (Application.platform == RuntimePlatform.OSXEditor)
			{
				DrawLineMac(pointA, pointB, color, width, antiAlias);
			}
		}

		public static void BezierLine(Vector2 start, Vector2 startTangent, Vector2 end, Vector2 endTangent, Color color, float width, bool antiAlias, int segments)
		{
			Vector2 pointA = cubeBezier(start, startTangent, end, endTangent, 0f);
			for (int i = 1; i <= segments; i++)
			{
				Vector2 vector = cubeBezier(start, startTangent, end, endTangent, (float)i / (float)segments);
				DrawLine(pointA, vector, color, width, antiAlias);
				pointA = vector;
			}
		}

		private static Vector2 cubeBezier(Vector2 s, Vector2 st, Vector2 e, Vector2 et, float t)
		{
			float d = 1f - t;
			return s * d * d * d + 3f * st * d * d * t + 3f * et * d * t * t + e * t * t * t;
		}

		private static Matrix4x4 translationMatrix(Vector3 v)
		{
			return Matrix4x4.TRS(v, Quaternion.identity, Vector3.one);
		}
	}
}
