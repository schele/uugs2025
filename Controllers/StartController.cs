using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.Controllers;
using uugs2025.Models.PublishedModels;
using UUGS2025.Models.ViewModels;

namespace UUGS2025.Controllers
{
    public class StartController : RenderController
    {
        private readonly IUmbracoContextAccessor _umbracoContextAccessor;

        public StartController(ILogger<RenderController> logger, ICompositeViewEngine compositeViewEngine, IUmbracoContextAccessor umbracoContextAccessor) : base(logger, compositeViewEngine, umbracoContextAccessor)
        {
            _umbracoContextAccessor = umbracoContextAccessor;
        }

        public override IActionResult Index()
        {
            var startPage = CurrentPage as Start;

            if (startPage != null)
            {
                var model = new StartPageViewModel(startPage, _umbracoContextAccessor);

                return CurrentTemplate(model);
            }

            return null;
        }
    }
}