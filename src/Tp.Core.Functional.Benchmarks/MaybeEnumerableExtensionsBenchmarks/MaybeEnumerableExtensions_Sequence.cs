// ReSharper disable InvokeAsExtensionMethod
// ReSharper disable InconsistentNaming
// ReSharper disable FieldCanBeMadeReadOnly.Local

using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;

namespace Tp.Core.Functional.Benchmarks.MaybeEnumerableExtensionsBenchmarks
{
	public class MaybeEnumerableExtensions_Sequence
	{
		private IEnumerable<Maybe<int>> _data = Enumerable.Range(0, 10).Select(Maybe.Just).ToArray();

		[Benchmark]
		public Maybe<IEnumerable<int>> Sequence_Last()
		{
			return MaybeEnumerableExtensions.Sequence(_data);
		}

		[Benchmark]
		public Maybe<IEnumerable<int>> Sequence_v1()
		{
			return Implementations.Sequence_v1(_data);
		}

		private static class Implementations
		{
			public static Maybe<IEnumerable<T>> Sequence_v1<T>(IEnumerable<Maybe<T>> parts)
			{
				var result = new List<T>();

				foreach (var maybe in parts)
				{
					if (maybe.HasValue)
					{
						result.Add(maybe.Value);
					}
					else
					{
						return Maybe.Nothing;
					}
				}

				return Maybe.Just((IEnumerable<T>) result.AsReadOnly());
			}
		}
	}
}