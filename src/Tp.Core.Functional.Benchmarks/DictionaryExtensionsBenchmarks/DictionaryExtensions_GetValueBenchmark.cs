using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;

// ReSharper disable InvokeAsExtensionMethod
// ReSharper disable InconsistentNaming

namespace Tp.Core.Functional.Benchmarks.DictionaryExtensionsBenchmarks
{
	public class DictionaryExtensions_GetValueBenchmark
	{
		private readonly Dictionary<int, string> _data =
			Enumerable.Range(0, 100).ToDictionary(num => num, num => num.ToString());

		[Benchmark]
		public Maybe<string> GetValue_Last()
		{
			return DictionaryExtensions.GetValue(_data, 42);
		}

		[Benchmark]
		public Maybe<string> GetValue_v1()
		{
			return Implementations.GetValue_v1(_data, 42);
		}

		private static class Implementations
		{
			public static Maybe<TVal> GetValue_v1<TKey, TVal>(IDictionary<TKey, TVal> d, TKey k)
			{
				return k == null ? Maybe.Nothing : Maybe.FromTryOut<TKey, TVal>(d.TryGetValue, k);
			}
		}
	}
}