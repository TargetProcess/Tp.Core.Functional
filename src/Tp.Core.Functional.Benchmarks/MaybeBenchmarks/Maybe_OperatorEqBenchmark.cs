using BenchmarkDotNet.Attributes;

// ReSharper disable FieldCanBeMadeReadOnly.Local
// ReSharper disable InconsistentNaming

namespace Tp.Core.Functional.Benchmarks.MaybeBenchmarks
{
	public class Maybe_OperatorEqBenchmark
	{
		private Maybe<int> _first = Maybe.Just(1);
		private Maybe<int> _second = Maybe.Just(1);

		[Benchmark]
		public bool OperatorEq_Last()
		{
			return _first == _second;
		}

		[Benchmark]
		public bool OperatorEq_v1()
		{
			return Implementations.OperatorEq_v1(_first, _second);
		}

		private static class Implementations
		{
			public static bool OperatorEq_v1<T>(Maybe<T> first, Maybe<T> second)
			{
				return Equals(first, second);
			}
		}
	}
}