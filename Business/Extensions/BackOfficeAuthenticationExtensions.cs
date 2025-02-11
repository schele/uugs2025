using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Umbraco.Cms.Api.Management.Security;
using UUGS2025.Business.Options;

namespace UUGS2025.Business.Extensions
{
    public static class BackOfficeAuthenticationExtensions
    {
        public static IUmbracoBuilder ConfigureAuthenticationUsers(this IUmbracoBuilder builder)
        {
            builder.Services.ConfigureOptions<EntraIDB2CBackOfficeExternalLoginProviderOptions>();

            builder.AddBackOfficeExternalLogins(logins =>
            {
                logins.AddBackOfficeLogin(
                    backOfficeAuthenticationBuilder =>
                    {
                        backOfficeAuthenticationBuilder.AddOpenIdConnect(
                            BackOfficeAuthenticationBuilder.SchemeForBackOffice(EntraIDB2CBackOfficeExternalLoginProviderOptions.SchemeName),
                            options =>
                            {
                                var configuration = builder.Services.BuildServiceProvider().GetRequiredService<IConfiguration>();

                                options.ClientId = configuration["AzureAd:ClientId"];
                                options.Authority = $"{configuration["AzureAd:Instance"]}/{configuration["AzureAd:TenantId"]}/v2.0";
                                options.CallbackPath = configuration["AzureAd:RedirectUri"] ?? "/signin-oidc";
                                options.ClientSecret = configuration["AzureAd:ClientSecret"];

                                //options.ResponseType = OpenIdConnectResponseType.CodeIdToken;
                                //options.UseTokenLifetime = false;
                                //options.SignInScheme = CookieAuthenticationDefaults.AuthenticationType;
                                //options.SignOutScheme = CookieAuthenticationDefaults.AuthenticationType;
                                //options.SaveTokens = true;

                                //options.Scope.Clear();
                                //options.Scope.Add(OpenIdConnectScope.OpenId);
                                //options.Scope.Add(OpenIdConnectScope.Email);

                                //options.TokenValidationParameters = new TokenValidationParameters
                                //{
                                //    ValidateIssuer = true,
                                //    ValidIssuer = options.Authority,
                                //    ValidateAudience = true,
                                //    ValidAudience = options.ClientId,
                                //    ValidateLifetime = true
                                //};

                                options.Events = new OpenIdConnectEvents
                                {
                                    OnRemoteFailure = context =>
                                    {
                                        context.HandleResponse();
                                        context.Response.Redirect("/umbraco/");
                                        return Task.CompletedTask;
                                    }
                                };
                            });
                    });
            });

            return builder;
        }
    }
}