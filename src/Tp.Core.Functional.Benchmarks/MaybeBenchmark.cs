using System;
using BenchmarkDotNet.Attributes;

// ReSharper disable InvokeAsExtensionMethod

namespace Tp.Core.Functional.Benchmarks
{
	public class MaybeBenchmark
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
				return Maybe_v1.GetOrThrow(_data, "Error message");
			}
			catch (InvalidOperationException)
			{
				return -1;
			}
		}
	}

	public static class Maybe_v1
	{
		public static TVal GetOrThrow<TVal>(this Maybe<TVal> maybe, string error)
		{
			return maybe.GetOrThrow(() => new InvalidOperationException(error));
		}
	}
}
