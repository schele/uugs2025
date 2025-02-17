namespace UUGS2025.Business.Extensions
{
    public static class StringExtensions
    {
        public static List<string> GetUniquePrefixes(this List<string> items)
        {
            return items
                .Select(s => ExtractPrefix(s))
                .Distinct()
                .ToList();
        }

        private static string ExtractPrefix(string input)
        {
            for (int i = 1; i < input.Length; i++)
            {
                if (char.IsLower(input[i - 1]) && char.IsUpper(input[i]))
                {
                    return input.Substring(0, i);
                }
            }

            return input;
        }
    }
}
