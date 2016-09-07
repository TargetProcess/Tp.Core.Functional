// ReSharper disable FieldCanBeMadeReadOnly.Local
// ReSharper disable InconsistentNaming

using BenchmarkDotNet.Attributes;

namespace Tp.Core.Functional.Benchmarks.NothingBenchmarks
{
	public class Nothing_OperatorNotEq
	{
		private Nothing _first = new Nothing();
		private Nothing _second = new Nothing();

		[Benchmark]
		public bool OperatorNotEq_Last()
		{
			return _first != _second;
		}

		[Benchmark]
		public bool OperatorNotEq_v1()
		{
			return Implementations.OperatorNotEq_v1(_first, _second);
		}

		private static class Implementations
		{
			public static bool OperatorNotEq_v1(Nothing first, Nothing second)
			{
				return !Equals(first, second);
			}
		}
	}
}