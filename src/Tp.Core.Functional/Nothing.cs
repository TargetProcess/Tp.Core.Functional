using System;

namespace Tp.Core
{
	public struct Nothing : IEquatable<Nothing>, IMaybe
	{
		public override int GetHashCode() => 0;

		public static bool operator ==(Nothing left, Nothing right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(Nothing left, Nothing right)
		{
			return !Equals(left, right);
		}

		public bool HasValue => false;

		public object Value => throw new NotSupportedException();

		public bool Equals(Nothing other) => true;

		public override bool Equals(object? obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			return obj is Nothing || obj.MaybeAs<IMaybe>().Select(x => !x.HasValue).GetOrDefault();
		}

		public override string ToString() => "Nothing";
	}
}