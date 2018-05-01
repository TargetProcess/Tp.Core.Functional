// 
// Copyright (c) 2005-2015 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using NUnit.Framework;

namespace Tp.Core.Functional.Tests
{
	[TestFixture]
	public class EihterTests : TestBase
	{
		[Test]
		public void SwitchTest()
		{
			var left = Either.CreateLeft<int, string>(1);
			var right = Either.CreateRight<int, string>("s");

		    var assertCount = 0;

			left.Switch(i =>
			{
			    Assert.AreEqual(1, i);
			    assertCount++;
			}, Fail);
			Assert.AreEqual(1, assertCount);


			right.Switch(Fail, i =>
			{
			    Assert.AreEqual("s", i);
			    assertCount++;
			});
			Assert.AreEqual(2, assertCount);


			var result1 = left.Switch(i => i * 2, Fail<string, int>);
			Assert.AreEqual(2, result1);

			var result2 = right.Switch(Fail<int, int>, s => 5);
			Assert.AreEqual(5, result2);
		}

		[Test]
		public void IfTest()
		{
			var eitherTrue = Either.If(true).Then(1).Else("s");
			Assert.AreEqual(2, eitherTrue.Switch(i => i * 2, Fail<string, int>));

			var eitherFalse = Either.If(false).Then(1).Else("s");
			Assert.AreEqual(5, eitherFalse.Switch(Fail<int, int>, s => 5));
		}
	}
}