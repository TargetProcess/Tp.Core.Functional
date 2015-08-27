using Tp.Core;

// ReSharper disable once CheckNamespace

namespace System.Collections.Generic
{
	public static class DictionaryExtensions
	{
		public static Maybe<TVal> GetValue<TKey, TVal>(this IDictionary<TKey, TVal> d, TKey k)
		{
			return k == null ? Maybe.Nothing : Maybe.FromTryOut<TKey, TVal>(d.TryGetValue, k);
		}
	}
}