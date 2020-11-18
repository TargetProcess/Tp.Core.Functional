using System;
using NUnit.Framework;

namespace Tp.Core.Functional.Tests
{
	[TestFixture]
	public class NullableExtensionTests : TestBase
	{
		[Test]
		public void SelectTest()
		{
			int? i = 1;
			Assert.AreEqual(2, i.Select(a => a * 2));

			int? n = null;
			Assert.IsNull(n.Select(Fail<int, int>));
		}

		[Test]
		public void BindTest()
		{
			int? i = 1;
			Assert.AreEqual(2, i.Bind(a => (int?)a * 2));
			Assert.IsNull(i.Bind(a => (int?)null));

			int? n = null;
			Assert.IsNull(n.Bind(Fail<int, int?>));
		}

		[Test]
		public void ToMaybeTest()
		{
			int? i = 1;
			AssertSome(i.ToMaybe(), 1);

			int? n = null;
			AssertNothing(n.ToMaybe());
		}

		[Test]
		public void ToNullableTest()
		{
			var i = Maybe.Just(1);
			Assert.AreEqual(1, i.ToNullable());

			var n = Maybe<int>.Nothing;
			Assert.IsNull(n.ToNullable());

		}

		[Test]
		public void ChooseTest()
		{
			var collection = new int?[] { 1, 2, null, 3 };

			var result = collection.Choose(x => x == 1 ? null : x);

			Assert.AreEqual(new int?[] { 2, 3 }, result);
		}

		[TestCase(null, null, ExpectedResult = null)]
		[TestCase(1, null, ExpectedResult = null)]
		[TestCase(null, 1u, ExpectedResult = null)]
		[TestCase(1, 1u, ExpectedResult = 2L)]
		public long? SelectManyTest(int? a, uint? b)
		{
			// ReSharper disable once PossibleInvalidOperationException
			return a.SelectMany(aa => b, (aa, bb) => (long) (aa + bb));
		}
	}
}