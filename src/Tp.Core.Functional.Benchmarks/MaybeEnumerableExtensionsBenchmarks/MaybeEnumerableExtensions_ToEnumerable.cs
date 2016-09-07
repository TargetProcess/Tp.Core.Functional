// ReSharper disable InvokeAsExtensionMethod
// ReSharper disable InconsistentNaming
// ReSharper disable FieldCanBeMadeReadOnly.Local

using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;

namespace Tp.Core.Functional.Benchmarks.MaybeEnumerableExtensionsBenchmarks
{
	public class MaybeEnumerableExtensions_ToEnumerable
	{
		private Maybe<int> _dataWithValue = Maybe.Just(5);
		private Maybe<int> _dataWithoutValue = Maybe.Nothing;

		#region HasValue

		[Benchmark]
		public int ToEnumerable_Last__HasValue()
		{
			return MaybeEnumerableExtensions
				.ToEnumerable(_dataWithValue)
				.Count();
		}

		[Benchmark]
		public int ToEnumerable_v1__HasValue()
		{
			return Implementations
				.ToEnumerable_v1(_dataWithValue)
				.Count();
		}

		#endregion // HasValue

		#region HasNoValue

		[Benchmark]
		public Maybe<int[]> ToEnumerable_Last__HasNoValue()
		{
			return MaybeEnumerableExtensions
				.ToEnumerable(_dataWithoutValue)
				.ToArray();
		}

		[Benchmark]
		public Maybe<int[]> ToEnumerable_v1__HasNoValue()
		{
			return Implementations
				.ToEnumerable_v1(_dataWithoutValue)
				.ToArray();
		}

		#endregion // HasNoValue

		private static class Implementations
		{
			public static IEnumerable<T> ToEnumerable_v1<T>(Maybe<T> maybe)
			{
				if (maybe.HasValue)
				{
					yield return maybe.Value;
				}
			}
		}
	}
}