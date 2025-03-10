using System;
using System.Threading;
using UnityEngine;

namespace TDTK
{
	public class TextOverlay
	{
		public delegate void TextOverlayHandler(TextOverlay textO);

		public Vector3 pos;

		public string msg;

		public float scale;

		public Color color;

		public bool useColor;

		public static event TextOverlayHandler onTextOverlayE;

		public TextOverlay(Vector3 p, string m, float s = 1f)
		{
			pos = p + GetScatterPos();
			msg = m;
			scale = s;
			if (TextOverlay.onTextOverlayE != null)
			{
				TextOverlay.onTextOverlayE(this);
			}
		}

		public TextOverlay(Vector3 p, string m, Color col, float s = 1f)
		{
			pos = p + GetScatterPos();
			msg = m;
			color = col;
			scale = s;
			useColor = true;
			if (TextOverlay.onTextOverlayE != null)
			{
				TextOverlay.onTextOverlayE(this);
			}
		}

		public Vector3 GetScatterPos()
		{
			float num = 0.75f;
			return new Vector3(UnityEngine.Random.Range(0f - num, num), UnityEngine.Random.Range(0f - num, num), UnityEngine.Random.Range(0f - num, num));
		}
	}
}
