using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using Umbraco.Cms.Api.Management.Security;
using UUGS2025.Business.Options;

namespace UUGS2025.Business.Extensions
{
    public static class AuthenticationExtensions
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
                                //options.Configuration = new OpenIdConnectConfiguration
                                //{
                                //    AuthorizationEndpoint = "https://login.microsoftonline.com/svensktravsport.onmicrosoft.com/v2.0",                                 
                                //    //AuthorizationEndpoint = "https://login.microsoftonline.com/ed48d7d0-b9d5-44df-9f19-f747f6e92524/oauth2/v2.0/authorize",
                                //    Issuer = "https://login.microsoftonline.com/svensktravsport.onmicrosoft.com/v2.0",
                                //    //Issuer = "https://login.microsoftonline.com/ed48d7d0-b9d5-44df-9f19-f747f6e92524/v2.0",
                                //    //TokenEndpoint = "https://login.microsoftonline.com/ed48d7d0-b9d5-44df-9f19-f747f6e92524/oauth2/v2.0/token"
                                //    TokenEndpoint = "https://login.microsoftonline.com/svensktravsport.onmicrosoft.com/v2.0/token"
                                //};

                                options.SignInScheme = "azure-cookie";
                                options.SignOutScheme = "azure-cookie";
                                options.ClientId = "457e643b-9c17-47fa-8f7e-ebe80327b2a6";
                                options.ClientSecret = "C_Z8Q~EydMJ9C4_sLBwU_oKAHG_zrxyxYdIoLcbq";

                                //options.ClientId = "7c1030c3-f1fc-4715-a9ee-7d6791325f44";
                                //options.ClientSecret = "nzk8Q~5agcQm7m85DBTx8pP-6uV3ko7gNARX8cCm";

                                options.ResponseType = OpenIdConnectResponseType.CodeIdToken;
                                options.Authority = "https://login.microsoftonline.com/svensktravsport.onmicrosoft.com/v2.0";
                                //options.Authority = "https://login.microsoftonline.com/ed48d7d0-b9d5-44df-9f19-f747f6e92524/oauth2/v2.0/authorize";
                                options.CallbackPath = "/signin-oidc";

                                options.Scope.Clear();
                                options.Scope.Add(OpenIdConnectScope.OpenId);
                                options.Scope.Add(OpenIdConnectScope.Email);

                                options.TokenValidationParameters = new TokenValidationParameters
                                {
                                    ValidateIssuer = true,
                                    RoleClaimType = ClaimTypes.Role,
                                    NameClaimType = ClaimTypes.Email,
                                    //this seems necessary despite matching all package versions
                                    SignatureValidator = (token, parameters) =>
                                    {
                                        System.Diagnostics.Debugger.Break();
                                        return new JsonWebToken(token);
                                    }
                                };

                                options.Events.OnAuthenticationFailed = (ctx) =>
                                {
                                    System.Diagnostics.Debugger.Break();
                                    return Task.FromResult(0);
                                };

                                options.Events.OnAuthorizationCodeReceived = (ctx) =>
                                {
                                    System.Diagnostics.Debugger.Break();
                                    return Task.FromResult(0);
                                };

                                options.Events.OnAccessDenied = (ctx) =>
                                {
                                    System.Diagnostics.Debugger.Break();
                                    return Task.FromResult(0);
                                };

                                options.Events.OnRemoteFailure = (ctx) =>
                                {
                                    System.Diagnostics.Debugger.Break();
                                    return Task.FromResult(0);
                                };

                                options.Events.OnMessageReceived = (ctx) =>
                                {
                                    System.Diagnostics.Debugger.Break();
                                    return Task.FromResult(0);
                                };

                                options.Events.OnRedirectToIdentityProvider = ctx =>
                                {
                                    System.Diagnostics.Debugger.Break();
                                    var redirectUri = "https://localhost:44368/signin-oidc"; // Absolute URI
                                    ctx.ProtocolMessage.RedirectUri = redirectUri;

                                    // Prevent redirect loop
                                    if (ctx.Response.StatusCode == 401)
                                    {
                                        ctx.HandleResponse();
                                    }

                                    return Task.CompletedTask;
                                };

                                options.Events.OnAuthenticationFailed = context =>
                                {
                                    System.Diagnostics.Debugger.Break();
                                    context.HandleResponse();
                                    context.Response.BodyWriter.WriteAsync(Encoding.ASCII.GetBytes(context.Exception.Message));
                                    return Task.FromResult(0);
                                };

                                options.Events.OnTokenValidated = (ctx) =>
                                {
                                    System.Diagnostics.Debugger.Break();
                                    var redirectUri = new Uri(ctx.Properties.RedirectUri, UriKind.RelativeOrAbsolute);

                                    if (redirectUri.IsAbsoluteUri)
                                    {
                                        ctx.Properties.RedirectUri = redirectUri.PathAndQuery;
                                    }

                                    if (ctx.Principal?.Identity is ClaimsIdentity claimsIdentity)
                                    {
                                        //var claims = claimsIdentity.GetClaimsFromGroupMembership();
                                        //claimsIdentity.AddClaims(claims);

                                        //todo syncronize umbraco user api
                                        //ServiceLocator.Current.GetInstance<ISynchronizingUserService>().SynchronizeAsync(claimsIdentity);
                                    }

                                    return Task.FromResult(0);
                                };
                            });
                    });
            });

            return builder;
        }
    }
}
