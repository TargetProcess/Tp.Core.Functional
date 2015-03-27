using System;
using System.Diagnostics;

namespace Tp.Core
{
	public struct Nothing : IEquatable<Nothing>, IMaybe
	{
		[DebuggerStepThrough]
		public override int GetHashCode()
		{
			return 0;
		}

		[DebuggerStepThrough]
		public static bool operator ==(Nothing left, Nothing right)
		{
			return Equals(left, right);
		}

		[DebuggerStepThrough]
		public static bool operator !=(Nothing left, Nothing right)
		{
			return !Equals(left, right);
		}

		public bool HasValue
		{
			[DebuggerStepThrough]
			get { return false; }
		}

		public object Value
		{
			get { throw new NotSupportedException(); }
		}

		[DebuggerStepThrough]
		public bool Equals(Nothing other)
		{
			return true;
		}

		[DebuggerStepThrough]
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (obj.GetType() != typeof(Nothing)) return false;
			return Equals((Nothing)obj);
		}

		[DebuggerStepThrough]
		public override string ToString()
		{
			return "Nothing";
		}
	}
}