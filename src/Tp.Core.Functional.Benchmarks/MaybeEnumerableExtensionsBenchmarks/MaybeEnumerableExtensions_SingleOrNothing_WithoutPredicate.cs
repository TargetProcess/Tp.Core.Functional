// ReSharper disable InvokeAsExtensionMethod
// ReSharper disable InconsistentNaming
// ReSharper disable FieldCanBeMadeReadOnly.Local

using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;

namespace Tp.Core.Functional.Benchmarks.MaybeEnumerableExtensionsBenchmarks
{
	public class MaybeEnumerableExtensions_SingleOrNothing_WithoutPredicate
	{
		private IEnumerable<int> _enumerable = Enumerable.Range(0, 10);
		private IEnumerable<int> _array = Enumerable.Range(0, 10).ToArray();
		private IEnumerable<int> _list = Enumerable.Range(0, 10).ToList();

		#region Enumerable

		[Benchmark]
		public Maybe<int> SingleOrNothing_Last__Enumerable()
		{
			return MaybeEnumerableExtensions.SingleOrNothing(_enumerable, false);
		}

		[Benchmark]
		public Maybe<int> SingleOrNothing_v1__Enumerable()
		{
			return Implementations.SingleOrNothing_v1(_enumerable, false);
		}

		#endregion // Enumerable

		#region Array

		[Benchmark]
		public Maybe<int> SingleOrNothing_Last__Array()
		{
			return MaybeEnumerableExtensions.SingleOrNothing(_array, false);
		}

		[Benchmark]
		public Maybe<int> SingleOrNothing_v1__Array()
		{
			return Implementations.SingleOrNothing_v1(_array, false);
		}

		#endregion // Array

		#region List

		[Benchmark]
		public Maybe<int> SingleOrNothing_Last__List()
		{
			return MaybeEnumerableExtensions.SingleOrNothing(_list, false);
		}

		[Benchmark]
		public Maybe<int> SingleOrNothing_v1__List()
		{
			return Implementations.SingleOrNothing_v1(_list, false);
		}

		#endregion // List

		private static class Implementations
		{
			public static Maybe<T> SingleOrNothing_v1<T>(
				IEnumerable<T> items,
				bool throwOnSeveral = true)
			{
				return items.SingleOrNothing(x => true, throwOnSeveral);
			}
		}
	}
}