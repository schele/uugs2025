using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Web;
using UUGS2025.Business.Interfaces;
using uugs2025.Models.PublishedModels;

namespace UUGS2025.Models.ViewModels
{
    public class BasePageViewModel<T> : IBasePageModel, IBaseContentModel, IPageModel where T : BaseContentModel
    {
        private readonly IUmbracoContextAccessor _umbracoContextAccessor;

        public T Content { get; set; }

        public BaseContentModel CurrentPage => Content;

        public BasePageViewModel(T content, IUmbracoContextAccessor umbracoContextAccessor)
        {
            Content = content;
            _umbracoContextAccessor = umbracoContextAccessor;
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

        public int Id => MapProperty(c => c.Id);

        public string Name => MapProperty(c => c.Name);

        public string? UrlSegment => MapProperty(c => c.UrlSegment);

        public int SortOrder => MapProperty(c => c.SortOrder);

        public int Level => MapProperty(c => c.Level);

        public string Path => MapProperty(c => c.Path);

        public int? TemplateId => MapProperty(c => c.TemplateId);

        public int CreatorId => MapProperty(c => c.CreatorId);

        public DateTime CreateDate => MapProperty(c => c.CreateDate);

        public int WriterId => MapProperty(c => c.WriterId);

        public DateTime UpdateDate => MapProperty(c => c.UpdateDate);

        public IReadOnlyDictionary<string, PublishedCultureInfo> Cultures => MapProperty(c => c.Cultures);

        public PublishedItemType ItemType => MapProperty(c => c.ItemType);

        public IPublishedContent? Parent => MapProperty(c => c.Parent);

        public IEnumerable<IPublishedContent> Children => MapProperty(c => c.Children);

        public IEnumerable<IPublishedContent> ChildrenForAllCultures => MapProperty(c => c.ChildrenForAllCultures);

        public IPublishedContentType ContentType => MapProperty(c => c.ContentType);

        public Guid Key => MapProperty(c => c.Key);

        public IEnumerable<IPublishedProperty> Properties => MapProperty(c => c.Properties);

        IPublishedContent IPageModel.Content => Content;

        public bool IsDraft(string? culture = null)
        {
            return Content.IsDraft();
        }

        public bool IsPublished(string? culture = null)
        {
            return Content.IsPublished();
        }

        public IPublishedProperty? GetProperty(string alias)
        {
            return Content.GetProperty(alias);
        }

        private TProperty MapProperty<TProperty>(Func<IPublishedContent, TProperty> selector)
        {
            return selector(Content);
        }
    }
}