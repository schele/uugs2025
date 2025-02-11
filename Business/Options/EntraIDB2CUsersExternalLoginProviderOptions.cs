using Microsoft.Extensions.Options;
using Umbraco.Cms.Web.Common.Security;

namespace UUGS2025.Business.Options
{
    public class EntraIDB2CMembersExternalLoginProviderOptions : IConfigureNamedOptions<MemberExternalLoginProviderOptions>
    {
        public const string SchemeName = "ActiveDirectoryB2C";

        public void Configure(string? name, MemberExternalLoginProviderOptions options)
        {
            //if (name != Constants.Security.MemberExternalAuthenticationTypePrefix + SchemeName)
            //{
            //    return;
            //}

            Configure(options);
        }

        public void Configure(MemberExternalLoginProviderOptions options)
        {
            // The following options are relevant if you
            // want to configure auto-linking on the authentication.
            options.AutoLinkOptions = new MemberExternalSignInAutoLinkOptions(

                // Set to true to enable auto-linking
                autoLinkExternalAccount: true,

                // [OPTIONAL]
                // Default: The culture specified in appsettings.json.
                // Specify the default culture to create the Member as.
                // It can be dynamically assigned in the OnAutoLinking callback.
                defaultCulture: null,

                // [OPTIONAL]
                // Specify the default "IsApproved" status.
                // Must be true for auto-linking.
                defaultIsApproved: true

                // [OPTIONAL]
                // Default: "Member"
                // Specify the Member Type alias.
                //defaultMemberTypeAlias: Constants.Security.DefaultMemberTypeAlias

            )
            {
                // [OPTIONAL] Callbacks
                OnAutoLinking = (autoLinkUser, loginInfo) =>
                {
                    // Customize the Member before it's linked.
                    // Modify the Members groups based on the Claims returned
                    // in the external login info.
                },
                OnExternalLogin = (user, loginInfo) =>
                {
                    // Customize the Member before it is saved whenever they have
                    // logged in with the external provider.
                    // Sync the Members name based on the Claims returned
                    // in the external login info

                    // Returns a boolean indicating if sign-in should continue or not.
                    return true;
                }
            };
        }
    }
}