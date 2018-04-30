using Tp.Core;

// ReSharper disable once CheckNamespace

namespace System.Collections.Generic
{
	public static class DictionaryExtensions
	{
		public static Maybe<TVal> GetValue<TKey, TVal>(this IDictionary<TKey, TVal> d, TKey k)
		{
			if (k == null)
			{
				return Maybe.Nothing;
			}

			// Don't use FromTryOut here as it's 10x slower than direct call to d.TryGetValue
			TVal val;
			return d.TryGetValue(k, out val) ? Maybe.Just(val) : Maybe<TVal>.Nothing;
		}
	}
}