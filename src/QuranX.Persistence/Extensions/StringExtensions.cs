namespace QuranX.Persistence.Extensions
{
	public static class StringExtensions
	{
		public static string AsNullIfEmpty(this string value)
		{
			if (string.IsNullOrEmpty(value))
				return null;
			return value;
		}

		public static string AsNullIfWhiteSpace(this string value)
		{
			if (string.IsNullOrWhiteSpace(value))
				return null;
			return value;
		}
	}
}
