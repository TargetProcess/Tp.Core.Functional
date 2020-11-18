//
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
//

using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Tp.Core.Functional.Tests
{
	[TestFixture]
	public class MaybeTests : TestBase
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

			Assert.AreEqual(maybeObject.GetHashCode(), 0);
			Assert.AreEqual(maybeInt.GetHashCode(), 0);

			Assert.AreEqual(Maybe.Nothing, Maybe<int>.Nothing);

			Assert.NotNull(Maybe.Nothing);
			Assert.NotNull(Maybe<int>.Nothing);
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
			Assert.IsFalse(maybeInt1.Equals(null));
		}

		[Test]
		public void JustEqualTest()
		{
			Maybe<int> maybeInt1 = 1;
			Maybe<int> maybeInt2 = 1;
			Maybe<int> maybeInt3 = 2;

			Assert.IsTrue(maybeInt1.Equals(maybeInt2));
			Assert.IsTrue(maybeInt1 == maybeInt2);
			Assert.IsTrue(maybeInt2.Equals(maybeInt1));
			Assert.IsTrue(maybeInt2 == maybeInt1);
			Assert.IsTrue(maybeInt3 != maybeInt1);
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

		[Test]
		public void NothingHasNoValue()
		{
			Maybe<string> hi = Maybe.Nothing;
			Assert.IsFalse(hi.HasValue);
			Assert.Throws<InvalidOperationException>(() =>
			{
				// ReSharper disable once UnusedVariable
				var value = hi.Value;
			});
		}

		[Test, TestCaseSource(nameof(GetEitherTestCases))]
		public Maybe<string> MaybeEitherTests(Maybe<int> left, Maybe<int> right)
		{
			return left.Either(right, l => "Left " + l, r => "Right " + r);
		}

		// ReSharper disable once UnusedMethodReturnValue.Local
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
			var foo = Maybe.Return(new Foo());
			var bar = Maybe.Return(new Bar());

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


		[Test]
		public void ReturnIfNotNullTest()
		{
			Assert.IsFalse(Maybe.ReturnIfNotNull<object>(null).HasValue);
			Assert.IsTrue(Maybe.ReturnIfNotNull(new object()).HasValue);
		}


		[Test]
		public void TryTest()
		{
			Assert.IsTrue(Maybe.Try(() => DateTime.Now).HasValue);
			Assert.IsFalse(Maybe.Try<object>(() => { throw new Exception(); }).HasValue);
		}

		[Test]
		public void FromTryOutTest()
		{
			Assert.AreEqual(Maybe.FromTryOut<int>(int.TryParse, "1"), Maybe.Just(1));
			Assert.AreEqual(Maybe.FromTryOut<int>(int.TryParse, "_"), Maybe.Nothing);
		}

		[Test]
		public void NothingIfNullTest()
		{
			object? foo = null;
			object bar = new object();
			int? i = null;
			int? j = 0;

			Assert.IsFalse(foo.NothingIfNull().HasValue);
			Assert.IsTrue(bar.NothingIfNull().HasValue);

			Assert.IsFalse(i.NothingIfNull().HasValue);
			Assert.IsTrue(j.NothingIfNull().HasValue);
		}

		[Test]
		public void DoTest()
		{
			var just = Maybe.Just(1);
			Maybe<int> nothing = Maybe.Nothing;

			int result = 0;
			var r1 = just.Do(x => result = x);
			Assert.AreEqual(result, 1);
			Assert.AreEqual(just, r1);


			bool elsePass = false;
			var r2 = nothing.Do(Fail, () => elsePass = true);
			Assert.IsTrue(elsePass);
			Assert.AreEqual(nothing, r2);
		}

		[Test]
		public void SelectTest()
		{
			Assert.AreEqual(Maybe.Just(1), Maybe.Just(2).Select(x => x / 2));
			Assert.AreEqual(Maybe.Nothing, Maybe<int>.Nothing.Select(Fail<int, int>));
		}
		[Test]
		public void WhereTest()
		{
			Assert.AreEqual(Maybe.Just(1), Maybe.Just(1).Where(x => x == 1));
			Assert.AreEqual(Maybe.Nothing, Maybe.Just(1).Where(x => x == 2));
			Assert.AreEqual(Maybe.Nothing, Maybe<int>.Nothing.Where(Fail<int, bool>));
		}

		[Test]
		public void TryGetValueTest()
		{
			var m = Maybe.Just(1);

			int i;
			Assert.IsTrue(m.TryGetValue(out i));
			Assert.AreEqual(1, i);

			var n = Maybe<int>.Nothing;

			Assert.IsFalse(n.TryGetValue(out i));
		}

		[Test]
		public void GetOrThrowTest()
		{
			var m = Maybe.Just(1);

			Assert.AreEqual(1, m.GetOrThrow(Fail<Exception>));

			Assert.AreEqual(1, m.GetOrThrow("Some Message"));

			var n = Maybe<int>.Nothing;

			Assert.That(() => n.GetOrThrow(() => new Exception("Some message")), Throws.Exception.Message.EqualTo("Some message"));
			Assert.That(() => n.GetOrThrow("Some message"), Throws.Exception.Message.EqualTo("Some message").And.InstanceOf<InvalidOperationException>());
		}

		[Test]
		public void ToTryTest()
		{
			var t1 = Maybe.Just(1).ToTry(Fail<Exception>);

			Assert.True(t1.IsSuccess);


			var t2 = Maybe<int>.Nothing.ToTry(() => new Exception("Some message"));
			Assert.IsFalse(t2.IsSuccess);

			t2.Switch(Fail, e => Assert.AreEqual("Some message", e.Message));
		}

		[Test]
		public void AnyTest()
		{
			Func<Maybe<int>> j = () => Maybe.Just(1);
			Func<Maybe<int>> n = () => Maybe.Nothing;

			Assert.AreEqual(Maybe.Any(j, n), Maybe.Just(1));
			Assert.AreEqual(Maybe.Any(n, j), Maybe.Just(1));
			Assert.AreEqual(Maybe.Any(n), Maybe.Nothing);
		}

		[Test]
		public void ToNullableTest()
		{
			Assert.AreEqual((int?)1, Maybe.Just(1).ToNullable());
			Assert.Null(Maybe<int>.Nothing.ToNullable());
		}

		[Test]
		public void GetOrElseTest()
		{
			Assert.AreEqual(1, Maybe.Just(1).GetOrElse(Fail<int>));

			Assert.AreEqual(2, Maybe<int>.Nothing.GetOrElse(() => 2));
		}

		[Test]
		public void GetOrDefaultTest()
		{
			Assert.AreEqual(1, Maybe.Just(1).GetOrDefault(2));
			Assert.AreEqual(2, Maybe<int>.Nothing.GetOrDefault(2));
		}

		[Test]
		public void GetOrDefaultCSharpNullableTest()
		{
			// Known issue: `null!` is required here
			// https://github.com/dotnet/roslyn/issues/40110
			// ReSharper disable once RedundantArgumentDefaultValue
			var value1 = Maybe<object>.Nothing.GetOrDefault(null!);
			Assert.IsNull(value1);

			var value2 = Maybe<object>.Nothing.GetOrNull();
			Assert.IsNull(value2);
		}

		[Test]
		public void OrElseTest()
		{
			Assert.AreEqual(Maybe.Just(1), Maybe.Just(1).OrElse(Fail<Maybe<int>>));

			Assert.AreEqual(Maybe.Just(2), Maybe<int>.Nothing.OrElse(() => Maybe.Just(2)));
		}

		[Test]
		public void BindTest()
		{
			Assert.AreEqual(Maybe.Just(2), Maybe.Just(1).Bind(x => Maybe.Just(x * 2)));

			Assert.AreEqual(Maybe.Nothing, Maybe<int>.Nothing.Bind(Fail<int, Maybe<int>>));
		}

		[Test]
		public void GetEnumeratorTest()
		{
			var justEnumerator = Maybe.Just(1).GetEnumerator();
			Assert.True(justEnumerator.MoveNext());
			Assert.AreEqual(1, justEnumerator.Current);
			Assert.IsFalse(justEnumerator.MoveNext());

			var nothingEnumerator = Maybe<int>.Nothing.GetEnumerator();
			Assert.False(nothingEnumerator.MoveNext());

			Assert.DoesNotThrow(() =>
			{
				foreach (var _ in Maybe<int>.Nothing)
				{
					throw new Exception();
				}
			}, "Supports structural foreach");
		}
	}
}