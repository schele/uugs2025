using Umbraco.Cms.Api.Management.Security;
using UUGS2025.Business.Options;

namespace UUGS2025.Business.Extensions
{
    public static class MemberAuthenticationExtensions
    {
        public static IUmbracoBuilder ConfigureAuthenticationMembers(this IUmbracoBuilder builder)
        {
            builder.Services.ConfigureOptions<EntraIDB2CMembersExternalLoginProviderOptions>();
            builder.AddMemberExternalLogins(logins =>
            {
                builder.Services.ConfigureOptions<EntraIDB2CMembersExternalLoginProviderOptions>();
                builder.AddBackOfficeExternalLogins(logins =>
                {
                    logins.AddBackOfficeLogin(
                        membersAuthenticationBuilder =>
                        {
                            membersAuthenticationBuilder.AddMicrosoftAccount(

                                // The scheme must be set with this method to work for the external login.
                                BackOfficeAuthenticationBuilder.SchemeForBackOffice(EntraIDB2CMembersExternalLoginProviderOptions.SchemeName),
                                options =>
                                {
                                    // Callbackpath: Represents the URL to which the browser should be redirected to.
                                    // The default value is /signin-oidc.
                                    // This needs to be unique.
                                    options.CallbackPath = "/umbraco-b2c-members-signin";

                                    //Obtained from the ENTRA ID B2C WEB APP
                                    options.ClientId = "457e643b-9c17-47fa-8f7e-ebe80327b2a6";
                                    //Obtained from the ENTRA ID B2C WEB APP
                                    options.ClientSecret = "C_Z8Q~EydMJ9C4_sLBwU_oKAHG_zrxyxYdIoLcbq";

                                    // If you are using single-tenant app registration (e.g. for an intranet site), you must specify the Token Endpoint and Authorization Endpoint:
                                    //options.TokenEndpoint = $"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/token";
                                    //options.AuthorizationEndpoint = $"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/authorize";

                                    options.SaveTokens = true;
                                });
                        });
                });
            });

            return builder;
        }
    }
}
