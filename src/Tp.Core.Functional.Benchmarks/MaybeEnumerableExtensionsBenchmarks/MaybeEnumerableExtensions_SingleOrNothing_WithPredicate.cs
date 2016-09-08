// ReSharper disable InvokeAsExtensionMethod
// ReSharper disable InconsistentNaming
// ReSharper disable FieldCanBeMadeReadOnly.Local

using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;

namespace Tp.Core.Functional.Benchmarks.MaybeEnumerableExtensionsBenchmarks
{
	public class MaybeEnumerableExtensions_SingleOrNothing_WithPredicate
	{
		private Func<int, bool> _predicate = x => x == 9;
		private IEnumerable<int> _enumerable = Enumerable.Range(0, 10);
		private IEnumerable<int> _array = Enumerable.Range(0, 10).ToArray();
		private IEnumerable<int> _list = Enumerable.Range(0, 10).ToList();

		#region IEnumerable

		[Benchmark]
		public Maybe<int> SingleOrNothing_Last__Enumerable()
		{
			return MaybeEnumerableExtensions.SingleOrNothing(_enumerable, _predicate, false);
		}

		[Benchmark]
		public Maybe<int> SingleOrNothing_v1__Enumerable()
		{
			return Implementations.SingleOrNothing_v1(_enumerable, _predicate, false);
		}

		#endregion // IEnumerable

		#region Array

		[Benchmark]
		public Maybe<int> SingleOrNothing_Last__Array()
		{
			return MaybeEnumerableExtensions.SingleOrNothing(_array, _predicate, false);
		}

		[Benchmark]
		public Maybe<int> SingleOrNothing_v1__Array()
		{
			return Implementations.SingleOrNothing_v1(_array, _predicate, false);
		}

		#endregion // Array

		#region List

		[Benchmark]
		public Maybe<int> SingleOrNothing_Last__List()
		{
			return MaybeEnumerableExtensions.SingleOrNothing(_list, _predicate, false);
		}

		[Benchmark]
		public Maybe<int> SingleOrNothing_v1__List()
		{
			return Implementations.SingleOrNothing_v1(_list, _predicate, false);
		}

		#endregion // Array

		private static class Implementations
		{
			public static Maybe<T> SingleOrNothing_v1<T>(
				IEnumerable<T> items,
				Func<T, bool> condition,
				bool throwOnSeveral = true)
			{
				var result = Maybe<T>.Nothing;
				using (var enumerator = items.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						var current = enumerator.Current;
						if (condition(current))
						{
							if (result.HasValue)
							{
								if (throwOnSeveral)
								{
									throw new InvalidOperationException("The input sequence contains more than one element.");
								}

								return Maybe.Nothing;
							}

							result = Maybe.Just(current);
						}
					}

					return result;
				}
			}
		}
	}
}