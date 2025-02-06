using System.Globalization;
using UUGS2025.Models;

namespace UUGS2025.Business.Services.Interfaces
{
    public interface ISearchService
    {
        List<Hit> SearchContent(string query, CultureInfo cultureInfo);
    }
}