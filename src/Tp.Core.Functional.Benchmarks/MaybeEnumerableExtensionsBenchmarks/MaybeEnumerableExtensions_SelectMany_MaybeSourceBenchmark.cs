// ReSharper disable InvokeAsExtensionMethod
// ReSharper disable InconsistentNaming

using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;

namespace Tp.Core.Functional.Benchmarks.MaybeEnumerableExtensionsBenchmarks
{
	public class MaybeEnumerableExtensions_SelectMany_MaybeSourceBenchmark
	{
		private readonly Maybe<int> _data = 10;
		private readonly IEnumerable<int> _enumerable = Enumerable.Range(0, 10);
		private readonly IEnumerable<int> _list = Enumerable.Range(0, 10).ToList();
		private readonly IEnumerable<int> _array = Enumerable.Range(0, 10).ToArray();
		private readonly Func<int, int, int> _resultSelector = (i, mi) => mi;

		#region Enumerable

		[Benchmark]
		public Maybe<int> SelectManyLast_Last__Enumerable()
		{
			return MaybeEnumerableExtensions
				.SelectMany(_data, _ => _enumerable, _resultSelector)
				.Select(xs => xs.Count());
		}

		[Benchmark]
		public Maybe<int> SelectMany_v1__Enumerable()
		{
			return Implementations
				.SelectMany_v1(_data, _ => _enumerable, _resultSelector)
				.Select(xs => xs.Count());
		}

		#endregion // Enumerable

		#region List

		[Benchmark]
		public Maybe<int> SelectManyLast_Last__List()
		{
			return MaybeEnumerableExtensions
				.SelectMany(_data, _ => _list, _resultSelector)
				.Select(xs => xs.Count());
		}

		[Benchmark]
		public Maybe<int> SelectMany_v1__List()
		{
			return Implementations
				.SelectMany_v1(_data, _ => _list, _resultSelector)
				.Select(xs => xs.Count());
		}

		#endregion // List

		#region Array

		[Benchmark]
		public Maybe<int> SelectManyLast_Last__Array()
		{
			return MaybeEnumerableExtensions
				.SelectMany(_data, _ => _array, _resultSelector)
				.Select(xs => xs.Count());
		}

		[Benchmark]
		public Maybe<int> SelectMany_v1__Array()
		{
			return Implementations
				.SelectMany_v1(_data, _ => _array, _resultSelector)
				.Select(xs => xs.Count());
		}

		#endregion // Array

		private static class Implementations
		{
			public static Maybe<IEnumerable<TResult>> SelectMany_v1<TSource, TCollection, TResult>(Maybe<TSource> source,
				Func<TSource, IEnumerable<TCollection>> collectionSelector,
				Func<TSource, TCollection, TResult> resultSelector)
			{
				return source.Select(sourceItem => collectionSelector(sourceItem)
					.Select(maybeItem => resultSelector(sourceItem, maybeItem)));
			}
		}
	}
}