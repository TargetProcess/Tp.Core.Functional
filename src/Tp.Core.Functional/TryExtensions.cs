namespace Tp.Core
{
	public static class TryExtensions
	{
		public static Try<T> Flatten<T>(this Try<Try<T>> @try)
		{
			return @try.SelectMany(x => x);
		}
	}
}
