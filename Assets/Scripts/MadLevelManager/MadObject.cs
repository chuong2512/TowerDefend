using UnityEngine;

namespace MadLevelManager
{
	public class MadObject : MonoBehaviour
	{
		public static int TableHash<T>(T[] table)
		{
			int num = 11;
			for (int i = 0; i < table.Length; i++)
			{
				T val = table[i];
				num = num * 37 + val.GetHashCode();
			}
			return num;
		}
	}
}
