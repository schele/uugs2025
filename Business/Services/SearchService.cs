using System.Globalization;
using UUGS2025.Business.Services.Interfaces;
using UUGS2025.Models;

namespace UUGS2025.Business.Services
{
    public class SearchService : ISearchService
    {
        public List<Hit> SearchContent(string query, CultureInfo cultureInfo)
        {
            var searchResults = new List<Hit>
            {
                new() { Name = $"{query} 1", Description = "Description 1", Url = "/example1", CreateDate = DateTime.Now },
                new() { Name = $"{query} 2", Description = "Description 2", Url = "/example2", CreateDate = DateTime.Now }
            };

            return searchResults;
        }
    }
}