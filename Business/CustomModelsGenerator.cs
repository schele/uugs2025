using Microsoft.Extensions.Options;
using System.Text;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Infrastructure.ModelsBuilder.Building;
using Umbraco.Cms.Infrastructure.ModelsBuilder;
using IHostingEnvironment = Umbraco.Cms.Core.Hosting.IHostingEnvironment;

namespace UUGS2025.Business
{
    public class CustomModelsGenerator : IModelsGenerator
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly OutOfDateModelsStatus _outOfDateModels;
        private readonly UmbracoServices _umbracoService;
        private ModelsBuilderSettings _config;

        public CustomModelsGenerator(UmbracoServices umbracoService, IOptionsMonitor<ModelsBuilderSettings> config,
            OutOfDateModelsStatus outOfDateModels, IHostingEnvironment hostingEnvironment)
        {
            _umbracoService = umbracoService;
            _config = config.CurrentValue;
            _outOfDateModels = outOfDateModels;
            _hostingEnvironment = hostingEnvironment;
            config.OnChange(x => _config = x);
        }

        public void GenerateModels()
        {
            var modelsDirectory = _config.ModelsDirectoryAbsolute(_hostingEnvironment);

            if (!Directory.Exists(modelsDirectory))
            {
                Directory.CreateDirectory(modelsDirectory);
            }

            foreach (var file in Directory.GetFiles(modelsDirectory, "*.generated.cs"))
            {
                File.Delete(file);
            }

            IList<TypeModel> typeModels = _umbracoService.GetAllTypes();

            var builder = new TextBuilder(_config, typeModels);

            foreach (TypeModel typeModel in builder.GetModelsToGenerate())
            {
                var sb = new StringBuilder();
                builder.Generate(sb, typeModel);
                var result = sb.ToString();

                if (typeModel.IsElement == false && typeModel.ItemType == TypeModel.ItemTypes.Content && result.Contains(": PublishedContentModel"))
                {
                    result = result
                        .Replace(": PublishedContentModel", ": BaseContentModel")
                        .Replace("using Umbraco.Extensions;", "using Umbraco.Extensions;" + Environment.NewLine + "using Umbraco.Cms.Core.Web;" + Environment.NewLine + "using UUGS2025.Models;");
                }

                var filename = Path.Combine(modelsDirectory, typeModel.ClrName + ".generated.cs");
                File.WriteAllText(filename, result);
            }

            _outOfDateModels.Clear();
        }
    }
}