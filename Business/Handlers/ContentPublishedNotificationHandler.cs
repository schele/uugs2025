using static Umbraco.Cms.Core.Constants;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models.Membership;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;
using Umbraco.Cms.Core.Web;
using uugs2025.Models.PublishedModels;

namespace UUGS2025.Business.Handlers
{
    public class ContentPublishedNotificationHandler : INotificationHandler<ContentPublishedNotification>
    {
        private readonly IUserGroupService _userGroupService;
        private readonly IUmbracoContextAccessor _umbracoContextAccessor;
        private readonly IShortStringHelper _shortStringHelper;
        private readonly IBackOfficeUserManager _backOfficeUserManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMediaService _mediaService;

        public ContentPublishedNotificationHandler(IMediaService mediaService, IHttpContextAccessor httpContextAccessor, IBackOfficeUserManager backOfficeUserManager, IShortStringHelper shortStringHelper, IUserGroupService userGroupService, IUmbracoContextAccessor umbracoContextAccessor)
        {
            _userGroupService = userGroupService;
            _umbracoContextAccessor = umbracoContextAccessor;
            _shortStringHelper = shortStringHelper;
            _backOfficeUserManager = backOfficeUserManager;
            _httpContextAccessor = httpContextAccessor;
            _mediaService = mediaService;
        }

        public async void Handle(ContentPublishedNotification notification)
        {
            var content = notification.PublishedEntities.FirstOrDefault();

            if (content != null && (
                content.ContentType.Alias.Equals(nameof(GlobalSettings), StringComparison.OrdinalIgnoreCase)))
            {
            }
            else
            {
                var status = _umbracoContextAccessor.TryGetUmbracoContext(out IUmbracoContext context);
                var currentUserPrincipal = _httpContextAccessor.HttpContext?.User;

                if (status)
                {
                    var currentUser = _backOfficeUserManager.GetUserAsync(currentUserPrincipal);
                    var userGuid = currentUser.AsGuid();
                    var pageReference = context.Content.GetById(content.Id);

                    if (pageReference is Start startPage)
                    {
                        //get the name and theme of the startpage
                        var name = startPage.Name;
                        //var theme = startPage.Theme;
                        var groupName = $"{name}Editors";

                        //todo create a media folder
                        var mediaFolder = _mediaService.CreateMedia(name, -1, "Folder", currentUser.Id);
                        _mediaService.Save(mediaFolder);                        

                        var userGroup = new UserGroup(_shortStringHelper)
                        {
                            Name = groupName,
                            Alias = groupName.ToFirstLower(),
                            Icon = Icons.User,
                            CreateDate = DateTime.Now,
                            HasAccessToAllLanguages = true,
                            StartContentId = pageReference.Id,
                            StartMediaId = mediaFolder.Id,
                            Permissions = new HashSet<string>
                            {
                                "Umb.Document.Read",
                                //"Umb.Document.CreateBlueprint",
                                "Umb.Document.Delete",
                                "Umb.Document.Create",
                                //"Umb.Document.Notifications",
                                "Umb.Document.Publish",
                                //"Umb.Document.Permissions",
                                "Umb.Document.Unpublish",
                                "Umb.Document.Update",
                                "Umb.Document.Duplicate",
                                "Umb.Document.Move",
                                "Umb.Document.Sort"
                                //"Umb.Document.CultureAndHostnames",
                                //"Umb.Document.PublicAccess",
                                //"Umb.Document.Rollback"
                            }
                        };

                        userGroup.AddAllowedSection("content");
                        var result = await _userGroupService.CreateAsync(userGroup, currentUser.Result.Key);
                    }
                }
            }
        }
    }
}