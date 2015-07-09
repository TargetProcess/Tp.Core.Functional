using System.Collections.Generic;
using System.Linq;
using Tp.Core;

// ReSharper disable once CheckNamespace
namespace System
{
	public static class NullableExtensions
	{
		public static TTo? Select<T, TTo>(this T? value, Func<T, TTo> func)
			where T : struct
			where TTo : struct
		{
			return value.HasValue ? func(value.Value) : (TTo?)null;
		}

		public static TC? SelectMany<TA, TB, TC>(this TA? ma, Func<TA, TB?> func, Func<TA, TB, TC> selector)
			where TA : struct
			where TB : struct
			where TC : struct
		{
			if (ma.HasValue)
			{
				var mb = func(ma.Value);
				if (mb.HasValue)
				{
					return selector(ma.Value, mb.Value);
				}
			}
			return null;
		}

		public static TTo? Bind<T, TTo>(this T? value, Func<T, TTo?> func)
			where T : struct
			where TTo : struct
		{
			return value.HasValue ? func(value.Value) : null;
		}


		public static IEnumerable<TTo> Choose<T, TTo>(this IEnumerable<T> xs, Func<T, TTo?> map) where TTo : struct
		{
			return xs.Select(map).Where(m => m.HasValue).Select(m => m.Value);
		}

		public static Maybe<T> ToMaybe<T>(this T? value) where T : struct
		{
			return value.HasValue ? Maybe.Just(value.Value) : Maybe.Nothing;
		}
	}
}