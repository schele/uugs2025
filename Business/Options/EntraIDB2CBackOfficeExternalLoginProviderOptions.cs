using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Polly;
using System.Globalization;
using System.Security.Claims;
using System.Text.RegularExpressions;
using Umbraco.Cms.Api.Management.Security;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models.Membership;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common;
using Umbraco.Extensions;
using uugs2025.Models.PublishedModels;
using UUGS2025.Business.Extensions;
using UUGS2025.Models.CustomProperties;

namespace UUGS2025.Business.Options
{
    public class EntraIDB2CBackOfficeExternalLoginProviderOptions : IConfigureNamedOptions<BackOfficeExternalLoginProviderOptions>
    {
        public const string SchemeName = "ActiveDirectoryB2C";
        private readonly IUmbracoContextAccessor _umbracoContextAccessor;
        private readonly Lazy<IUserService> _userService;
        private readonly Lazy<IUserGroupService> _userGroupService;        

        public EntraIDB2CBackOfficeExternalLoginProviderOptions(IUmbracoContextAccessor umbracoContextAccessor, Lazy<IUserService> userService, Lazy<IUserGroupService> userGroupService)
        {
            _umbracoContextAccessor = umbracoContextAccessor;
            _userService = userService;
            _userGroupService = userGroupService;
        }

        public void Configure(string? name, BackOfficeExternalLoginProviderOptions options)
        {
            if (name != Constants.Security.BackOfficeExternalAuthenticationTypePrefix + SchemeName)
            {
                return;
            }

            Configure(options);
        }

        public void Configure(BackOfficeExternalLoginProviderOptions options)
        {
            options.AutoLinkOptions = new ExternalSignInAutoLinkOptions(autoLinkExternalAccount: true, defaultCulture: null)
            {
                OnExternalLogin = (user, loginInfo) =>
                {
                    var tags = new List<string>();

                    foreach (var item in loginInfo.Principal.Claims)
                    {
                        var claimValue = item.Value;
                        var existingClaims = new List<string>();

                        if (_umbracoContextAccessor.TryGetUmbracoContext(out var umbracoContext))
                        {
                            var content = umbracoContext.Content;

                            if (content != null)
                            {
                                var settingsPage = content.GetAtRoot().DescendantsOrSelf<GlobalSettings>().FirstOrDefault();

                                if (settingsPage != null)
                                {
                                    if (settingsPage.Sites != null)
                                    {
                                        //get claims from the global settingspage
                                        var commonClaims = JsonConvert.DeserializeObject<List<KeyValueTagItem>>(settingsPage.Sites.RootElement.ToString());

                                        if (commonClaims != null)
                                        {
                                            foreach (var tagItem in commonClaims)
                                            {
                                                if (tagItem.Key.Contains(claimValue))
                                                {
                                                    // If the claim exists in the Tags collection, add it to the list of existing claims
                                                    existingClaims.Add(claimValue);

                                                    foreach (var tag in tagItem.Tags)
                                                    {
                                                        if (!tags.Contains(tag)) // Check if the tag already exists in the tags list
                                                        {
                                                            tags.Add(tag); // Add the tag only if it's not already in the list
                                                        }
                                                    }
                                                }
                                            }
                                        }                                        
                                    }                                    
                                }
                            }
                        }
                    }

                    SyncUserGroups(user, tags);

                    return true;
                }
            };
        }

        private void SyncUserGroups(BackOfficeIdentityUser user, List<string> tags)
        {
            //add roles
            foreach (var claim in tags)
            {
                user.AddRole(claim);
            }

            //approve user
            user.IsApproved = true;

            //add start pages and media folders
            var nodeIds = new List<int>();
            var mediaIds = new List<int>();
            var startPages = tags.GetUniquePrefixes();

            foreach (var item in startPages)
            {
                var nodeId = GetNodeIdByName(item);
                var mediaId = GetMediaFolderIdByName(item);

                if (nodeId.HasValue)
                {
                    nodeIds.Add(nodeId.Value);
                }

                if (mediaId.HasValue)
                {
                    mediaIds.Add(mediaId.Value);
                }
            }

            user.StartContentIds = [.. nodeIds];
            user.StartMediaIds = [.. mediaIds];

            //add groups
            var validGroups = _userGroupService.Value
                .GetAllAsync(0, 100).Result.Items
                .Where(g => tags.Any(tag => tag.Equals(g.Alias, StringComparison.OrdinalIgnoreCase)))
                .Cast<IReadOnlyUserGroup>()
                .ToList();

            user.SetGroups(validGroups);
        }

        public int? GetNodeIdByName(string nodeName)
        {
            if (_umbracoContextAccessor.TryGetUmbracoContext(out var umbracoContext))
            {
                var content = umbracoContext.Content;

                var node = content?.GetAtRoot()
                    .SelectMany(x => x.DescendantsOrSelf())
                    .FirstOrDefault(x => x.Name.Equals(nodeName, StringComparison.OrdinalIgnoreCase));

                return node?.Id;
            }

            return null;
        }

        public int? GetMediaFolderIdByName(string folderName)
        {
            if (_umbracoContextAccessor.TryGetUmbracoContext(out var umbracoContext))
            {
                var media = umbracoContext.Media;

                var folder = media.GetAtRoot()
                    .SelectMany(x => x.DescendantsOrSelf())
                    .FirstOrDefault(x => x.Name.Equals(folderName, StringComparison.OrdinalIgnoreCase));

                return folder?.Id;
            }

            return null;
        }
    }
}