// ReSharper disable InconsistentNaming
// ReSharper disable FieldCanBeMadeReadOnly.Local

using BenchmarkDotNet.Attributes;

namespace Tp.Core.Functional.Benchmarks.MaybeBenchmarks
{
	public class Maybe_Equals
	{
		private Maybe<int> _first = Maybe.Just(1);
		private Maybe<int> _second = Maybe.Just(1);

		[Benchmark]
		public bool Equals_Last()
		{
			return _first.Equals(_second);
		}

		[Benchmark]
		public bool Equals_v1()
		{
			return Implementations.Equals_v1(_first, _second);
		}

		private static class Implementations
		{
			public static bool Equals_v1<T>(Maybe<T> first, Maybe<T> second)
			{
				return (!first.HasValue && !second.HasValue) || (second.HasValue && first.HasValue && Equals(second.Value, first.Value));
			}
		}
	}
}
