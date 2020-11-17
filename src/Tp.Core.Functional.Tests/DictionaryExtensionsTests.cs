//
// Copyright (c) 2005-2015 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
//

using System.Collections.Generic;
using NUnit.Framework;

namespace Tp.Core.Functional.Tests
{
	[TestFixture]
	public class DictionaryExtensionsTests : TestBase
	{
		[Test]
		public void GetValueTest()
		{
#pragma warning disable 8714
			var d = new Dictionary<int?, string> { { 1, "a" } };
#pragma warning restore 8714

			AssertSome(d.GetValue(1), "a");
			AssertNothing(d.GetValue(2));
			AssertNothing(d.GetValue(null));
		}
	}
}