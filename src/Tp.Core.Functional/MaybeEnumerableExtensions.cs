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

		[DebuggerStepThrough]
		public static IEnumerable<T> ToEnumerable<T>(this Maybe<T> maybe)
		{
			if (maybe.HasValue)
			{
				yield return maybe.Value;
			}
		}

		[DebuggerStepThrough]
		public static Maybe<T> FirstOrNothing<T>(this IEnumerable<T> items)
		{
			return FirstOrNothing(items, x => true);
		}

		[DebuggerStepThrough]
		public static Maybe<T> SingleOrNothing<T>(this IEnumerable<T> items, bool throwOnSeveral = true)
		{
			return SingleOrNothing(items, x => true, throwOnSeveral);
		}

		[DebuggerStepThrough]
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

		[DebuggerStepThrough]
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
						}

						result = Maybe.Just(current);
					}
				}
				return result;
			}
		}

		[DebuggerStepThrough]
		public static IEnumerable<Maybe<TTo>> Bind<TTo, TFrom>(this IEnumerable<Maybe<TFrom>> m, Func<TFrom, Maybe<TTo>> f)
		{
			return m.Select(x => x.Bind<TFrom, TTo>(f));
		}

		[DebuggerStepThrough]
		public static IEnumerable<T> Choose<T>(this IEnumerable<Maybe<T>> items)
		{
			return items.Choose(x => x);
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

		[DebuggerStepThrough]
		public static IEnumerable<TResult> Choose<T, TResult>(this IEnumerable<T> items, Func<T, Maybe<TResult>> f)
		{
			return items.Select(f).Where(x => x.HasValue).Select(x => x.Value);
		}

		[DebuggerStepThrough]
		public static Maybe<IEnumerable<T>> NothingIfEmpty<T>(this ICollection<T> xs) where T : class
		{
			return xs.Any() ? Maybe.Return(xs.AsEnumerable()) : Maybe.Nothing;
		}
	}
}