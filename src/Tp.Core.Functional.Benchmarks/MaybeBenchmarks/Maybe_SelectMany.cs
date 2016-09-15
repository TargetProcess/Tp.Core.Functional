// ReSharper disable FieldCanBeMadeReadOnly.Local
// ReSharper disable InconsistentNaming
// ReSharper disable InvokeAsExtensionMethod

using System;
using BenchmarkDotNet.Attributes;

namespace Tp.Core.Functional.Benchmarks.MaybeBenchmarks
{
	public class Maybe_SelectMany
	{
		private Maybe<int> _ma = Maybe.Just(1);

		[Benchmark]
		public Maybe<int> SelectMany_Last()
		{
			return Maybe.SelectMany(_ma, a => Maybe.Just(a), (a, b) => a + b);
		}

		[Benchmark]
		public Maybe<int> SelectMany_v1()
		{
			return Implementations.SelectMany_v1(_ma, a => Maybe.Just(a), (a, b) => a + b);
		}

		private static class Implementations
		{
			public static Maybe<TC> SelectMany_v1<TA, TB, TC>(Maybe<TA> ma, Func<TA, Maybe<TB>> func, Func<TA, TB, TC> selector)
			{
				return ma.Bind(a => func(a).Bind(b => Maybe.Just(selector(a, b))));
			}
		}
	}
}