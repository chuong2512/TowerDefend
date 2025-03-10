using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class CombineMesh : MonoBehaviour
{
	private void Start()
	{
		Quaternion localRotation = base.transform.localRotation;
		IEnumerator enumerator = base.transform.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Transform transform = (Transform)enumerator.Current;
				transform.position += base.transform.position;
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
		base.transform.position = Vector3.zero;
		base.transform.rotation = Quaternion.identity;
		MeshFilter[] componentsInChildren = GetComponentsInChildren<MeshFilter>();
		CombineInstance[] array = new CombineInstance[componentsInChildren.Length - 1];
		int num = 0;
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (!(componentsInChildren[i].sharedMesh == null))
			{
				array[num].mesh = componentsInChildren[i].sharedMesh;
				array[num++].transform = componentsInChildren[i].transform.localToWorldMatrix;
				componentsInChildren[i].GetComponent<Renderer>().enabled = false;
			}
		}
		GetComponent<MeshFilter>().mesh = new Mesh();
		GetComponent<MeshFilter>().mesh.CombineMeshes(array);
		GetComponent<Renderer>().material = componentsInChildren[1].GetComponent<Renderer>().sharedMaterial;
		base.transform.localRotation = localRotation;
	}
}
