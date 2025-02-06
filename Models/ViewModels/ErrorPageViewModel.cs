using Umbraco.Cms.Core.Web;
using uugs2025.Models.PublishedModels;

namespace UUGS2025.Models.ViewModels
{
    public class ErrorPageViewModel : BasePageViewModel<Error>
    {
        public ErrorPageViewModel(Error content, IUmbracoContextAccessor umbracoContextAccessor) : base(content, umbracoContextAccessor)
        {
        }
    }
}