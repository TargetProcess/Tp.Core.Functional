using BenchmarkDotNet.Attributes;

// ReSharper disable FieldCanBeMadeReadOnly.Local
// ReSharper disable InconsistentNaming

namespace Tp.Core.Functional.Benchmarks.NothingBenchmarks
{
	public class Nothing_OperatorEqBenchmark
	{
		private Nothing _first = new Nothing();
		private Nothing _second = new Nothing();

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
			public static bool OperatorEq_v1(Nothing first, Nothing second)
			{
				return Equals(first, second);
			}
		}
	}
}