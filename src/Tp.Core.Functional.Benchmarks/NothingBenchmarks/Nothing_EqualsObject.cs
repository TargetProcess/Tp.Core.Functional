// ReSharper disable FieldCanBeMadeReadOnly.Local
// ReSharper disable InconsistentNaming

using BenchmarkDotNet.Attributes;

namespace Tp.Core.Functional.Benchmarks.NothingBenchmarks
{
	public class Nothing_EqualsObject
	{
		private Nothing _nothing = new Nothing();
		private object _nothingObj = new Nothing();
		private object _maybeObj = Maybe.Just(1);

		[Benchmark]
		public bool Equals_Last__NothingAndNothing()
		{
			return _nothing.Equals(_nothingObj);
		}

		[Benchmark]
		public bool Equals_v1__NothingAndNothing()
		{
			return Implementations.Equals_v1(_nothing, _nothingObj);
		}

		[Benchmark]
		public bool Equals_Last__NothingAndMaybe()
		{
			return _nothing.Equals(_maybeObj);
		}

		[Benchmark]
		public bool Equals_v1__NothingAndMaybe()
		{
			return Implementations.Equals_v1(_nothing, _maybeObj);
		}

		private static class Implementations
		{
			public static bool Equals_v1(Nothing first, object second)
			{
				return second is Nothing || second.MaybeAs<IMaybe>().Select(x => !x.HasValue).GetOrDefault();
			}
		}
	}
}