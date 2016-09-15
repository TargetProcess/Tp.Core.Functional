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
			else
			{
				TVal result;
				return dictionary.TryGetValue(key, out result) ? Maybe.Return(result) : Maybe<TVal>.Nothing;
			}
		}
	}
}