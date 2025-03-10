using System;
using UnityEngine;

namespace TDTK
{
	[Serializable]
	public class ProceduralUnitSetting
	{
		public GameObject unit;

		public UnitCreep unitC;

		public bool enabled = true;

		public int minWave;

		public ProceduralVariable HP = new ProceduralVariable(1f, 50f);

		public ProceduralVariable shield = new ProceduralVariable(0f, 25f);

		public ProceduralVariable speed = new ProceduralVariable(1f, 6f);

		public ProceduralVariable interval = new ProceduralVariable(1f, 6f);
	}
}
