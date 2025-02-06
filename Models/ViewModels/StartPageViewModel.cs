using Umbraco.Cms.Core.Web;
using uugs2025.Models.PublishedModels;

namespace UUGS2025.Models.ViewModels
{
    public class StartPageViewModel : BasePageViewModel<Start>
    {
        public StartPageViewModel(Start content, IUmbracoContextAccessor umbracoContextAccessor) : base(content, umbracoContextAccessor)
        {
        }
    }
}