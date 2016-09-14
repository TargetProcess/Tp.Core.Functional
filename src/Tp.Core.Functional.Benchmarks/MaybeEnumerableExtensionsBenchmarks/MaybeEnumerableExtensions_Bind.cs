// ReSharper disable InvokeAsExtensionMethod
// ReSharper disable InconsistentNaming
// ReSharper disable FieldCanBeMadeReadOnly.Local

using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;

namespace Tp.Core.Functional.Benchmarks.MaybeEnumerableExtensionsBenchmarks
{
	public class MaybeEnumerableExtensions_Bind
	{
		private IEnumerable<Maybe<int>> _enumerable = Enumerable.Range(0, 10).Select(Maybe.Just);
		private IEnumerable<Maybe<int>> _list = Enumerable.Range(0, 10).Select(Maybe.Just).ToList();
		private IEnumerable<Maybe<int>> _array = Enumerable.Range(0, 10).Select(Maybe.Just).ToArray();
		private Func<int, Maybe<int>> _selector = i => i*2;

		#region Enumerable

		[Benchmark]
		public int Bind_Last__Enumerable()
		{
			return MaybeEnumerableExtensions
				.Bind(_enumerable, _selector)
				.Count();
		}

		[Benchmark]
		public int Bind_v1__Enumerable()
		{
			return Implementations
				.Bind_v1(_enumerable, _selector)
				.Count();
		}

		#endregion // Enumerable

		#region List

		[Benchmark]
		public int Bind_Last__List()
		{
			return MaybeEnumerableExtensions
				.Bind(_list, _selector)
				.Count();
		}

		[Benchmark]
		public int Bind_v1__List()
		{
			return Implementations
				.Bind_v1(_list, _selector)
				.Count();
		}

		#endregion // List

		#region Array

		[Benchmark]
		public int Bind_Last__Array()
		{
			return MaybeEnumerableExtensions
				.Bind(_array, _selector)
				.Count();
		}

		[Benchmark]
		public int Bind_v1__Array()
		{
			return Implementations
				.Bind_v1(_array, _selector)
				.Count();
		}

		#endregion // Array

		private static class Implementations
		{
			public static IEnumerable<Maybe<TTo>> Bind_v1<TTo, TFrom>(
				IEnumerable<Maybe<TFrom>> m,
				Func<TFrom, Maybe<TTo>> f)
			{
				return m.Select(x => x.Bind(f));
			}
		}
	}
}