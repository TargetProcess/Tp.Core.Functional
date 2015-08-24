using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Tp.Core.Annotations;

namespace Tp.Core
{
	public static class MaybeEnumerableExtensions
	{
		public static IEnumerable<Maybe<TResult>> SelectMany<TSource, TMaybe, TResult>(this IEnumerable<TSource> source,
			Func<TSource, Maybe<TMaybe>> maybeSelector, Func<TSource, TMaybe, TResult> resultSelector)
		{
			return source.Select(sourceItem => maybeSelector(sourceItem).Select(maybeItem => resultSelector(sourceItem, maybeItem)));
		}

		public static Maybe<IEnumerable<TResult>> SelectMany<TSource, TCollection, TResult>(this Maybe<TSource> source,
			[InstantHandle] Func<TSource, IEnumerable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
		{
			return source.Select(sourceItem => collectionSelector(sourceItem).Select(maybeItem => resultSelector(sourceItem, maybeItem)));
		}

		public static IEnumerable<T> ToEnumerable<T>(this Maybe<T> maybe)
		{
			if (maybe.HasValue)
			{
				yield return maybe.Value;
			}
		}

		public static Maybe<T> FirstOrNothing<T>(this IEnumerable<T> items)
		{
			return FirstOrNothing(items, x => true);
		}

		public static Maybe<T> SingleOrNothing<T>(this IEnumerable<T> items, bool throwOnSeveral = true)
		{
			return SingleOrNothing(items, x => true, throwOnSeveral);
		}

		public static Maybe<T> FirstOrNothing<T>(this IEnumerable<T> items, [InstantHandle] Func<T, bool> condition)
		{
			using (var enumerator = items.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					var current = enumerator.Current;
					if (condition(current))
						return Maybe.Just(current);
				}
				return Maybe.Nothing;
			}
		}

		public static Maybe<T> SingleOrNothing<T>(this IEnumerable<T> items, [InstantHandle] Func<T, bool> condition, bool throwOnSeveral = true)
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

		public static IEnumerable<Maybe<TTo>> Bind<TTo, TFrom>(this IEnumerable<Maybe<TFrom>> m, Func<TFrom, Maybe<TTo>> f)
		{
			return m.Select(x => x.Bind(f));
		}

		/// <summary>
		/// Returns Nothing if any of <paramref name="parts"/> element is nothing.
		/// </summary>
		public static Maybe<IEnumerable<T>> Sequence<T>(this IEnumerable<Maybe<T>> parts)
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
			return Maybe.Just((IEnumerable<T>)result.AsReadOnly());
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
			return xs.Any() ? Maybe.Return(xs.AsEnumerable()) : Maybe.Nothing;
		}
	}
}