// ReSharper disable InvokeAsExtensionMethod
// ReSharper disable InconsistentNaming
// ReSharper disable FieldCanBeMadeReadOnly.Local

using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;

namespace Tp.Core.Functional.Benchmarks.NullableBenchmarks
{
	public class NullableExtensions_Choose
	{
		private IEnumerable<int> _enumerable = Enumerable.Range(0, 10);
		private IEnumerable<int> _array = Enumerable.Range(0, 10).ToArray();
		private IEnumerable<int> _list = Enumerable.Range(0, 10).ToList();
		private Func<int, int?> _selector = x => x > 5 ? (int?)x : null;

		#region Enumerable

		[Benchmark]
		public int Choose_Last__Enumerable()
		{
			return NullableExtensions.Choose(_enumerable, _selector).Count();
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
			return NullableExtensions.Choose(_array, _selector).Count();
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
			return NullableExtensions.Choose(_list, _selector).Count();
		}

		[Benchmark]
		public int Choose_v1__List()
		{
			return Implementations.Choose_v1(_list, _selector).Count();
		}

		#endregion // List

		private static class Implementations
		{
			public static IEnumerable<TTo> Choose_v1<T, TTo>(IEnumerable<T> xs, Func<T, TTo?> map) where TTo : struct
			{
				return xs.Select(map).Where(m => m.HasValue).Select(m => m.Value);
			}
		}
	}
}