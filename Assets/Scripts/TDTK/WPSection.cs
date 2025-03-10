using System.Collections.Generic;
using UnityEngine;

namespace TDTK
{
	public class WPSection
	{
		public Transform waypointT;

		private List<Vector3> posList = new List<Vector3>();

		public bool isPlatform;

		public PlatformTD platform;

		public int pathIDOnPlatform;

		public WPSection(Transform wpT)
		{
			waypointT = wpT;
		}

		public List<Vector3> GetPosList()
		{
			return posList;
		}

		public void SetPosList(List<Vector3> list)
		{
			posList = list;
		}

		public Vector3 GetStartPos()
		{
			if (isPlatform)
			{
				return platform.GetSubPathPath(pathIDOnPlatform)[0];
			}
			return posList[0];
		}

		public Vector3 GetEndPos()
		{
			if (isPlatform)
			{
				return platform.GetSubPathPath(pathIDOnPlatform)[platform.GetSubPathPath(pathIDOnPlatform).Count - 1];
			}
			return posList[posList.Count - 1];
		}
	}
}
