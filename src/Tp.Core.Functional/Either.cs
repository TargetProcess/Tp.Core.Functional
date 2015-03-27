using System;
using System.Collections.Generic;
using System.Globalization;

namespace Tp.Core
{
	public static class Either
	{
		public static Choice If(bool condition)
		{
			return new Choice(condition);
		}

		public static Either<TLeft, TRight> CreateLeft<TLeft, TRight>(TLeft value)
		{
			return new Left<TLeft, TRight>(value);
		}

		public static Either<TLeft, TRight> CreateRight<TLeft, TRight>(TRight value)
		{
			return new Right<TLeft, TRight>(value);
		}

		public class Choice
		{
			private readonly bool _choice;
			public Choice(bool choice)
			{
				_choice = choice;
			}

			public PartialChoice<TLeft> Then<TLeft>(TLeft left)
			{
				return new PartialChoice<TLeft>(_choice, left);
			}
		}

		public class PartialChoice<TLeft>
		{
			private readonly bool _selectLeft;
			private readonly TLeft _left;
			public PartialChoice(bool selectLeft, TLeft left)
			{
				_selectLeft = selectLeft;
				_left = left;
			}

			public Either<TLeft, TRight> Else<TRight>(TRight right)
			{
				return _selectLeft ? CreateLeft<TLeft, TRight>(_left) : CreateRight<TLeft, TRight>(right);
			}
		}

		private sealed class Left<TLeft, TRight> : Either<TLeft, TRight>, IEquatable<Left<TLeft, TRight>>
		{
			private readonly TLeft _value;
			public Left(TLeft value)
			{
				_value = value;
			}

			private TLeft Value { get { return _value; } }

			public TResult Switch<TResult>(Func<TLeft, TResult> caseLeft, Func<TRight, TResult> caseRight)
			{
				return caseLeft(Value);
			}

			public void Switch(Action<TLeft> caseLeft, Action<TRight> caseRight)
			{
				caseLeft(Value);
			}

			public bool Equals(Left<TLeft, TRight> other)
			{
				if (other == this)
					return true;
				if (other == null)
					return false;
				return EqualityComparer<TLeft>.Default.Equals(this.Value, other.Value);
			}

			public override bool Equals(object obj)
			{
				return Equals(obj as Left<TLeft, TRight>);
			}

			public override int GetHashCode()
			{
				return EqualityComparer<TLeft>.Default.GetHashCode(Value);
			}

			public override string ToString()
			{
				return string.Format(CultureInfo.CurrentCulture, "Left({0})", new object[] { Value });
			}
		}

		private sealed class Right<TLeft, TRight> : Either<TLeft, TRight>, IEquatable<Right<TLeft, TRight>>
		{
			private readonly TRight _value;
			public Right(TRight value)
			{
				_value = value;
			}

			private TRight Value { get { return _value; } }

			public TResult Switch<TResult>(Func<TLeft, TResult> caseLeft, Func<TRight, TResult> caseRight)
			{
				return caseRight(Value);
			}

			public void Switch(Action<TLeft> caseLeft, Action<TRight> caseRight)
			{
				caseRight(Value);
			}

			public bool Equals(Right<TLeft, TRight> other)
			{
				if (other == this)
					return true;
				if (other == null)
					return false;
				return EqualityComparer<TRight>.Default.Equals(Value, other.Value);
			}

			public override bool Equals(object obj)
			{
				return Equals(obj as Right<TLeft, TRight>);
			}

			public override int GetHashCode()
			{
				return EqualityComparer<TRight>.Default.GetHashCode(Value);
			}

			public override string ToString()
			{
				return string.Format(CultureInfo.CurrentCulture, "Right({0})", new object[] { Value });
			}
		}
	}

	// ReSharper disable InconsistentNaming
	public interface Either<out TLeft, out TRight>
	{
		TResult Switch<TResult>(Func<TLeft, TResult> caseLeft, Func<TRight, TResult> caseRight);
		void Switch(Action<TLeft> caseLeft, Action<TRight> caseRight);
	}
	// ReSharper restore InconsistentNaming
}
