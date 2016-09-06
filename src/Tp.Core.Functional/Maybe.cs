using System;
using System.Collections.Generic;
using System.Linq;
using Tp.Core.Annotations;

namespace Tp.Core
{
	public static class Maybe
	{
		public delegate bool TryDelegate<in TArg, TResult>(TArg value, out TResult result);

		public static readonly Nothing Nothing = default(Nothing);

		public static Maybe<T> Just<T>(T value) => new Maybe<T>(value);

		public static Maybe<T> Return<T>(T value) => Just(value);

		public static Maybe<T> ReturnIfNotNull<T>(T value) where T : class
		{
			return value == null ? Maybe<T>.Nothing : Just(value);
		}

		public static Maybe<T> Try<T>([InstantHandle] Func<T> action) => Core.Try.Create(action).ToMaybe();

		public static Maybe<TResult> FromTryOut<TArg, TResult>([InstantHandle] TryDelegate<TArg, TResult> call, TArg value)
		{
			TResult result;
			return call(value, out result) ? Just(result) : Maybe<TResult>.Nothing;
		}

		public static Maybe<TResult> FromTryOut<TResult>([InstantHandle] TryDelegate<string, TResult> call, string value)
		{
			return FromTryOut<string, TResult>(call, value);
		}

		public static Maybe<T> Do<T>(
			this Maybe<T> m,
			[NotNull] [InstantHandle] Action<T> f,
			[InstantHandle] Action @else = null)
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

		public static bool TryGetValue<T>(this Maybe<T> m, out T value)
		{
			if (m.HasValue)
			{
				value = m.Value;
				return true;
			}

			value = default(T);
			return false;
		}

		public static Maybe<TTo> Select<TFrom, TTo>(this Maybe<TFrom> m, [InstantHandle] Func<TFrom, TTo> f)
		{
			return m.HasValue ? Return(f(m.Value)) : Maybe<TTo>.Nothing;
		}

		public static Maybe<T> Where<T>(this Maybe<T> m, Func<T, bool> condition)
		{
			return m.HasValue && condition(m.Value) ? m : Maybe<T>.Nothing;
		}

		public static Maybe<TTo> Bind<TFrom, TTo>(this Maybe<TFrom> m, [InstantHandle] Func<TFrom, Maybe<TTo>> f)
		{
			return m.HasValue ? f(m.Value) : Maybe<TTo>.Nothing;
		}

		public static Maybe<T> OfType<T>(this IMaybe maybe, bool nullMeansNothing = true)
		{
			return maybe.HasValue ? maybe.Value.MaybeAs<T>(nullMeansNothing) : Maybe<T>.Nothing;
		}

		public static Try<TVal> ToTry<TVal, TError>(this Maybe<TVal> maybe, [InstantHandle] Func<TError> error)
			where TError : Exception
		{
			return maybe.HasValue ? (Try<TVal>) new Success<TVal>(maybe.Value) : new Failure<TVal>(error());
		}

		public static TVal GetOrThrow<TVal, TError>(this Maybe<TVal> maybe, [InstantHandle] Func<TError> error)
			where TError : Exception
		{
			if (!maybe.HasValue)
			{
				throw error();
			}

			return maybe.Value;
		}

		public static TVal GetOrThrow<TVal>(this Maybe<TVal> maybe, string error)
		{
			if (!maybe.HasValue)
			{
				throw new InvalidOperationException(error);
			}

			return maybe.Value;
		}

		public static Maybe<TC> SelectMany<TA, TB, TC>(
			this Maybe<TA> ma,
			[InstantHandle] Func<TA, Maybe<TB>> func,
			[InstantHandle] Func<TA, TB, TC> selector)
		{
			if (!ma.HasValue)
			{
				return Maybe<TC>.Nothing;
			}

			var mb = func(ma.Value);
			return !mb.HasValue ? Maybe<TC>.Nothing : Just(selector(ma.Value, mb.Value));
		}

		public static Maybe<TTo> MaybeAs<TTo>(this object o, bool nullMeansNothing = true)
		{
			if ((!nullMeansNothing && o == null) || o is TTo)
			{
				return Just((TTo) o);
			}

			return Maybe<TTo>.Nothing;
		}

		public static Maybe<T> NothingIfNull<T>(this T o) where T : class => ReturnIfNotNull(o);

		public static Maybe<T> NothingIfNull<T>(this T? o) where T : struct
		{
			return o.HasValue ? Just(o.Value) : Maybe<T>.Nothing;
		}

		public static Maybe<T> OrElse<T>(this Maybe<T> maybe, [InstantHandle] Func<Maybe<T>> @else)
		{
			return maybe.HasValue ? maybe : @else();
		}

		public static T GetOrElse<T>(this Maybe<T> maybe, [InstantHandle] Func<T> @else)
		{
			return maybe.HasValue ? maybe.Value : @else();
		}

		public static T GetOrDefault<T>(this Maybe<T> maybe, T @default = default(T))
		{
			return maybe.HasValue ? maybe.Value : @default;
		}

		public static Maybe<TResult> Either<T1, T2, TResult>(
			this Maybe<T1> left, Maybe<T2> right,
			[InstantHandle] Func<T1, TResult> caseLeft,
			[InstantHandle] Func<T2, TResult> caseRight)
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
		{
			foreach (var getMaybe in maybes)
			{
				var maybe = getMaybe();
				if (maybe.HasValue)
				{
					return maybe;
				}
			}

			return Maybe<T>.Nothing;
		}
	}

	public struct Maybe<T> : IMaybe
	{
		public static readonly Maybe<T> Nothing = default (Maybe<T>);

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

		public static implicit operator Maybe<T>(Nothing nothing) => Nothing;

		public static implicit operator Maybe<T>(T value)
		{
			var maybe = value as IMaybe;
			return maybe != null && !maybe.HasValue ? Nothing : new Maybe<T>(value);
		}

		public bool Equals(Maybe<T> other)
		{
			if (!HasValue && !other.HasValue)
			{
				return true;
			}

			if (other.HasValue && HasValue)
			{
				var equalityComparer = EqualityComparer<T>.Default;
				return equalityComparer.Equals(other.Value, Value);
			}

			return false;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
			{
				return false;
			}

			if (obj is Maybe<T>)
			{
				return Equals((Maybe<T>) obj);
			}

			var maybe = obj as IMaybe;
			return maybe != null && !maybe.HasValue && !HasValue;
		}

		public override int GetHashCode() => HasValue ? EqualityComparer<T>.Default.GetHashCode(Value) : 0;

		public static bool operator ==(Maybe<T> left, Maybe<T> right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(Maybe<T> left, Maybe<T> right)
		{
			return !left.Equals(right);
		}

		internal Maybe(T value)
		{
			_value = value;
			HasValue = true;
		}

		[Pure]
		public IEnumerator<T> GetEnumerator()
		{
			if (HasValue)
			{
				yield return _value;
			}
		}

		public override string ToString() => HasValue ? $"Just<{typeof (T).Name}>( {Value} )" : "Nothing";
	}
}