using System.Collections.Generic;
using UnityEngine;

namespace TDTK
{
	public class ResourceDB : MonoBehaviour
	{
		public List<Rsc> rscList = new List<Rsc>();

		public static ResourceDB LoadDB()
		{
			GameObject gameObject = Resources.Load("DB_TDTK/ResourceDB", typeof(GameObject)) as GameObject;
			return gameObject.GetComponent<ResourceDB>();
		}

		public static List<Rsc> Load()
		{
			GameObject gameObject = Resources.Load("DB_TDTK/ResourceDB", typeof(GameObject)) as GameObject;
			ResourceDB component = gameObject.GetComponent<ResourceDB>();
			return component.rscList;
		}

		public static List<Rsc> LoadClone()
		{
			GameObject gameObject = Resources.Load("DB_TDTK/ResourceDB", typeof(GameObject)) as GameObject;
			ResourceDB component = gameObject.GetComponent<ResourceDB>();
			List<Rsc> list = new List<Rsc>();
			if (component != null)
			{
				for (int i = 0; i < component.rscList.Count; i++)
				{
					list.Add(component.rscList[i].Clone());
				}
			}
			return list;
		}
	}
}
