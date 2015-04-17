using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Tp.Core
{
	public class MaybeDictionary<TKey, TValue> : IDictionary<TKey, Maybe<TValue>>
	{
		private readonly Dictionary<TKey, TValue> _storage;

		public MaybeDictionary(IDictionary<TKey, TValue> other = null, IEqualityComparer<TKey> comparer = null)
		{
			_storage = other == null ? new Dictionary<TKey, TValue>(comparer) : new Dictionary<TKey, TValue>(other, comparer);
		}

		private IDictionary<TKey, TValue> Storage
		{
			get { return _storage; }
		}

		public IEnumerator<KeyValuePair<TKey, Maybe<TValue>>> GetEnumerator()
		{
			return _storage.Select(x => new KeyValuePair<TKey, Maybe<TValue>>(x.Key, Maybe.Just(x.Value))).GetEnumerator();
		}

		public void Add(KeyValuePair<TKey, Maybe<TValue>> item)
		{
			Unbox(item).Do(Storage.Add);
		}

		public void Clear()
		{
			throw new NotImplementedException();
		}

		public bool Contains(KeyValuePair<TKey, Maybe<TValue>> item)
		{
			return Unbox(item).Select(Storage.Contains).GetOrDefault();
		}

		public void CopyTo(KeyValuePair<TKey, Maybe<TValue>>[] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}

		public bool Remove(KeyValuePair<TKey, Maybe<TValue>> item)
		{
			return Unbox(item).Select(Storage.Remove).GetOrDefault();
		}

		public int Count
		{
			get { throw new NotImplementedException(); }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#region Implementation of IDictionary<TKey,Maybe<TValue>>

		public bool ContainsKey(TKey key)
		{
			throw new NotImplementedException();
		}

		public void Add(TKey key, Maybe<TValue> value)
		{
			value.Do(v => _storage.Add(key, v));
		}

		public bool Remove(TKey key)
		{
			throw new NotImplementedException();
		}

		public bool TryGetValue(TKey key, out Maybe<TValue> maybeValue)
		{
			maybeValue = _storage.GetValue(key);
			return true;
		}

		public Maybe<TValue> this[TKey key]
		{
			get { return _storage.GetValue(key); }
			set { value.Do(v => _storage.Add(key, v)); }
		}

		ICollection<TKey> IDictionary<TKey, Maybe<TValue>>.Keys
		{
			get { return _storage.Keys; }
		}

		public ICollection<Maybe<TValue>> Values
		{
			get { return _storage.Values.Select(Maybe.Just).ToList().AsReadOnly(); }
		}

		#endregion

		private static Maybe<KeyValuePair<TKey, TValue>> Unbox(KeyValuePair<TKey, Maybe<TValue>> pair)
		{
			return pair.Value.Select(v => new KeyValuePair<TKey, TValue>(pair.Key, v));
		}
	}
}