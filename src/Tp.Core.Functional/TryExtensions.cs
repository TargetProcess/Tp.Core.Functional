using System.Collections.Generic;
using System.Linq;

namespace Tp.Core
{
	public static class TryExtensions
	{
		public static Maybe<T> ToMaybe<T>(this IEnumerable<Try<T>> tries)
		{
			return tries.Select(x => x.ToMaybe())
				.Choose()
				.FirstOrNothing();
		}
	}
}
