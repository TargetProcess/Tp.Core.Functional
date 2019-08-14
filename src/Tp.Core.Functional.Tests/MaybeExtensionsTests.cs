//
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
//

using NUnit.Framework;

namespace Tp.Core.Functional.Tests
{
	[TestFixture]
	public class MaybeExtensionsTests : TestBase
	{
		[Test]
		public void StringToMaybeTest()
		{
			string nullString = null;
			string emptyString = string.Empty;
			string valueString = "abc";

			Assert.IsFalse(nullString.ToMaybe(treatEmptyAsNull: true).HasValue);
			Assert.IsFalse(nullString.ToMaybe(treatEmptyAsNull: false).HasValue);

			Assert.IsFalse(emptyString.ToMaybe(treatEmptyAsNull: true).HasValue);
			Assert.IsTrue(emptyString.ToMaybe(treatEmptyAsNull: false).HasValue);

			Assert.IsTrue(valueString.ToMaybe(treatEmptyAsNull: true).HasValue);
			Assert.IsTrue(valueString.ToMaybe(treatEmptyAsNull: false).HasValue);
		}
	}
}