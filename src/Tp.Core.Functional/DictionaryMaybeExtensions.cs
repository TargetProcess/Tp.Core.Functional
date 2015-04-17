using Tp.Core;

// ReSharper disable once CheckNamespace

namespace System.Collections.Generic
{
	public static class DictionaryMaybeExtensions
	{
		public static Maybe<TVal> GetValue<TKey, TVal>(this IDictionary<TKey, TVal> d, TKey k)
		{
			if (k == null)
				return Maybe.Nothing;
			return Maybe.FromTryOut<TKey, TVal>(d.TryGetValue, k);
		}
	}
}