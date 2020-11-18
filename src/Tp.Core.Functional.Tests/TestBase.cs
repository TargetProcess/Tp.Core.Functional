using System;
using NUnit.Framework;

namespace Tp.Core.Functional.Tests
{
	public class TestBase
	{
		protected static T Fail<T>()
		{
			Assert.Fail();
			return default!;
		}

		protected static TRes Fail<TArg, TRes>(TArg arg)
		{
			Assert.Fail();
			return default!;
		}

		protected static void Fail<T>(T arg)
		{
			Assert.Fail();
		}

		protected static T Throw<T>()
		{
			throw new TestException();
		}

		protected static T Throw<TArg, T>(TArg arg)
		{
			throw new TestException();
		}

		protected static void AssertSuccess<T>(Try<int> @where, T expected)
		{
			Assert.IsTrue(@where.IsSuccess);
			Assert.AreEqual(expected, @where.Value);
		}

		protected static void AssertFailure<TException>(Try<int> failedFailure) where TException : Exception
		{
			Assert.IsFalse(failedFailure.IsSuccess);
			Assert.Throws<TException>(() =>
			{
				// ReSharper disable once UnusedVariable
				var value = failedFailure.Value;
			});
		}

		// ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Global
		protected void AssertNothing<T>(Maybe<T> nothing)
		{
			Assert.IsFalse(nothing.HasValue);
		}

		protected void AssertSome<T>(Maybe<T> maybe, T value)
		{
			Assert.IsTrue(maybe.HasValue);
			Assert.AreEqual(value, maybe.Value);
		}
	}
}