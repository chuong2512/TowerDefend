using UnityEngine;

namespace MadLevelManager
{
	public class MadUndo
	{
		public static void LegacyRegisterSceneUndo(string name)
		{
		}

		public static void LegacyRegisterUndo(Object obj, string name)
		{
		}

		public static void DestroyObjectImmediate(GameObject gameObject)
		{
		}

		public static void RecordObject(Object obj, string name)
		{
		}

		public static void RecordObject2(Object obj, string name)
		{
			RecordObject(obj, name);
			LegacyRegisterUndo(obj, name);
		}

		public static void RegisterCreatedObjectUndo(Object obj, string name)
		{
		}
	}
}
