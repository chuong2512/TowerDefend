using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
	public List<Pool> poolList = new List<Pool>();

	public static ObjectPoolManager instance;

	private void Awake()
	{
		if (!(instance != null))
		{
			instance = this;
		}
	}

	public static Transform Spawn(Transform objT, float activeDuration = -1f)
	{
		return Spawn(objT.gameObject, Vector3.zero, Quaternion.identity, activeDuration).transform;
	}

	public static Transform Spawn(Transform objT, Vector3 pos, Quaternion rot, float activeDuration = -1f)
	{
		return instance._Spawn(objT.gameObject, pos, rot, activeDuration).transform;
	}

	public static GameObject Spawn(GameObject obj, float activeDuration = -1f)
	{
		return Spawn(obj, Vector3.zero, Quaternion.identity, activeDuration);
	}

	public static GameObject Spawn(GameObject obj, Vector3 pos, Quaternion rot, float activeDuration = -1f)
	{
		return instance._Spawn(obj, pos, rot, activeDuration);
	}

	public GameObject _Spawn(GameObject obj, Vector3 pos, Quaternion rot, float activeDuration = -1f)
	{
		if (obj == null)
		{
			UnityEngine.Debug.Log("NullReferenceException: obj unspecified");
			return null;
		}
		int num = GetPoolID(obj);
		if (num == -1)
		{
			num = _New(obj);
		}
		GameObject gameObject = poolList[num].Spawn(pos, rot);
		if (activeDuration > 0f)
		{
			StartCoroutine(UnspawnRoutine(gameObject, activeDuration));
		}
		return gameObject;
	}

	private IEnumerator UnspawnRoutine(GameObject spawnedObj, float activeDuration)
	{
		yield return new WaitForSeconds(activeDuration);
		Unspawn(spawnedObj);
	}

	public static void Unspawn(Transform objT, float delay)
	{
		instance.StartCoroutine(instance.UnspawnRoutine(objT.gameObject, delay));
	}

	public static void Unspawn(GameObject obj, float delay)
	{
		instance.StartCoroutine(instance.UnspawnRoutine(obj, delay));
	}

	public static void Unspawn(Transform objT)
	{
		instance._Unspawn(objT.gameObject);
	}

	public static void Unspawn(GameObject obj)
	{
		instance._Unspawn(obj);
	}

	public void _Unspawn(GameObject obj)
	{
		for (int i = 0; i < poolList.Count; i++)
		{
			if (poolList[i].Unspawn(obj))
			{
				return;
			}
		}
		UnityEngine.Object.Destroy(obj);
	}

	public static int New(Transform objT, int count = 2)
	{
		if (instance == null)
		{
			Init();
		}
		return instance._New(objT.gameObject, count);
	}

	public static int New(GameObject obj, int count = 2)
	{
		if (instance == null)
		{
			Init();
		}
		return instance._New(obj, count);
	}

	public int _New(GameObject obj, int count = 2)
	{
		int num = GetPoolID(obj);
		if (num != -1)
		{
			poolList[num].MatchObjectCount(count);
		}
		else
		{
			Pool pool = new Pool();
			pool.prefab = obj;
			pool.MatchObjectCount(count);
			poolList.Add(pool);
			num = poolList.Count - 1;
		}
		return num;
	}

	private int GetPoolID(GameObject obj)
	{
		for (int i = 0; i < poolList.Count; i++)
		{
			if (poolList[i].prefab == obj)
			{
				return i;
			}
		}
		return -1;
	}

	public static void Init()
	{
		if (!(instance != null))
		{
			GameObject gameObject = new GameObject();
			gameObject.name = "ObjectPoolManager";
			instance = gameObject.AddComponent<ObjectPoolManager>();
		}
	}

	public static void ClearAll()
	{
		for (int i = 0; i < instance.poolList.Count; i++)
		{
			instance.poolList[i].Clear();
		}
		instance.poolList = new List<Pool>();
	}

	public static Transform GetOPMTransform()
	{
		return instance.transform;
	}
}
