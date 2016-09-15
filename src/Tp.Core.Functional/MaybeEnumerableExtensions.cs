using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Tp.Core.Annotations;

namespace Tp.Core
{
	public static class MaybeEnumerableExtensions
	{
		public static IEnumerable<Maybe<TResult>> SelectMany<TSource, TMaybe, TResult>(
			this IEnumerable<TSource> source,
			Func<TSource, Maybe<TMaybe>> maybeSelector,
			Func<TSource, TMaybe, TResult> resultSelector)
		{
			var list = source as IList<TSource>;
			if (list != null)
			{
				return SelectManyList(maybeSelector, resultSelector, list);
			}

			return SelectManyIterator(source, maybeSelector, resultSelector);
		}

		public static Maybe<IEnumerable<TResult>> SelectMany<TSource, TCollection, TResult>(
			this Maybe<TSource> source,
			[InstantHandle] Func<TSource, IEnumerable<TCollection>> collectionSelector,
			Func<TSource, TCollection, TResult> resultSelector)
		{
			if (!source.HasValue)
			{
				return Maybe<IEnumerable<TResult>>.Nothing;
			}

			var collection = collectionSelector(source.Value);

			var list = collection as IList<TCollection>;
			if (list != null)
			{
				var result = new TResult[list.Count];

				for (var i = 0; i < list.Count; i++)
				{
					result[i] = resultSelector(source.Value, list[i]);
				}

				return Maybe.Just<IEnumerable<TResult>>(result);
			}

			return Maybe.Just(collection.Select(maybeItem => resultSelector(source.Value, maybeItem)));
		}

		public static IEnumerable<T> ToEnumerable<T>(this Maybe<T> maybe)
		{
			if (!maybe.HasValue)
			{
				return new T[0];
			}

			return new[] {maybe.Value};
		}

		public static Maybe<T> FirstOrNothing<T>(this IEnumerable<T> items)
		{
			var enumerator = items.GetEnumerator();
			return enumerator.MoveNext() ? Maybe.Just(enumerator.Current) : Maybe<T>.Nothing;
		}

		public static Maybe<T> FirstOrNothing<T>(this IEnumerable<T> items, [InstantHandle] Func<T, bool> condition)
		{
			foreach (var item in items)
			{
				if (condition(item))
				{
					return Maybe.Just(item);
				}
			}

			return Maybe<T>.Nothing;
		}

		public static Maybe<T> SingleOrNothing<T>(this IEnumerable<T> items, bool throwOnSeveral = true)
		{
			var array = items as T[];
			if (array != null)
			{
				return SingleOrNothingArray(array, throwOnSeveral);
			}

			var list = items as IList<T>;
			if (list != null)
			{
				return SingleOrNothingList(list, throwOnSeveral);
			}

			return SingleOrNothingEnumerable(items, throwOnSeveral);
		}

		public static Maybe<T> SingleOrNothing<T>(
			this IEnumerable<T> items,
			[InstantHandle] Func<T, bool> condition,
			bool throwOnSeveral = true)
		{
			var array = items as T[];
			if (array != null)
			{
				return SingleOrNothingArray(array, condition, throwOnSeveral);
			}

			var list = items as List<T>;
			if (list != null)
			{
				return SingleOrNothingList(list, condition, throwOnSeveral);
			}

			return SingleOrNothingEnumerable(items, condition, throwOnSeveral);
		}

		public static IEnumerable<Maybe<TTo>> Bind<TTo, TFrom>(
			this IEnumerable<Maybe<TFrom>> source,
			Func<TFrom, Maybe<TTo>> selector)
		{
			var array = source as Maybe<TFrom>[];
			if (array != null)
			{
				return BindArray(selector, array);
			}

			var list = source as IList<Maybe<TFrom>>;
			if (list != null)
			{
				return BindList(selector, list);
			}

			return source.Select(item => item.Bind(selector));
		}

		/// <summary>
		/// Returns Nothing if any of <paramref name="parts"/> element is nothing.
		/// </summary>
		public static Maybe<IEnumerable<T>> Sequence<T>(this IEnumerable<Maybe<T>> parts)
		{
			var result = new List<T>();

			foreach (var maybe in parts)
			{
				if (!maybe.HasValue)
				{
					return Maybe<IEnumerable<T>>.Nothing;
				}

				result.Add(maybe.Value);
			}

			return Maybe.Just<IEnumerable<T>>(result);
		}

		public static IEnumerable<T> Choose<T>(this IEnumerable<Maybe<T>> items)
		{
			return items.Choose(x => x);
		}

		public static IEnumerable<TResult> Choose<T, TResult>(this IEnumerable<T> items, Func<T, Maybe<TResult>> f)
		{
			return items.Choose(f, (_, x) => x);
		}

		public static IEnumerable<TResult> Choose<T, TIntermediate, TResult>(
			this IEnumerable<T> items,
			Func<T, Maybe<TIntermediate>> f,
			Func<T, TIntermediate, TResult> resultSelector)
		{
			foreach (var item in items)
			{
				var maybeValue = f(item);
				if (maybeValue.HasValue)
				{
					yield return resultSelector(item, maybeValue.Value);
				}
			}
		}

		public static Maybe<IEnumerable<T>> NothingIfEmpty<T>(this ICollection<T> xs)
		{
			return xs.Count > 0 ? Maybe.Just<IEnumerable<T>>(xs) : Maybe<IEnumerable<T>>.Nothing;
		}

		public static IEnumerable<T> EmptyIfNothing<T>(this Maybe<IEnumerable<T>> items)
		{
			return items.GetOrDefault(Enumerable.Empty<T>());
		}

		private static IEnumerable<Maybe<TResult>> SelectManyIterator<TSource, TMaybe, TResult>(
			IEnumerable<TSource> source,
			Func<TSource, Maybe<TMaybe>> maybeSelector,
			Func<TSource, TMaybe, TResult> resultSelector)
		{
			foreach (var item in source)
			{
				yield return GetResultItemForSelectMany(item, maybeSelector, resultSelector);
			}
		}

		private static Maybe<TResult>[] SelectManyList<TSource, TMaybe, TResult>(
			Func<TSource, Maybe<TMaybe>> maybeSelector,
			Func<TSource, TMaybe, TResult> resultSelector,
			IList<TSource> list)
		{
			var result = new Maybe<TResult>[list.Count];

			for (var i = 0; i < list.Count; i++)
			{
				result[i] = GetResultItemForSelectMany(list[i], maybeSelector, resultSelector);
			}

			return result;
		}

		private static Maybe<TResult> GetResultItemForSelectMany<TSource, TMaybe, TResult>(
			TSource sourceItem,
			Func<TSource, Maybe<TMaybe>> maybeSelector,
			Func<TSource, TMaybe, TResult> resultSelector)
		{
			var selectionResult = maybeSelector(sourceItem);
			return selectionResult.HasValue
				? Maybe.Just(resultSelector(sourceItem, selectionResult.Value))
				: Maybe<TResult>.Nothing;
		}

		private static Maybe<T> SingleOrNothingArray<T>(T[] array, bool throwOnSeveral)
		{
			if (array.Length == 0)
			{
				return Maybe.Nothing;
			}

			if (array.Length == 1)
			{
				return Maybe.Just(array[0]);
			}

			if (throwOnSeveral)
			{
				throw new InvalidOperationException("The input sequence contains more than one element.");
			}

			return Maybe<T>.Nothing;
		}

		private static Maybe<T> SingleOrNothingList<T>(IList<T> list, bool throwOnSeveral)
		{
			if (list.Count == 0)
			{
				return Maybe<T>.Nothing;
			}

			if (list.Count == 1)
			{
				return Maybe.Just(list[0]);
			}

			if (throwOnSeveral)
			{
				throw new InvalidOperationException("The input sequence contains more than one element.");
			}

			return Maybe<T>.Nothing;
		}

		private static Maybe<T> SingleOrNothingEnumerable<T>(IEnumerable<T> items, bool throwOnSeveral)
		{
			var enumerator = items.GetEnumerator();
			if (!enumerator.MoveNext())
			{
				return Maybe<T>.Nothing;
			}

			var first = enumerator.Current;
			if (!enumerator.MoveNext())
			{
				return Maybe.Just(first);
			}

			if (throwOnSeveral)
			{
				throw new InvalidOperationException("The input sequence contains more than one element.");
			}

			return Maybe<T>.Nothing;
		}

		private static Maybe<T> SingleOrNothingEnumerable<T>(
			IEnumerable<T> items,
			[InstantHandle] Func<T, bool> condition,
			bool throwOnSeveral = true)
		{
			var result = Maybe<T>.Nothing;

			foreach (var item in items)
			{
				if (SingleOrNothingProcessItem(condition, item, throwOnSeveral, ref result))
				{
					break;
				}
			}

			return result;
		}

		[SuppressMessage("ReSharper", "ForCanBeConvertedToForeach")]
		[SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
		private static Maybe<T> SingleOrNothingArray<T>(
			T[] items,
			[InstantHandle] Func<T, bool> condition,
			bool throwOnSeveral = true)
		{
			var result = Maybe<T>.Nothing;

			for (var i = 0; i < items.Length; i++)
			{
				if (SingleOrNothingProcessItem(condition, items[i], throwOnSeveral, ref result))
				{
					break;
				}
			}

			return result;
		}

		[SuppressMessage("ReSharper", "ForCanBeConvertedToForeach")]
		private static Maybe<T> SingleOrNothingList<T>(
			List<T> items,
			[InstantHandle] Func<T, bool> condition,
			bool throwOnSeveral = true)
		{
			var result = Maybe<T>.Nothing;

			for (var i = 0; i < items.Count; i++)
			{
				if (SingleOrNothingProcessItem(condition, items[i], throwOnSeveral, ref result))
				{
					break;
				}
			}

			return result;
		}

		private static bool SingleOrNothingProcessItem<T>(
			Func<T, bool> condition,
			T item,
			bool throwOnSeveral,
			ref Maybe<T> result)
		{
			if (!condition(item))
			{
				return false;
			}

			if (result.HasValue)
			{
				if (throwOnSeveral)
				{
					throw new InvalidOperationException("The input sequence contains more than one element.");
				}

				result = Maybe<T>.Nothing;
				return true;
			}
			else
			{
				result = Maybe.Just(item);
				return false;
			}
		}

		private static IEnumerable<Maybe<TTo>> BindArray<TTo, TFrom>(Func<TFrom, Maybe<TTo>> selector, Maybe<TFrom>[] array)
		{
			var result = new Maybe<TTo>[array.Length];

			for (var i = 0; i < array.Length; i++)
			{
				result[i] = array[i].Bind(selector);
			}

			return result;
		}

		private static IEnumerable<Maybe<TTo>> BindList<TTo, TFrom>(Func<TFrom, Maybe<TTo>> selector, IList<Maybe<TFrom>> list)
		{
			var result = new Maybe<TTo>[list.Count];

			for (var i = 0; i < list.Count; i++)
			{
				result[i] = list[i].Bind(selector);
			}

			return result;
		}
	}
}