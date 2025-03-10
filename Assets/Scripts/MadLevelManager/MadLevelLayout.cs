using UnityEngine;

namespace MadLevelManager
{
	public class MadLevelLayout
	{
		public static MadLevelAbstractLayout current
		{
			get
			{
				MadLevelAbstractLayout madLevelAbstractLayout = TryGet();
				if (madLevelAbstractLayout == null)
				{
					UnityEngine.Debug.LogError("There's no level layout on the current scene");
				}
				return madLevelAbstractLayout;
			}
		}

		public static MadLevelAbstractLayout TryGet()
		{
			Object[] array = Object.FindObjectsOfType(typeof(MadLevelAbstractLayout));
			if (array.Length == 0)
			{
				return null;
			}
			if (array.Length > 1)
			{
				UnityEngine.Debug.LogWarning("There's more than one level layout on the current scene.");
			}
			return array[0] as MadLevelAbstractLayout;
		}
	}
}
