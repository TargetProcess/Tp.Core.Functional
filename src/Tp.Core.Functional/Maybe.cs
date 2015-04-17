using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Tp.Core.Annotations;

namespace Tp.Core
{
	public static class Maybe
	{
		public static readonly Nothing Nothing = default(Nothing);

		[DebuggerStepThrough]
		public static Maybe<T> Just<T>(T value)
		{
			return new Maybe<T>(value);
		}

		[DebuggerStepThrough]
		public static Maybe<T> Return<T>(T v)
		{
			return Just(v);
		}

		[DebuggerStepThrough]
		public static Maybe<T> ReturnIfNotNull<T>(T v)
			where T : class
		{
			return v == null ? Maybe<T>.Nothing : Just(v);
		}

		[DebuggerStepThrough]
		public static Maybe<T> Try<T>([InstantHandle] Func<T> action)
		{
			return Core.Try.Create(action).ToMaybe();
		}

		public delegate bool TryDelegate<in TArg, TResult>(TArg value, out TResult result);

		public static Maybe<TResult> FromTryOut<TArg, TResult>(TryDelegate<TArg, TResult> call, TArg value)
		{
			TResult result;
			return call(value, out result) ? Just(result) : Maybe<TResult>.Nothing;
		}

		public static Maybe<TResult> FromTryOut<TResult>(TryDelegate<string, TResult> call, string value)
		{
			return FromTryOut<string, TResult>(call, value);
		}

		[DebuggerStepThrough]
		public static void Do<TFrom>(this Maybe<TFrom> m, [InstantHandle]Action<TFrom> f, [InstantHandle]Action @else = null)
		{
			if (m.HasValue)
			{
				f(m.Value);
			}
			else
			{
				if (@else != null)
					@else();
			}
		}

		[DebuggerStepThrough]
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

		[DebuggerStepThrough]
		public static Maybe<TTo> Select<TFrom, TTo>(this Maybe<TFrom> m, [InstantHandle]Func<TFrom, TTo> f)
		{
			return m.HasValue ? Return(f(m.Value)) : Maybe<TTo>.Nothing;
		}

		[DebuggerStepThrough]
		public static Maybe<T> Where<T>(this Maybe<T> m, Func<T, bool> condition)
		{
			return m.HasValue && condition(m.Value) ? m : Maybe<T>.Nothing;
		}

		[DebuggerStepThrough]
		public static Maybe<TTo> Bind<TFrom, TTo>(this Maybe<TFrom> m, [InstantHandle]Func<TFrom, Maybe<TTo>> f)
		{
			return m.HasValue ? f(m.Value) : Maybe<TTo>.Nothing;
		}

		public static Maybe<T> OfType<T>(this IMaybe maybe, bool nullMeansNothing = true)
		{
			return maybe.HasValue ? maybe.Value.MaybeAs<T>(nullMeansNothing) : Maybe<T>.Nothing;
		}


		[DebuggerStepThrough]
		public static Try<TVal> ToTry<TVal, TError>(this Maybe<TVal> maybe, [InstantHandle]Func<TError> error) where TError : Exception
		{
			if (maybe.HasValue)
			{
				return new Success<TVal>(maybe.Value);
			}
			return new Failure<TVal>(error());
		}

		[DebuggerStepThrough]
		public static TVal GetOrThrow<TVal, TError>(this Maybe<TVal> maybe, [InstantHandle]Func<TError> error) where TError : Exception
		{
			if (!maybe.HasValue)
			{
				throw error();
			}
			return maybe.Value;
		}

		[DebuggerStepThrough]
		public static TVal GetOrThrow<TVal>(this Maybe<TVal> maybe, string error)
		{
			return maybe.GetOrThrow(() => new InvalidOperationException(error));
		}

		[DebuggerStepThrough]
		public static Maybe<TC> SelectMany<TA, TB, TC>(this Maybe<TA> ma, Func<TA, Maybe<TB>> func, Func<TA, TB, TC> selector)
		{
			return ma.Bind(a => func(a).Bind(b => Just(selector(a, b))));
		}

		[DebuggerStepThrough]
		public static Maybe<TTo> MaybeAs<TTo>(this object o, bool nullMeansNothing = true)
		{
			// ReSharper disable ExpressionIsAlwaysNull
			if ((!nullMeansNothing && o == null) || o is TTo)
				return Just((TTo)o);
			return Maybe<TTo>.Nothing;
			// ReSharper restore ExpressionIsAlwaysNull
		}

		[DebuggerStepThrough]
		public static Maybe<T> NothingIfNull<T>(this T o) where T : class
		{
			return ReturnIfNotNull(o);
		}

		[DebuggerStepThrough]
		public static Maybe<T> NothingIfNull<T>(this T? o) where T : struct
		{
			return o.HasValue ? Just(o.Value) : Maybe<T>.Nothing;
		}

		[DebuggerStepThrough]
		public static Maybe<T> OrElse<T>(this Maybe<T> maybe, [InstantHandle] Func<Maybe<T>> @else)
		{
			return maybe.HasValue ? maybe : @else();
		}

		[DebuggerStepThrough]
		public static T GetOrElse<T>(this Maybe<T> maybe, [InstantHandle] Func<T> @else)
		{
			return maybe.HasValue ? maybe.Value : @else();
		}

		[DebuggerStepThrough]
		public static T GetOrDefault<T>(this Maybe<T> maybe, T @default = default(T))
		{
			return maybe.HasValue ? maybe.Value : @default;
		}

		[DebuggerStepThrough]
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
			return maybe.HasValue ? maybe.Value : (T?)null;
		}

		public static Maybe<T> Any<T>([InstantHandle]params Func<Maybe<T>>[] maybes)
		{
			return maybes.Select(f => f()).Where(m => m.HasValue).FirstOrDefault(Maybe<T>.Nothing);
		}


	}

	public struct Maybe<T> : IMaybe
	{
		public static readonly Maybe<T> Nothing = Maybe.Nothing;

		private readonly bool _hasValue;
		private readonly T _value;

		public bool HasValue
		{
			[DebuggerStepThrough]
			get { return _hasValue; }
		}

		object IMaybe.Value
		{
			[DebuggerStepThrough]
			get
			{
				return Value;
			}
		}

		public T Value
		{
			[DebuggerStepThrough]
			get
			{
				if (!HasValue)
				{
					throw new InvalidOperationException("Cannot get value from Nothing");
				}
				return _value;
			}
		}

		[DebuggerStepThrough]
		public static implicit operator Maybe<T>(Nothing nothing)
		{
			return Nothing;
		}

		[DebuggerStepThrough]
		public static implicit operator Maybe<T>(T value)
		{
			var maybe = value as IMaybe;
			if (maybe != null && !maybe.HasValue)
				return Nothing;
			return new Maybe<T>(value);
		}

		[DebuggerStepThrough]
		public bool Equals(Maybe<T> other)
		{
			return (!HasValue && !other.HasValue) || (other.HasValue && HasValue && Equals(other.Value, Value));
		}

		[DebuggerStepThrough]
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (obj is Maybe<T>)
				return Equals((Maybe<T>)obj);
			var maybe = obj as IMaybe;
			return maybe != null && !maybe.HasValue && !HasValue;
		}

		[DebuggerStepThrough]
		public override int GetHashCode()
		{
			return HasValue ? Value.GetHashCode() : 0;
		}


		[DebuggerStepThrough]
		public static bool operator ==(Maybe<T> left, Maybe<T> right)
		{
			return Equals(left, right);
		}

		[DebuggerStepThrough]
		public static bool operator !=(Maybe<T> left, Maybe<T> right)
		{
			return !Equals(left, right);
		}

		[DebuggerStepThrough]
		internal Maybe(T value)
		{
			_value = value;
			_hasValue = true;
		}

		[Pure]
		public IEnumerator<T> GetEnumerator()
		{
			if (_hasValue)
				yield return _value;
		}

		[DebuggerStepThrough]
		public override string ToString()
		{
			return HasValue ? string.Format("Just<{0}>( {1} )", typeof(T).Name, Value) : "Nothing";
		}


	}


}
