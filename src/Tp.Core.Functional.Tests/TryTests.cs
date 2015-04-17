// 
// Copyright (c) 2005-2015 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using NUnit.Framework;

namespace Tp.Core.Functional.Tests
{
	class TestException : Exception
	{

	}

	[TestFixture]
	public class TryTests : TestBase
	{
		private readonly Try<int> _success = Try.Success(1);
		private readonly Try<int> _failure = Try.Failure<int>(new TestException());


		[Test]
		public void EqualTest()
		{
			Assert.AreEqual(_success, Try.Success(1));
			Assert.IsFalse(_success.Equals(null));
			// ReSharper disable once EqualExpressionComparison
			Assert.IsTrue(_success.Equals(_success));
		}

		[Test]
		public void CreateTest()
		{
			AssertSuccess(Try.Create(() => 1), 1);
			AssertFailure<TestException>(Try.Create(Throw<int>));
		}

		[Test]
		public void SuccessFailureTest()
		{
			Assert.AreEqual(1, _success.Value);

			Assert.False(_failure.IsSuccess);
		}


		[Test]
		public void GetOrElseTest()
		{
			Assert.AreEqual(1, _success.GetOrElse(Fail<int>));
			Assert.AreEqual(1, _failure.GetOrElse(() => 1));
		}

		[Test]
		public void OrElseTest()
		{
			Assert.AreEqual(_success, _success.OrElse(Fail<Try<int>>));
			Assert.AreEqual(_success, _failure.OrElse(() => _success));
			AssertFailure<TestException>(_failure.OrElse(Throw<Try<int>>));
		}

		[Test]
		public void WhereTest()
		{
			AssertSuccess(_success.Where(x => x == 1), 1);
			AssertFailure<ArgumentOutOfRangeException>(_success.Where(x => x == 2));
			AssertFailure<TestException>(_failure.Where(Fail<int, bool>));
			AssertFailure<TestException>(_success.Where(Throw<int, bool>));
		}

		[Test]
		public void SelectManyTest()
		{
			AssertSuccess(_success.SelectMany(x => Try.Success(2)), 2);
			AssertFailure<TestException>(_failure.SelectMany(Fail<int, Try<int>>));
			AssertFailure<TestException>(_success.SelectMany(Throw<int, Try<int>>));
		}

		[Test]
		public void SelectTest()
		{
			AssertSuccess(_success.Select(x => x * 2), 2);
			AssertFailure<TestException>(_failure.Select(Fail<int, int>));
			AssertFailure<TestException>(_success.Select(Throw<int, int>));
		}

		[Test]
		public void RecoverTest()
		{
			AssertSuccess(_success.Recover((Func<Exception, int>)Fail<Exception, int>), 1);
			AssertSuccess(_success.Recover((Func<Exception, Try<int>>)Fail<Exception, Try<int>>), 1);

			AssertSuccess(_failure.Recover(e =>
			{
				Assert.IsInstanceOf<TestException>(e);
				return 5;
			}), 5);


			AssertSuccess(_failure.Recover(e =>
			{
				Assert.IsInstanceOf<TestException>(e);
				return Try.Success(5);
			}), 5);

			AssertFailure<NullReferenceException>(_failure.Recover(e =>
			{
				Assert.IsInstanceOf<TestException>(e);
				return Try.Failure<int>(new NullReferenceException());
			}));

			AssertFailure<NullReferenceException>(_failure.Recover((Func<Exception, int>)(e =>
			{
				Assert.IsInstanceOf<TestException>(e);
				throw new NullReferenceException();
			})));

			AssertFailure<NullReferenceException>(_failure.Recover((Func<Exception, Try<int>>)(e =>
			{
				Assert.IsInstanceOf<TestException>(e);
				throw new NullReferenceException();
			})));
		}

		[Test]
		public void SwitchTest()
		{
			_success.Switch(i => Assert.AreEqual(1, i), Fail);
			_failure.Switch(Fail, Assert.IsInstanceOf<TestException>);

			Assert.AreEqual(Assert.Counter, 2);
		}
	}
}