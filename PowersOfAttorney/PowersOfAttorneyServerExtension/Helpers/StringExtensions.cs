namespace PowersOfAttorneyServerExtension.Helpers
{
    internal static class StringExtensions
    {
        public static string AsNullable(this string content)
        {
            return string.IsNullOrEmpty(content.Trim()) ? null : content;
        }
    }
}