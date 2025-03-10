using System;
using System.Threading;
using UnityEngine;

public class PreviewScreenUtil : MonoBehaviour
{
	private static PreviewScreenUtil _instance;

	private int W;

	private int H;

	public static PreviewScreenUtil instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new GameObject("ScreenUtil").AddComponent<PreviewScreenUtil>();
				UnityEngine.Object.DontDestroyOnLoad(_instance.gameObject);
			}
			return _instance;
		}
	}

	public event Action ActionScreenResized;

	public PreviewScreenUtil()
	{
		this.ActionScreenResized = delegate
		{
		};
		
	}

	public static bool isInScreenRect(Rect rect, Vector2 point)
	{
		point.y = (float)Screen.height - point.y;
		if (rect.Contains(point))
		{
			return true;
		}
		return false;
	}

	public static Rect getObjectBounds(GameObject obj)
	{
		if (obj.GetComponent<Renderer>() != null)
		{
			return getRendererBounds(obj.GetComponent<Renderer>());
		}
		return default(Rect);
	}

	public static Rect getRendererBounds(Renderer renderer)
	{
		Camera main = Camera.main;
		Vector3 min = renderer.bounds.min;
		float x = min.x;
		Vector3 max = renderer.bounds.max;
		Vector3 vector = main.WorldToScreenPoint(new Vector3(x, max.y, 0f));
		Camera main2 = Camera.main;
		Vector3 max2 = renderer.bounds.max;
		float x2 = max2.x;
		Vector3 min2 = renderer.bounds.min;
		Vector3 vector2 = main2.WorldToScreenPoint(new Vector3(x2, min2.y, 0f));
		return new Rect(vector.x, (float)Screen.height - vector.y, vector2.x - vector.x, vector.y - vector2.y);
	}

	private void Awake()
	{
		W = Screen.width;
		H = Screen.height;
	}

	private void FixedUpdate()
	{
		if (W != Screen.width || H != Screen.height)
		{
			W = Screen.width;
			H = Screen.height;
			this.ActionScreenResized();
		}
	}
}
