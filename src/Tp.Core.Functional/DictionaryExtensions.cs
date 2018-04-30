using Tp.Core;

// ReSharper disable once CheckNamespace

namespace System.Collections.Generic
{
	public static class DictionaryExtensions
	{
		public static Maybe<TVal> GetValue<TKey, TVal>(this IDictionary<TKey, TVal> dictionary, TKey key)
		{
			if (key == null)
			{
				return Maybe<TVal>.Nothing;
			}

			// Don't use FromTryOut here as it's 10x slower than direct call to d.TryGetValue
			TVal val;
			return dictionary.TryGetValue(key, out val) ? Maybe.Just(val) : Maybe<TVal>.Nothing;
		}
	}
}
