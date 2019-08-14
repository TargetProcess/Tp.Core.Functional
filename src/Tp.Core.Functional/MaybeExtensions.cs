namespace Tp.Core
{
	public static class MaybeExtensions
	{
		public static Maybe<string> ToMaybe(this string str, bool treatEmptyAsNull = true)
		{
			if (str == null || treatEmptyAsNull && string.IsNullOrEmpty(str))
			{
				return Maybe<string>.Nothing;
			}

			return Maybe.Just(str);
		}
	}
}
