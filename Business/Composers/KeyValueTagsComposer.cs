using Umbraco.Cms.Core.Composing;
using UUGS2025.Business.ValueConverters;

namespace UUGS2025.Business.Composers
{
    public class KeyValueTagComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.PropertyValueConverters().Append<KeyValueTagsValueConverter>();
        }
    }
}