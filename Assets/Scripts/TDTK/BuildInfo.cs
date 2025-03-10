using System;
using System.Collections.Generic;
using UnityEngine;

namespace TDTK
{
	[Serializable]
	public class BuildInfo
	{
		public _TileStatus status = _TileStatus.Available;

		public Vector3 position = Vector3.zero;

		public PlatformTD platform;

		public List<int> availableTowerIDList = new List<int>();
	}
}
