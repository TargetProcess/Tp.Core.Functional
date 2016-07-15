using System;

namespace Tp.Core
{
	public struct Nothing : IEquatable<Nothing>, IMaybe
	{
		public override int GetHashCode() => 0;
		public bool HasValue => false;
		public bool Equals(Nothing other) => true;
		public override string ToString() => "Nothing";
		public static bool operator ==(Nothing left, Nothing right) => true;
		public static bool operator !=(Nothing left, Nothing right) => false;

		public object Value
		{
			get { throw new NotSupportedException(); }
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
			{
				return false;
			}

			if (obj is Nothing)
			{
				return true;
			}

			return obj is IMaybe && !((IMaybe) obj).HasValue;
		}
	}
}