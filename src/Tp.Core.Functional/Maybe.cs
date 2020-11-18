using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Tp.Core.Annotations;

// ReSharper disable once CheckNamespace
namespace Tp.Core
{
	public static class Maybe
	{
		public static readonly Nothing Nothing = default(Nothing);

		public static Maybe<T> Just<T>(T value) where T : notnull => new Maybe<T>(value);

		public static Maybe<T> Return<T>(T v) where T : notnull => Just(v);

		public static Maybe<T> ReturnIfNotNull<T>(T? v)
			where T : class
		{
			return v == null ? Maybe<T>.Nothing : Just(v);
		}

		public static Maybe<T> Try<T>([InstantHandle] Func<T> action) where T : notnull =>
			Core.Try.Create(action).ToMaybe();

		public delegate bool TryDelegate<in TArg, TResult>(TArg value, out TResult result);

		[PublicAPI]
		public static Maybe<TResult> FromTryOut<TArg, TResult>(
			[InstantHandle] TryDelegate<TArg, TResult> call,
			TArg value)
			where TResult : notnull
		{
			return call(value, out var result) ? Just(result) : Maybe<TResult>.Nothing;
		}

		public static Maybe<TResult> FromTryOut<TResult>(
			[InstantHandle] TryDelegate<string, TResult> call,
			string value)
			where TResult : notnull
		{
			return FromTryOut<string, TResult>(call, value);
		}

		public static Maybe<T> Do<T>(
			this Maybe<T> m,
			[InstantHandle] Action<T> f,
			[InstantHandle] Action? @else = null)
			where T : notnull
		{
			if (m.HasValue)
			{
				f(m.Value);
			}
			else
			{
				@else?.Invoke();
			}

			return m;
		}

		public static bool TryGetValue<T>(this Maybe<T> m, [MaybeNullWhen(false)] out T value)
			where T : notnull
		{
			if (m.HasValue)
			{
				value = m.Value;
				return true;
			}

			value = default!;
			return false;
		}

		public static Maybe<TTo> Select<TFrom, TTo>(this Maybe<TFrom> m, [InstantHandle] Func<TFrom, TTo> f)
			where TFrom : notnull
			where TTo : notnull
		{
			return m.HasValue ? Return(f(m.Value)) : Maybe<TTo>.Nothing;
		}

		public static Maybe<T> Where<T>(this Maybe<T> m, Func<T, bool> condition)
			where T : notnull
		{
			return m.HasValue && condition(m.Value) ? m : Maybe<T>.Nothing;
		}

		public static Maybe<TTo> Bind<TFrom, TTo>(this Maybe<TFrom> m, [InstantHandle] Func<TFrom, Maybe<TTo>> f)
			where TFrom : notnull
			where TTo : notnull
		{
			return m.HasValue ? f(m.Value) : Maybe<TTo>.Nothing;
		}

		public static Maybe<T> OfType<T>(this IMaybe maybe)
			where T : notnull
		{
			return maybe.HasValue ? maybe.Value.MaybeAs<T>() : Maybe<T>.Nothing;
		}

		public static Try<TVal> ToTry<TVal, TError>(this Maybe<TVal> maybe, [InstantHandle] Func<TError> error)
			where TVal : notnull
			where TError : Exception
		{
			if (maybe.HasValue)
			{
				return new Success<TVal>(maybe.Value);
			}

			return new Failure<TVal>(error());
		}

		public static TVal GetOrThrow<TVal, TError>(this Maybe<TVal> maybe, [InstantHandle] Func<TError> error)
			where TVal : notnull
			where TError : Exception
		{
			if (!maybe.HasValue)
			{
				throw error();
			}

			return maybe.Value;
		}

		public static TVal GetOrThrow<TVal>(this Maybe<TVal> maybe, string error)
			where TVal : notnull
		{
			if (!maybe.HasValue)
			{
				throw new InvalidOperationException(error);
			}

			return maybe.Value;
		}

		public static Maybe<TC> SelectMany<TA, TB, TC>(this Maybe<TA> ma, [InstantHandle] Func<TA, Maybe<TB>> func,
			[InstantHandle] Func<TA, TB, TC> selector)
			where TA : notnull
			where TB : notnull
			where TC : notnull
		{
			return ma.Bind(a => func(a).Bind(b => Just(selector(a, b))));
		}

		public static Maybe<TTo> MaybeAs<TTo>(this object? o)
			where TTo : notnull
		{
			if (o is TTo)
				return Just((TTo) o!);

			return Maybe<TTo>.Nothing;
		}

		public static Maybe<T> NothingIfNull<T>(this T? o) where T : class
		{
			return o == null ? Maybe<T>.Nothing : Just(o);
		}

		public static Maybe<T> NothingIfNull<T>(this T? o) where T : struct
		{
			return o.HasValue ? Just(o.Value) : Maybe<T>.Nothing;
		}

		public static Maybe<T> OrElse<T>(this Maybe<T> maybe, [InstantHandle] Func<Maybe<T>> @else)
			where T : notnull
		{
			return maybe.HasValue ? maybe : @else();
		}

		public static T GetOrElse<T>(this Maybe<T> maybe, [InstantHandle] Func<T> @else)
			where T : notnull
		{
			return maybe.HasValue ? maybe.Value : @else();
		}

		public static T GetOrDefault<T>(this Maybe<T> maybe, T @default = default)
			where T : notnull
		{
			return maybe.HasValue ? maybe.Value : @default;
		}

		public static T? GetOrNull<T>(this Maybe<T> maybe)
			where T : class
		{
			return maybe.HasValue ? maybe.Value : null;
		}

		public static Maybe<TResult> Either<T1, T2, TResult>(
			this Maybe<T1> left, Maybe<T2> right,
			[InstantHandle] Func<T1, TResult> caseLeft,
			[InstantHandle] Func<T2, TResult> caseRight)
			where T1 : notnull
			where T2 : notnull
			where TResult : notnull
		{
			if (left.HasValue)
			{
				return Just(caseLeft(left.Value));
			}

			if (right.HasValue)
			{
				return Just(caseRight(right.Value));
			}

			return Maybe<TResult>.Nothing;
		}

		public static T? ToNullable<T>(this Maybe<T> maybe) where T : struct
		{
			return maybe.HasValue ? maybe.Value : (T?) null;
		}

		public static Maybe<T> Any<T>([InstantHandle] params Func<Maybe<T>>[] maybes)
			where T : notnull
		{
			return maybes.Select(f => f()).Where(m => m.HasValue).FirstOrDefault(Maybe<T>.Nothing);
		}
	}

	public readonly struct Maybe<T> : IMaybe, IEquatable<Maybe<T>> where T : notnull
	{
		public static readonly Maybe<T> Nothing = Maybe.Nothing;

		private readonly T _value;

		public bool HasValue { get; }

		object IMaybe.Value => Value;

		public T Value
		{
			get
			{
				if (!HasValue)
				{
					throw new InvalidOperationException("Cannot get value from Nothing");
				}

				return _value;
			}
		}

		// ReSharper disable once UnusedParameter.Global
		public static implicit operator Maybe<T>(Nothing nothing)
		{
			return Nothing;
		}

		public static implicit operator Maybe<T>(T value)
		{
			return value is IMaybe maybe && !maybe.HasValue ? Nothing : new Maybe<T>(value);
		}

		public bool Equals(Maybe<T> other)
		{
			return (!HasValue && !other.HasValue) || (other.HasValue && HasValue && Equals(other.Value, Value));
		}

		public override bool Equals(object? obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (obj is Maybe<T> typed)
			{
				return Equals(typed);
			}

			return obj is IMaybe asInterface && !asInterface.HasValue && !HasValue;
		}

		public override int GetHashCode() => HasValue ? EqualityComparer<T>.Default.GetHashCode(Value) : 0;

		public static bool operator ==(Maybe<T> left, Maybe<T> right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(Maybe<T> left, Maybe<T> right)
		{
			return !Equals(left, right);
		}

		internal Maybe(T value)
		{
			_value = value ?? throw new ArgumentNullException(nameof(value));
			HasValue = true;
		}

		[Pure]
		public IEnumerator<T> GetEnumerator()
		{
			if (HasValue)
				yield return _value;
		}

		public override string ToString() => HasValue ? $"Just<{typeof(T).Name}>( {Value} )" : "Nothing";
	}
}