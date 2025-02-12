using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Umbraco.Cms.Api.Management.Security;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Web;

namespace UUGS2025.Business.Options
{
    public class EntraIDB2CBackOfficeExternalLoginProviderOptions : IConfigureNamedOptions<BackOfficeExternalLoginProviderOptions>
    {
        public const string SchemeName = "ActiveDirectoryB2C";
        private readonly IUmbracoContextAccessor _umbracoContextAccessor;

        public EntraIDB2CBackOfficeExternalLoginProviderOptions(IUmbracoContextAccessor umbracoContextAccessor)
        {
            _umbracoContextAccessor = umbracoContextAccessor;
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
            options.AutoLinkOptions = new ExternalSignInAutoLinkOptions(
                autoLinkExternalAccount: true,
                defaultCulture: null
            )
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
                                //var settingsPage = content.GetAtRoot().DescendantsOrSelf<GlobalSettings>().FirstOrDefault();

                                //if (settingsPage != null)
                                //{
                                //    //get claims from the global settingspage
                                //    var commonClaims = JsonConvert.DeserializeObject<List<KeyValueTagItem>>(settingsPage.Sites.RootElement.ToString());

                                //    foreach (var tagItem in commonClaims)
                                //    {
                                //        if (tagItem.Key.Contains(claimValue))
                                //        {
                                //            // If the claim exists in the Tags collection, add it to the list of existing claims
                                //            existingClaims.Add(claimValue);

                                //            foreach (var tag in tagItem.Tags)
                                //            {
                                //                if (!tags.Contains(tag)) // Check if the tag already exists in the tags list
                                //                {
                                //                    tags.Add(tag); // Add the tag only if it's not already in the list
                                //                }
                                //            }
                                //        }
                                //    }
                                //}
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
            foreach (var claim in tags)
            {
                user.AddRole(claim);
            }
        }
    }
}