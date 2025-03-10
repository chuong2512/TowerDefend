using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Pool
{
	public GameObject prefab;

	public List<GameObject> inactiveList = new List<GameObject>();

	public List<GameObject> activeList = new List<GameObject>();

	public int cap = 1000;

	public GameObject Spawn(Vector3 pos, Quaternion rot)
	{
		GameObject gameObject = null;
		if (inactiveList.Count == 0)
		{
			gameObject = UnityEngine.Object.Instantiate(prefab, pos, rot);
		}
		else
		{
			gameObject = inactiveList[0];
			gameObject.transform.parent = null;
			gameObject.transform.position = pos;
			gameObject.transform.rotation = rot;
			gameObject.SetActive(value: true);
			inactiveList.RemoveAt(0);
		}
		activeList.Add(gameObject);
		return gameObject;
	}

	public bool Unspawn(GameObject obj)
	{
		if (activeList.Contains(obj))
		{
			obj.SetActive(value: false);
			obj.transform.parent = ObjectPoolManager.GetOPMTransform();
			activeList.Remove(obj);
			inactiveList.Add(obj);
			return true;
		}
		if (inactiveList.Contains(obj))
		{
			return true;
		}
		return false;
	}

	public void MatchObjectCount(int count)
	{
		if (count <= cap)
		{
			int totalObjectCount = GetTotalObjectCount();
			for (int i = totalObjectCount; i < count; i++)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(prefab);
				gameObject.SetActive(value: false);
				gameObject.transform.parent = ObjectPoolManager.GetOPMTransform();
				inactiveList.Add(gameObject);
			}
		}
	}

	public int GetTotalObjectCount()
	{
		return inactiveList.Count + activeList.Count;
	}

	public void Clear()
	{
		for (int i = 0; i < inactiveList.Count; i++)
		{
			if (inactiveList[i] != null)
			{
				UnityEngine.Object.Destroy(inactiveList[i]);
			}
		}
		for (int j = 0; j < activeList.Count; j++)
		{
			if (activeList[j] != null)
			{
				UnityEngine.Object.Destroy(inactiveList[j]);
			}
		}
	}
}
