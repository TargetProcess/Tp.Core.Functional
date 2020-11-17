namespace Tp.Core
{
	public interface IMaybe
	{
		bool HasValue { get; }
		object? Value { get; }
	}
}