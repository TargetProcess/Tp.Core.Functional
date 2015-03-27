// 
// Copyright (c) 2005-2014 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections.Generic;
using System.Linq;

namespace Tp.Core
{
	public static class TryExtensions
	{
		public static Maybe<T> ToMaybe<T>(this IEnumerable<Try<T>> tries)
		{
			return tries.Select(x => x.ToMaybe())
				.Choose()
				.FirstOrNothing();
		}
	}
}
