using System;
using Tp.Core.Annotations;

// ReSharper disable once CheckNamespace
namespace Tp.Core
{
	public static class TryExtensions
	{
		public static Try<T> Flatten<T>(this Try<Try<T>> @try)
		{
			return @try.SelectMany(x => x);
		}

		public static Maybe<T> ToMaybe<T>(this Try<T> @try) where T : notnull
		{
			Maybe<T> result = default;
			@try.Switch(x => result = Maybe.Just(x), _ => result = Maybe<T>.Nothing);
			return result;
		}

		public static Try<T> Recover<T, TException>(
			this Try<T> @try,
			[InstantHandle] Func<TException, Maybe<T>> recover)
			where T : notnull
			where TException : Exception
		{
			Try<T> result = default!;
			@try.Switch(x => result = @try,
				exception => Try
					.Create(() => exception.MaybeAs<TException>().Bind(recover).ToTry(() => exception))
					.Flatten());
			return result;
		}
	}
}