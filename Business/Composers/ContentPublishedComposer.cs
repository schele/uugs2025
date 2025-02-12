using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Notifications;
using UUGS2025.Business.Handlers;

namespace UUGS2025.Business.Composers
{
    public class ContentPublishedComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.AddNotificationHandler<ContentPublishedNotification, ContentPublishedNotificationHandler>();
        }
    }
}