// ReSharper disable InvokeAsExtensionMethod
// ReSharper disable InconsistentNaming
// ReSharper disable FieldCanBeMadeReadOnly.Local

using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;

namespace Tp.Core.Functional.Benchmarks.MaybeEnumerableExtensionsBenchmarks
{
	public class MaybeEnumerableExtensions_Choose_EnumerableMaybeAsSource
	{
		private IEnumerable<Maybe<int>> _enumerable = Enumerable.Range(0, 10).Select(Maybe.Just);
		private IEnumerable<Maybe<int>> _array = Enumerable.Range(0, 10).Select(Maybe.Just).ToArray();
		private IEnumerable<Maybe<int>> _list = Enumerable.Range(0, 10).Select(Maybe.Just).ToList();

		#region Enumerable

		[Benchmark]
		public int Choose_Last__Enumerable()
		{
			return MaybeEnumerableExtensions.Choose(_enumerable).Count();
		}

		[Benchmark]
		public int Choose_v1__Enumerable()
		{
			return Implementations.Choose_v1(_enumerable).Count();
		}

		#endregion // Enumerable

		#region Array

		[Benchmark]
		public int Choose_Last__Array()
		{
			return MaybeEnumerableExtensions.Choose(_array).Count();
		}

		[Benchmark]
		public int Choose_v1__Array()
		{
			return Implementations.Choose_v1(_array).Count();
		}

		#endregion // Array

		#region List

		[Benchmark]
		public int Choose_Last__List()
		{
			return MaybeEnumerableExtensions.Choose(_list).Count();
		}

		[Benchmark]
		public int Choose_v1__List()
		{
			return Implementations.Choose_v1(_list).Count();
		}

		#endregion // List

		private static class Implementations
		{
			public static IEnumerable<T> Choose_v1<T>(IEnumerable<Maybe<T>> items)
			{
				return items.Choose(x => x);
			}
		}
	}
}