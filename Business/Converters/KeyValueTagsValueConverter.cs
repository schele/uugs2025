using Newtonsoft.Json;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using UUGS2025.Models.CustomProperties;

namespace UUGS2025.Business.Converters
{
    public class KeyValueTagsValueConverter : PropertyValueConverterBase
    {
        private readonly ILogger<KeyValueTagsValueConverter> _logger;

        public KeyValueTagsValueConverter(ILogger<KeyValueTagsValueConverter> logger)
        {
            _logger = logger;
        }

        public override bool IsConverter(IPublishedPropertyType propertyType) => propertyType.EditorAlias.Equals("keyValueTags");

        public override Type GetPropertyValueType(IPublishedPropertyType propertyType) => typeof(IEnumerable<KeyValueTagItem>);

        public override PropertyCacheLevel GetPropertyCacheLevel(IPublishedPropertyType propertyType) => PropertyCacheLevel.Element;

        public override object? ConvertSourceToIntermediate(IPublishedElement owner, IPublishedPropertyType propertyType, object? source, bool preview)
        {
            if (source == null) return null;

            var sourceString = source.ToString();

            if (string.IsNullOrWhiteSpace(sourceString)) return Enumerable.Empty<KeyValueTagItem>();

            try
            {
                return JsonConvert.DeserializeObject<IEnumerable<KeyValueTagItem>>(sourceString);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);

                return Enumerable.Empty<KeyValueTagItem>();
            }
        }
    }
}