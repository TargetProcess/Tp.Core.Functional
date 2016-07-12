using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;

// ReSharper disable InvokeAsExtensionMethod
namespace Tp.Core.Functional.Benchmarks
{
	public class DictionaryExtensionsBenchmark
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
			return DictionaryExtensions_v1.GetValue(_data, 42);
		}
	}

	public static class DictionaryExtensions_v1
	{
		public static Maybe<TVal> GetValue<TKey, TVal>(this IDictionary<TKey, TVal> d, TKey k)
		{
			return k == null ? Maybe.Nothing : Maybe.FromTryOut<TKey, TVal>(d.TryGetValue, k);
		}
	}
}