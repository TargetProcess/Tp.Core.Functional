using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;

// ReSharper disable InconsistentNaming

namespace Tp.Core.Functional.Benchmarks.MaybeBenchmarks
{
	public class Maybe_AnyBenchmark
	{
		private readonly Func<Maybe<bool>>[] _data =
		{
			() => Maybe.Nothing,
			() => Maybe.Nothing,
			() => Maybe.Just(true),
			() => Maybe.Nothing
		};

		[Benchmark]
		public Maybe<bool> Any_Last()
		{
			return Maybe.Any(_data);
		}

		[Benchmark]
		public Maybe<bool> Any_v1()
		{
			return Implementations.Any_v1(_data);
		}

		private static class Implementations
		{
			public static Maybe<T> Any_v1<T>(Func<Maybe<T>>[] maybes)
			{
				return FirstOrDefault(maybes.Select(f => f()).Where(m => m.HasValue), Maybe<T>.Nothing);
			}

			private static T FirstOrDefault<T>(IEnumerable<T> source, T defaultValue)
			{
				using (var enumerator = source.GetEnumerator())
				{
					return enumerator.MoveNext() ? enumerator.Current : defaultValue;
				}
			}
		}
	}
}
