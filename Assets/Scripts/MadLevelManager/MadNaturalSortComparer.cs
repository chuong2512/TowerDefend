using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MadLevelManager
{
	public class MadNaturalSortComparer : IComparer<string>, IDisposable
	{
		private readonly bool isAscending;

		private Dictionary<string, string[]> table = new Dictionary<string, string[]>();

		public MadNaturalSortComparer(bool inAscendingOrder = true)
		{
			isAscending = inAscendingOrder;
		}

		public int Compare(string x, string y)
		{
			throw new NotImplementedException();
		}

		int IComparer<string>.Compare(string x, string y)
		{
			if (x == y)
			{
				return 0;
			}
			if (!table.TryGetValue(x, out string[] value))
			{
				value = Regex.Split(x.Replace(" ", string.Empty), "([0-9]+)");
				table.Add(x, value);
			}
			if (!table.TryGetValue(y, out string[] value2))
			{
				value2 = Regex.Split(y.Replace(" ", string.Empty), "([0-9]+)");
				table.Add(y, value2);
			}
			int num;
			for (int i = 0; i < value.Length && i < value2.Length; i++)
			{
				if (value[i] != value2[i])
				{
					num = PartCompare(value[i], value2[i]);
					return (!isAscending) ? (-num) : num;
				}
			}
			num = ((value2.Length > value.Length) ? 1 : ((value.Length > value2.Length) ? (-1) : 0));
			return (!isAscending) ? (-num) : num;
		}

		private static int PartCompare(string left, string right)
		{
			if (!int.TryParse(left, out int result))
			{
				return left.CompareTo(right);
			}
			if (!int.TryParse(right, out int result2))
			{
				return left.CompareTo(right);
			}
			return result.CompareTo(result2);
		}

		public void Dispose()
		{
			table.Clear();
			table = null;
		}
	}
}
