using System;
using System.Linq;
using NUnit.Framework;

namespace Tp.Core.Functional.Tests
{
	[TestFixture]
	public class MaybeEnumerableTests : TestBase
	{
		[Test]
		public void NothingIfEmptyTest()
		{
			var empty = new int[0];
			var nonEmpty = new int[1];

			AssertNothing(empty.NothingIfEmpty());
			AssertSome(nonEmpty.NothingIfEmpty(), nonEmpty);
		}

		[Test]
		public void FirstOrNothingTest()
		{
			var empty = new int[0];
			var nonEmpty = new[] { 1, 2, 1, 2 };

			AssertNothing(empty.FirstOrNothing());
			AssertNothing(empty.FirstOrNothing(Fail<int, bool>));

			AssertSome(nonEmpty.FirstOrNothing(), 1);
			AssertSome(nonEmpty.FirstOrNothing(x => x == 2), 2);

			AssertNothing(nonEmpty.FirstOrNothing(x => false));
		}

		[Test]
		public void SingleOrNothingTest()
		{
			var empty = new int[0];
			var nonEmpty = new[] { 1, 2, 1, 2 };

			AssertNothing(empty.SingleOrNothing());
			AssertNothing(empty.SingleOrNothing(Fail<int, bool>));
			AssertNothing(empty.SingleOrNothing(throwOnSeveral: false));
			AssertNothing(empty.SingleOrNothing(Fail<int, bool>, throwOnSeveral: false));

			Assert.Throws<InvalidOperationException>(() => nonEmpty.SingleOrNothing(throwOnSeveral: true));
			AssertNothing(nonEmpty.SingleOrNothing(throwOnSeveral: false));

			Assert.Throws<InvalidOperationException>(() => nonEmpty.SingleOrNothing(x => x == 2, throwOnSeveral: true));
			AssertNothing(nonEmpty.SingleOrNothing(x => x == 2, throwOnSeveral: false));
		}

		[Test]
		public void SequenceTest()
		{
			var withNothing = new[] { Maybe.Just(1), Maybe.Nothing };
			var withoutNothing = new[] { Maybe.Just(1), Maybe.Just(2) };
			var emtpy = new Maybe<int>[0];

			AssertNothing(withNothing.Sequence());
			AssertSome(withoutNothing.Sequence(), new[] { 1, 2 });

			AssertSome(emtpy.Sequence(), new int[0]);
		}

		[Test]
		public void ToEnumerableTest()
		{
			Assert.IsEmpty(Maybe<int>.Nothing.ToEnumerable());
			Assert.AreEqual(new[] { 1 }, Maybe.Just(1).ToEnumerable());
		}

		[Test]
		public void ChooseTest()
		{
			var withNothing = new[] { Maybe.Just(1), Maybe.Nothing };
			var withoutNothing = new[] { Maybe.Just(1), Maybe.Just(2) };
			var onlyNothing = new Maybe<int>[] { Maybe.Nothing };
			var emtpy = new Maybe<int>[0];


			Assert.AreEqual(new[] { 1 }, withNothing.Choose());
			Assert.AreEqual(new[] { 1, 2 }, withoutNothing.Choose());
			Assert.IsEmpty(onlyNothing.Choose());
			Assert.IsEmpty(emtpy.Choose());
		}

		[Test]
		public void Choose2Test()
		{
			var collection = new[] { "1", "2" };

			Assert.AreEqual(collection.Choose(x => x == "1" ? Maybe.Just(1) : Maybe.Nothing), new[] { 1 });
			Assert.IsEmpty(collection.Choose(x => Maybe<string>.Nothing));
		}

		[Test]
		public void ChooseWithSelectorTest()
		{
			var collection = new[] {1, 2, 3};
			var result = collection.Choose(
				input => input == 1 ? Maybe.Just(input.ToString()) : Maybe.Nothing,
				(input, v) => new {Input = input, Output = v}).ToList();

			Assert.AreEqual(new[] {new {Input = 1, Output = "1"}}, result);

			Assert.IsEmpty(collection.Choose<int, string, string>(
				x => Maybe<string>.Nothing,
				(a, b) => { throw new Exception("This should never be thrown"); }));
		}

		[Test]
		public void SelectManyTest()
		{
			var collection = new[] { "1", "2" };

			var result = collection.SelectMany(x => x == "1" ? Maybe.Just(1) : Maybe.Nothing, (s, i) => string.Format("{0}_{1}", s, i));

			Assert.AreEqual(new[] { Maybe.Just("1_1"), Maybe<string>.Nothing }, result);

			var q = from x in collection
					from m in (x == "1" ? Maybe.Just(1) : Maybe.Nothing)
					select x + "_" + m;

			Assert.AreEqual(new[] { Maybe.Just("1_1"), Maybe<string>.Nothing }, q);
		}

		[Test]
		public void SelectMany2Test()
		{
			{
				var some = Maybe.Just(1);


				var result = some.SelectMany(x => x == 1 ? Maybe.Just(2) : Maybe.Nothing, (s, i) => string.Format("{0}_{1}", s, i));

				AssertSome(result, "1_2");

				var q = from x in some
						from m in x == 1 ? Maybe.Just(2) : Maybe.Nothing
						select x + "_" + m;

				AssertSome(q, "1_2");
			}
			{
				var nothing = Maybe<int>.Nothing;
				var result = nothing.SelectMany(x => x == 1 ? Maybe.Just(2) : Maybe.Nothing,
					(s, i) => string.Format("{0}_{1}", s, i));

				AssertNothing(result);

				var q = from x in nothing
						from m in x == 1 ? Maybe.Just(2) : Maybe.Nothing
						select x + "_" + m;

				AssertNothing(q);
			}

			{
				var some = Maybe.Just(2);
				var result = some.SelectMany(x => x == 1 ? Maybe.Just(2) : Maybe.Nothing,
					(s, i) => string.Format("{0}_{1}", s, i));

				AssertNothing(result);

				var q = from x in some
						from m in x == 1 ? Maybe.Just(2) : Maybe.Nothing
						select x + "_" + m;

				AssertNothing(q);

			}
		}

		[Test]
		public void SelectMany3Test()
		{
			{
				var some = Maybe.Just(1);

				var result = some.SelectMany(x => x == 1 ? new[] { 1, 2 } : new[] { 2, 3 }, (i, i1) => string.Format("{0}_{1}", i, i1));

				AssertSome(result, new[] { "1_1", "1_2" });

				var q = from x in some
						from y in x == 1 ? new[] { 1, 2 } : new[] { 2, 3 }
						select string.Format("{0}_{1}", x, y);

				AssertSome(q, new[] { "1_1", "1_2" });
			}
			{
				var some = Maybe<int>.Nothing;

				var result = some.SelectMany(x => x == 1 ? new[] { 1, 2 } : new[] { 2, 3 }, (i, i1) => string.Format("{0}_{1}", i, i1));

				AssertNothing(result);

				var q = from x in some
						from y in x == 1 ? new[] { 1, 2 } : new[] { 2, 3 }
						select string.Format("{0}_{1}", x, y);

				AssertNothing(q);
			}
		}

		[Test]
		public void BindTest()
		{
			var collection = new[] { Maybe.Just(1), Maybe.Nothing, Maybe.Just(2) };

			var result = collection.Bind(x => x == 1 ? Maybe.Just(3) : Maybe.Nothing);


			Assert.AreEqual(result, new[] { Maybe.Just(3), Maybe.Nothing, Maybe<int>.Nothing });
		}

	}
}