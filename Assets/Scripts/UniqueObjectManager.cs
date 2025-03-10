using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniqueObjectManager : MonoBehaviour
{
	public GameObject[] sceneObjects;

	public GameObject[] prefabs;

	public static List<GameObject> createdObjects = new List<GameObject>();

	public static UniqueObjectManager mgr;

	public static GameObject[] SceneObjects => mgr.sceneObjects;

	public static GameObject[] Prefabs => mgr.prefabs;

	public static List<GameObject> CreatedObjects => createdObjects;

	public static GameObject InstantiatePrefab(string prefabName, Vector3 position, Quaternion rotation)
	{
		GameObject gameObject = FindPrefabWithName(prefabName);
		if (gameObject == null)
		{
			throw new Exception("Cannot instantiate prefab: No such prefab exists.");
		}
		if (gameObject.GetComponent<ES2UniqueID>() == null)
		{
			throw new Exception("Can't instantiate a prefab which has no UniqueID attached.");
		}
		GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject, position, rotation);
		CreatedObjects.Add(gameObject2);
		return gameObject2;
	}

	public static GameObject InstantiatePrefab(string prefabName)
	{
		return InstantiatePrefab(prefabName, Vector3.zero, Quaternion.identity);
	}

	public static void DestroyObject(GameObject obj)
	{
		if (!CreatedObjects.Remove(obj))
		{
			throw new Exception("Cannot destroy prefab: No such prefab exists.");
		}
		IEnumerator enumerator = obj.transform.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Transform transform = (Transform)enumerator.Current;
				DestroyObject(transform.gameObject);
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
		UnityEngine.Object.Destroy(obj);
	}

	public static GameObject FindPrefabWithName(string prefabName)
	{
		GameObject result = null;
		for (int i = 0; i < Prefabs.Length; i++)
		{
			if (Prefabs[i].name == prefabName)
			{
				result = Prefabs[i];
			}
		}
		return result;
	}

	public void Awake()
	{
		mgr = this;
	}
}
