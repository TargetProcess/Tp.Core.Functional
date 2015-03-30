// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections.Generic;
using NUnit.Framework;

namespace Tp.Core.Functional.Tests
{
	[TestFixture]
	public class MaybeTests
	{
		[Test]
		public void NothingTest()
		{
			Maybe<int> maybeInt = Maybe.Nothing;
			Maybe<object> maybeObject = Maybe.Nothing;

			// ReSharper disable once SuspiciousTypeConversion.Global
			Assert.IsTrue(maybeInt.Equals(maybeObject));
			Assert.IsTrue(maybeInt == maybeObject);
			Assert.IsTrue(maybeObject.Equals(maybeInt));
			Assert.IsTrue(maybeObject == maybeInt);
		}

		[Test]
		public void NothingSelfTest()
		{
			Maybe<int> maybeInt1 = Maybe.Nothing;
			Maybe<int> maybeInt2 = Maybe.Nothing;

			Assert.IsTrue(maybeInt1.Equals(maybeInt2));
			Assert.IsTrue(maybeInt1 == maybeInt2);
			Assert.IsTrue(maybeInt2.Equals(maybeInt1));
			Assert.IsTrue(maybeInt2 == maybeInt1);
		}

		[Test]
		public void JustEqualTest()
		{
			Maybe<int> maybeInt1 = 1;
			Maybe<int> maybeInt2 = 1;

			Assert.IsTrue(maybeInt1.Equals(maybeInt2));
			Assert.IsTrue(maybeInt1 == maybeInt2);
			Assert.IsTrue(maybeInt2.Equals(maybeInt1));
			Assert.IsTrue(maybeInt2 == maybeInt1);
		}
		[Test]
		public void JustNotEqualTest()
		{
			Maybe<int> maybeInt1 = 1;
			Maybe<int> maybeInt2 = 2;

			Assert.IsFalse(maybeInt1.Equals(maybeInt2));
			Assert.IsFalse(maybeInt1 == maybeInt2);
			Assert.IsFalse(maybeInt2.Equals(maybeInt1));
			Assert.IsFalse(maybeInt2 == maybeInt1);
		}

		[Test]
		public void JustNotEqualDiffTypesTest()
		{
			Maybe<int> maybeInt1 = 1;

			// Unfortunately, R# has a bug which breaks the inspection here without introducing the separate variable (http://youtrack.jetbrains.com/issue/RSRP-393174).
			// Should be inlined when it's fixed.
			// ReSharper disable once ConvertToConstant.Local
			int sampleValue = 1;
			Maybe<object> maybeInt2 = sampleValue;

// ReSharper disable once SuspiciousTypeConversion.Global
			Assert.IsFalse(maybeInt1.Equals(maybeInt2));
			Assert.IsFalse(maybeInt1 == maybeInt2);
			Assert.IsFalse(maybeInt2.Equals(maybeInt1));
			Assert.IsFalse(maybeInt2 == maybeInt1);
		}

		[Test]
		public void SelectManyTest()
		{
			const string value = "value";
			Maybe<string> ma = Maybe.Just(value);

			var r = from a in ma
					from b in Maybe.Just(a)
					from c in Maybe.Just(b)
					from d in Maybe.Just(c)
					select a + b + c + d;

			Assert.IsTrue(r.HasValue);
			Assert.AreEqual(value + value + value + value, r.Value);


			var x = from a in ma
					from b in Maybe<object>.Nothing
					from c in Maybe.Just(b)
					select a + b + c;

			Assert.IsFalse(x.HasValue);
		}

		[Test]
		public void JustHasValue()
		{
			var hi = Maybe.Just("hi");
			Assert.IsTrue(hi.HasValue);
			Assert.AreEqual(hi.Value, "hi");
		}

		[Test, ExpectedException(typeof(System.InvalidOperationException), ExpectedMessage = "Cannot get value from Nothing")]
		public void NothingHasNoValue()
		{
			Maybe<string> hi = Maybe.Nothing;
			Assert.IsFalse(hi.HasValue);
			Assert.AreNotEqual(hi.Value, "hi");
		}

		[Test, TestCaseSource("GetEitherTestCases")]
		public Maybe<string> MaybeEitherTests(Maybe<int> left, Maybe<int> right)
		{
			return left.Either(right, l => "Left " + l, r => "Right " + r);
		}

		private static IEnumerable<TestCaseData> GetEitherTestCases()
		{
			yield return new TestCaseData(Maybe.Just(5), Maybe<int>.Nothing)
				.Returns(Maybe.Just("Left 5"))
				.SetDescription("Should use the left branch if it has a value");

			yield return new TestCaseData(Maybe.Just(5), Maybe.Just(7))
				.Returns(Maybe.Just("Left 5"))
				.SetDescription("Should use the left branch with a value even if the right one has a value");

			yield return new TestCaseData(Maybe<int>.Nothing, Maybe.Just(7))
				.Returns(Maybe.Just("Right 7"))
				.SetDescription("Should use the right branch if it has a value");

			yield return new TestCaseData(Maybe<int>.Nothing, Maybe<int>.Nothing)
				.Returns(Maybe<int>.Nothing)
				.SetDescription("Should return Nothing if no branch has a value");
		}

		[Test]
		public void EitherShouldShortCircuitEvenIfLeftBranchReturnsNothing()
		{
			var left = Maybe.Just(5);
			var right = Maybe.Just("Hello");

			var result = left.Either(right, l => Maybe<string>.Nothing, Maybe.Just);
			Assert.AreEqual(result, Maybe.Just(Maybe<string>.Nothing));
		}

		class Foo { }
		class Bar : Foo { }

		[Test]
		public void NothingOfTypeTest()
		{
			var nothingBar = Maybe<Bar>.Nothing;
			var nothingFoo = Maybe<Foo>.Nothing;
			var nothingInt = Maybe<int>.Nothing;


			Assert.IsFalse(nothingBar.OfType<Foo>().HasValue);
			Assert.IsFalse(nothingBar.OfType<Bar>().HasValue);
			Assert.IsFalse(nothingBar.OfType<int>().HasValue);

			Assert.IsFalse(nothingFoo.OfType<Bar>().HasValue);

			Assert.IsFalse(nothingInt.OfType<Foo>().HasValue);

		}

		[Test]
		public void OfTypeTest()
		{
			var foo = Maybe.Just(new Foo());
			var bar = Maybe.Just(new Bar());

			Maybe<Foo> foobar = Maybe.Just((Foo)new Bar());


			Assert.IsTrue(foo.OfType<Foo>().HasValue);
			Assert.IsFalse(foo.OfType<Bar>().HasValue);
			Assert.IsFalse(foo.OfType<int>().HasValue);

			Assert.IsTrue(bar.OfType<Foo>().HasValue);
			Assert.IsInstanceOf<Foo>(bar.OfType<Foo>().Value);

			Assert.IsTrue(foobar.OfType<Foo>().HasValue);

			Assert.IsTrue(foobar.OfType<Bar>().HasValue);

			var i = Maybe.Just(0);
			Assert.IsTrue(i.OfType<int>().HasValue);
			Assert.IsTrue(i.OfType<object>().HasValue);
		}

	}
}