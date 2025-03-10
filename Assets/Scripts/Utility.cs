using System;
using System.Collections;
using UnityEngine;

public class Utility : MonoBehaviour
{
	public static Vector3 GetWorldScale(Transform transform)
	{
		Vector3 vector = transform.localScale;
		Transform parent = transform.parent;
		while (parent != null)
		{
			vector = Vector3.Scale(vector, parent.localScale);
			parent = parent.parent;
		}
		return vector;
	}

	public static void DestroyColliderRecursively(Transform root)
	{
		IEnumerator enumerator = root.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Transform transform = (Transform)enumerator.Current;
				if (transform.GetComponent<Collider>() != null)
				{
					UnityEngine.Object.Destroy(transform.GetComponent<Collider>());
				}
				DestroyColliderRecursively(transform);
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = (enumerator as IDisposable)) != null)
			{
				disposable.Dispose();
			}
		}
	}

	public static void DisableColliderRecursively(Transform root)
	{
		IEnumerator enumerator = root.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Transform transform = (Transform)enumerator.Current;
				if (transform.gameObject.GetComponent<Collider>() != null)
				{
					transform.gameObject.GetComponent<Collider>().enabled = false;
				}
				DisableColliderRecursively(transform);
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = (enumerator as IDisposable)) != null)
			{
				disposable.Dispose();
			}
		}
	}

	public static void SetMatRecursively(Transform root, string materialName)
	{
		IEnumerator enumerator = root.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Transform transform = (Transform)enumerator.Current;
				if (transform.GetComponent<Renderer>() != null)
				{
					Material[] materials = transform.GetComponent<Renderer>().materials;
					foreach (Material material in materials)
					{
						material.shader = Shader.Find(materialName);
					}
				}
				SetMatRecursively(transform, materialName);
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = (enumerator as IDisposable)) != null)
			{
				disposable.Dispose();
			}
		}
	}

	public static void SetMatColorRecursively(Transform root, string colorName, Color color)
	{
		IEnumerator enumerator = root.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Transform transform = (Transform)enumerator.Current;
				if (transform.GetComponent<Renderer>() != null)
				{
					Material[] materials = transform.GetComponent<Renderer>().materials;
					foreach (Material material in materials)
					{
						material.SetColor(colorName, color);
					}
				}
				SetMatColorRecursively(transform, colorName, color);
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = (enumerator as IDisposable)) != null)
			{
				disposable.Dispose();
			}
		}
	}

	public static void DisableAllChildRendererRecursively(Transform root)
	{
		IEnumerator enumerator = root.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Transform transform = (Transform)enumerator.Current;
				Renderer component = transform.GetComponent<Renderer>();
				if (component != null)
				{
					component.enabled = false;
				}
				DisableAllChildRendererRecursively(transform);
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = (enumerator as IDisposable)) != null)
			{
				disposable.Dispose();
			}
		}
	}

	public static void EnbleAllChildRendererRecursively(Transform root)
	{
		IEnumerator enumerator = root.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Transform transform = (Transform)enumerator.Current;
				Renderer component = transform.GetComponent<Renderer>();
				if (component != null)
				{
					component.enabled = true;
				}
				EnbleAllChildRendererRecursively(transform);
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = (enumerator as IDisposable)) != null)
			{
				disposable.Dispose();
			}
		}
	}
}
