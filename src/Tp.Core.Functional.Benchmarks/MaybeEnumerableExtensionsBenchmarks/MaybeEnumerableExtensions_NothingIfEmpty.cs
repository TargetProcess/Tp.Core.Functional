// ReSharper disable InvokeAsExtensionMethod
// ReSharper disable InconsistentNaming
// ReSharper disable FieldCanBeMadeReadOnly.Local

using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;

namespace Tp.Core.Functional.Benchmarks.MaybeEnumerableExtensionsBenchmarks
{
	public class MaybeEnumerableExtensions_NothingIfEmpty
	{
		private ICollection<int> _data = Enumerable.Range(0, 10).ToArray();

		[Benchmark]
		public Maybe<IEnumerable<int>> NothingIfEmpty_Last()
		{
			return MaybeEnumerableExtensions.NothingIfEmpty(_data);
		}

		[Benchmark]
		public Maybe<IEnumerable<int>> NothingIfEmpty_v1()
		{
			return Implementations.NothingIfEmpty_v1(_data);
		}

		private static class Implementations
		{
			public static Maybe<IEnumerable<T>> NothingIfEmpty_v1<T>(ICollection<T> xs)
			{
				return xs.Any() ? Maybe.Return(xs.AsEnumerable()) : Maybe.Nothing;
			}
		}
	}
}