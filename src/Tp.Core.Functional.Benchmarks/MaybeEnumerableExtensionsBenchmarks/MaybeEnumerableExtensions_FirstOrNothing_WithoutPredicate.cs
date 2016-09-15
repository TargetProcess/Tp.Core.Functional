// ReSharper disable InvokeAsExtensionMethod
// ReSharper disable InconsistentNaming
// ReSharper disable FieldCanBeMadeReadOnly.Local

using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;

namespace Tp.Core.Functional.Benchmarks.MaybeEnumerableExtensionsBenchmarks
{
	public class MaybeEnumerableExtensions_FirstOrNothing_WithoutPredicate
	{
		private IEnumerable<int> _enumerableData = Enumerable.Range(0, 5);
		private IEnumerable<int> _listData = Enumerable.Range(0, 5).ToList();
		private IEnumerable<int> _arrayData = Enumerable.Range(0, 5).ToArray();

		private IEnumerable<int> _emptyEnumerableData = Enumerable.Empty<int>();
		private IEnumerable<int> _emptyListData = new List<int>();
		private IEnumerable<int> _emptyArrayData = new int[0];

		#region Enumerable

		[Benchmark]
		public Maybe<int> FirstOrNothing_Last__Enumerable_WithValue()
		{
			return MaybeEnumerableExtensions.FirstOrNothing(_enumerableData);
		}

		[Benchmark]
		public Maybe<int> FirstOrNothing_v1__Enumerable_WithValue()
		{
			return Implementations.FirstOrNothing_v1(_enumerableData);
		}

		[Benchmark]
		public Maybe<int> FirstOrNothing_Last__Enumerable_Empty()
		{
			return MaybeEnumerableExtensions.FirstOrNothing(_emptyEnumerableData);
		}

		[Benchmark]
		public Maybe<int> FirstOrNothing_v1__Enumerable_Empty()
		{
			return Implementations.FirstOrNothing_v1(_emptyEnumerableData);
		}

		#endregion // Enumerable

		#region List

		[Benchmark]
		public Maybe<int> FirstOrNothing_Last__List_WithValue()
		{
			return MaybeEnumerableExtensions.FirstOrNothing(_listData);
		}

		[Benchmark]
		public Maybe<int> FirstOrNothing_v1__List_WithValue()
		{
			return Implementations.FirstOrNothing_v1(_listData);
		}

		[Benchmark]
		public Maybe<int> FirstOrNothing_Last__List_Empty()
		{
			return MaybeEnumerableExtensions.FirstOrNothing(_emptyListData);
		}

		[Benchmark]
		public Maybe<int> FirstOrNothing_v1__List_Empty()
		{
			return Implementations.FirstOrNothing_v1(_emptyListData);
		}

		#endregion // List

		#region Array

		[Benchmark]
		public Maybe<int> FirstOrNothing_Last__Array_WithValue()
		{
			return MaybeEnumerableExtensions.FirstOrNothing(_arrayData);
		}

		[Benchmark]
		public Maybe<int> FirstOrNothing_v1__Array_WithValue()
		{
			return Implementations.FirstOrNothing_v1(_arrayData);
		}

		[Benchmark]
		public Maybe<int> FirstOrNothing_Last__Array_Empty()
		{
			return MaybeEnumerableExtensions.FirstOrNothing(_emptyArrayData);
		}

		[Benchmark]
		public Maybe<int> FirstOrNothing_v1__Array_Empty()
		{
			return Implementations.FirstOrNothing_v1(_emptyArrayData);
		}

		#endregion // Array

		private static class Implementations
		{
			public static Maybe<T> FirstOrNothing_v1<T>(IEnumerable<T> items)
			{
				return items.FirstOrNothing(x => true);
			}
		}
	}
}