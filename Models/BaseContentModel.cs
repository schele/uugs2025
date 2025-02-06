using System.Globalization;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Web;
using uugs2025.Models.PublishedModels;

namespace UUGS2025.Models
{
    public class BaseContentModel : PublishedContentModel
    {
        private readonly IUmbracoContextAccessor _umbracoContextAccessor;

        public BaseContentModel(IPublishedContent content, IPublishedValueFallback publishedValueFallback) : base(content, publishedValueFallback)
        {
            _umbracoContextAccessor = StaticServiceProvider.Instance.GetService<IUmbracoContextAccessor>();
        }

        public CultureInfo CurrentCultureInfo()
        {
            var cultureName = CultureInfo.CurrentCulture.Name;
			var cultureInfo = new CultureInfo(cultureName);

			return cultureInfo;
		}

        public Start StartPage
        {
            get
            {
                if (_umbracoContextAccessor.TryGetUmbracoContext(out var umbracoContext))
                {
                    var content = umbracoContext.Content;

                    return content.GetAtRoot().DescendantsOrSelf<Start>().FirstOrDefault();
                }

                return null;
            }
        }
    }
}