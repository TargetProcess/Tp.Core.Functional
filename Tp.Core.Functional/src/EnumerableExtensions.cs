using System.Collections.Generic;

namespace Tp.Core
{
	internal static class EnumerableExtensions
	{
		public static T FirstOrDefault<T>(this IEnumerable<T> source, T defaultValue)
		{
			using (IEnumerator<T> enumerator = source.GetEnumerator())
			{
				return enumerator.MoveNext() ? enumerator.Current : defaultValue;
			}
		}
	}
}