// ReSharper disable InvokeAsExtensionMethod
// ReSharper disable InconsistentNaming
// ReSharper disable FieldCanBeMadeReadOnly.Local

using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;

namespace Tp.Core.Functional.Benchmarks.MaybeEnumerableExtensionsBenchmarks
{
	public class MaybeEnumerableExtensions_Choose_EnumerableAsSource
	{
		private IEnumerable<int> _enumerable = Enumerable.Range(0, 10);
		private IEnumerable<int> _array = Enumerable.Range(0, 10).ToArray();
		private IEnumerable<int> _list = Enumerable.Range(0, 10).ToList();
		private Func<int, Maybe<int>> _selector = x => x > 5 ? Maybe.Just(x) : Maybe.Nothing;

		#region Enumerable

		[Benchmark]
		public int Choose_Last__Enumerable()
		{
			return MaybeEnumerableExtensions.Choose(_enumerable, _selector).Count();
		}

		[Benchmark]
		public int Choose_v1__Enumerable()
		{
			return Implementations.Choose_v1(_enumerable, _selector).Count();
		}

		#endregion // Enumerable

		#region Array

		[Benchmark]
		public int Choose_Last__Array()
		{
			return MaybeEnumerableExtensions.Choose(_array, _selector).Count();
		}

		[Benchmark]
		public int Choose_v1__Array()
		{
			return Implementations.Choose_v1(_array, _selector).Count();
		}

		#endregion // Array

		#region List

		[Benchmark]
		public int Choose_Last__List()
		{
			return MaybeEnumerableExtensions.Choose(_list, _selector).Count();
		}

		[Benchmark]
		public int Choose_v1__List()
		{
			return Implementations.Choose_v1(_list, _selector).Count();
		}

		#endregion // List

		private static class Implementations
		{
			public static IEnumerable<TResult> Choose_v1<T, TResult>(IEnumerable<T> items, Func<T, Maybe<TResult>> f)
			{
				return MaybeEnumerableExtensions.Choose(items, f, (_, x) => x);
			}
		}
	}
}