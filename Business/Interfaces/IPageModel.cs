using Umbraco.Cms.Core.Models.PublishedContent;

namespace UUGS2025.Business.Interfaces
{
    public interface IPageModel : IPublishedContent
    {
        IPublishedContent Content { get; }
    }
}