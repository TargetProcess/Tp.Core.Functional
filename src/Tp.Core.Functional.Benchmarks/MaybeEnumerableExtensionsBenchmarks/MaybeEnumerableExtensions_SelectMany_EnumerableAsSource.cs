// ReSharper disable InvokeAsExtensionMethod
// ReSharper disable InconsistentNaming
// ReSharper disable FieldCanBeMadeReadOnly.Local

using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;

namespace Tp.Core.Functional.Benchmarks.MaybeEnumerableExtensionsBenchmarks
{
	public class MaybeEnumerableExtensions_SelectMany_EnumerableAsSource
	{
		private IEnumerable<int> _enumerableData = Enumerable.Range(0, 10);
		private IEnumerable<int> _listData = Enumerable.Range(0, 10).ToList();
		private IEnumerable<int> _arrayData = Enumerable.Range(0, 10).ToArray();
		private Func<int, Maybe<int>> _maybeSelector = i => i%2 == 0 ? Maybe.Just(i) : Maybe<int>.Nothing;
		private Func<int, int, int> _resultSelector = (i, mi) => mi;

		#region Enumerable

		[Benchmark]
		public int SelectMany_Last__Enumerable()
		{
			return MaybeEnumerableExtensions
				.SelectMany(_enumerableData, _maybeSelector, _resultSelector)
				.Count();
		}

		[Benchmark]
		public int SelectMany_v1__Enumerable()
		{
			return Implementations
				.SelectMany_v1(_enumerableData, _maybeSelector, _resultSelector)
				.Count();
		}

		#endregion // Enumerable

		#region List

		[Benchmark]
		public int SelectMany_Last__List()
		{
			return MaybeEnumerableExtensions
				.SelectMany(_listData, _maybeSelector, _resultSelector)
				.Count();
		}

		[Benchmark]
		public int SelectMany_v1__List()
		{
			return Implementations
				.SelectMany_v1(_listData, _maybeSelector, _resultSelector)
				.Count();
		}

		#endregion // List

		#region Array

		[Benchmark]
		public int SelectMany_Last__Array()
		{
			return MaybeEnumerableExtensions
				.SelectMany(_arrayData, _maybeSelector, _resultSelector)
				.Count();
		}

		[Benchmark]
		public int SelectMany_v1__Array()
		{
			return Implementations
				.SelectMany_v1(_arrayData, _maybeSelector, _resultSelector)
				.Count();
		}

		#endregion // Array

		private static class Implementations
		{
			public static IEnumerable<Maybe<TResult>> SelectMany_v1<TSource, TMaybe, TResult>(
				IEnumerable<TSource> source,
				Func<TSource, Maybe<TMaybe>> maybeSelector,
				Func<TSource, TMaybe, TResult> resultSelector)
			{
				return source.Select(
					sourceItem => maybeSelector(sourceItem).Select(
						maybeItem => resultSelector(sourceItem, maybeItem)));
			}
		}
	}
}