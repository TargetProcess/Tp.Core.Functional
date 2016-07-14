using System;
using BenchmarkDotNet.Attributes;

// ReSharper disable InconsistentNaming
// ReSharper disable InvokeAsExtensionMethod

namespace Tp.Core.Functional.Benchmarks.MaybeBenchmarks
{
	public class Maybe_GetOrThrowBenchmark
	{
		private readonly Maybe<int> _data = Maybe<int>.Nothing;

		[Benchmark]
		public int GetOrThrow_String_Last()
		{
			try
			{
				return Maybe.GetOrThrow(_data, "Error message");
			}
			catch (InvalidOperationException)
			{
				return -1;
			}
		}

		[Benchmark]
		public int GetOrThrow_String_v1()
		{
			try
			{
				return Implementations.GetOrThrow_v1(_data, "Error message");
			}
			catch (InvalidOperationException)
			{
				return -1;
			}
		}

		private static class Implementations
		{
			public static TVal GetOrThrow_v1<TVal>(Maybe<TVal> maybe, string error)
			{
				return maybe.GetOrThrow(() => new InvalidOperationException(error));
			}
		}
	}
}