namespace UUGS2025.Models.CustomProperties
{
    public class KeyValueTagItem
    {
        public string Title { get; set; } = string.Empty;

        public string Key { get; set; } = string.Empty;

        public IEnumerable<string> Tags { get; set; } = [];
    }
}