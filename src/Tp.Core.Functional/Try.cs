using System;
using System.Collections.Generic;
using Tp.Core.Annotations;

namespace Tp.Core
{
	public static class Try
	{
		public static Try<T> Create<T>([InstantHandle] Func<T> process)
		{
			try
			{
				return new Success<T>(process());
			}
			catch (Exception e)
			{
				return new Failure<T>(e);
			}
		}

		public static Try<T> Success<T>(T value) => new Success<T>(value);

		public static Try<T> Failure<T>(Exception e) => new Failure<T>(e);
	}

	//// ReSharper disable once InconsistentNaming
	public interface Try<T>
	{
		T GetOrElse(Func<T> @default);
		Try<T> OrElse(Func<Try<T>> @default);
		Maybe<T> ToMaybe();
		Either<T, Exception> ToEither();
		Try<U> Select<U>(Func<T, U> selector);
		Try<T> Where(Func<T, bool> filter);
		Try<U> SelectMany<U>(Func<T, Try<U>> selector);

		T Value { get; }
		bool IsSuccess { get; }
		void Switch([InstantHandle]Action<T> sucess, [InstantHandle]Action<Exception> exception);

		Try<T> Recover([InstantHandle]Func<Exception, T> recover);
		Try<T> Recover([InstantHandle]Func<Exception, Try<T>> recover);

		Try<T> Recover<TException>([InstantHandle] Func<TException, Maybe<T>> recover);
	}

	public sealed class Success<T> : Try<T>, IEquatable<Success<T>>
	{
		public Success(T value)
		{
			Value = value;
		}

		T Try<T>.GetOrElse(Func<T> @default) => Value;

		public Try<T> OrElse(Func<Try<T>> @default) => this;

		public Maybe<T> ToMaybe() => Maybe.Just(Value);

		public Either<T, Exception> ToEither() => Either.CreateLeft<T, Exception>(Value);

		public Try<U> Select<U>(Func<T, U> selector) => Try.Create(() => selector(Value));

		public Try<T> Where(Func<T, bool> filter)
		{
			return Try.Create<Try<T>>(() =>
			{
				if (filter(Value))
					return this;
				return new Failure<T>(new ArgumentOutOfRangeException($"Predicate does not hold for {Value}"));
			}
				).Flatten();
		}

		public Try<U> SelectMany<U>(Func<T, Try<U>> selector)
		{
			try
			{
				return selector(Value);
			}
			catch (Exception e)
			{
				return new Failure<U>(e);
			}
		}

		public T Value { get; }

		public bool IsSuccess { get; } = true;

		public void Switch(Action<T> sucess, Action<Exception> exception) => sucess(Value);

		public Try<T> Recover(Func<Exception, T> recover) => this;

		public Try<T> Recover(Func<Exception, Try<T>> recover) => this;
		public Try<T> Recover<TException>(Func<TException, Maybe<T>> recover) => this;

		public bool Equals(Success<T>? other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return EqualityComparer<T>.Default.Equals(Value, other.Value);
		}

		public override bool Equals(object? obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			return obj is Success<T> && Equals((Success<T>)obj);
		}

		public override int GetHashCode() => EqualityComparer<T>.Default.GetHashCode(Value);
	}

	public sealed class Failure<T> : Try<T>
	{
		private readonly Exception _exception;

		// ReSharper disable once ConvertToAutoProperty
		public Exception Exception
		{
			get { return _exception; }
		}

		public Failure(Exception exception)
		{
			_exception = exception;
		}

		public T GetOrElse(Func<T> @default) => @default();

		public Try<T> OrElse(Func<Try<T>> @default) => Try.Create(@default).Flatten();

		public Maybe<T> ToMaybe() => Maybe<T>.Nothing;

		public Either<T, Exception> ToEither() => Either.CreateRight<T, Exception>(Exception);

		public Try<U> Select<U>(Func<T, U> selector) => new Failure<U>(_exception);

		public Try<T> Where(Func<T, bool> filter) => this;

		public Try<U> SelectMany<U>(Func<T, Try<U>> selector) => new Failure<U>(_exception);

		public T Value
		{
			get { throw _exception; }
		}

		public bool IsSuccess { get; } = false;

		public void Switch(Action<T> sucess, Action<Exception> exception) => exception(_exception);

		public Try<T> Recover(Func<Exception, T> recover) => Try.Create(() => recover(_exception));

		public Try<T> Recover(Func<Exception, Try<T>> recover) => Try.Create(() => recover(_exception)).Flatten();

		public Try<T> Recover<TException>(Func<TException, Maybe<T>> recover)
		{
			return Try.Create(() => _exception.MaybeAs<TException>().Bind(recover).ToTry(() => _exception)).Flatten();
		}
	}
}
