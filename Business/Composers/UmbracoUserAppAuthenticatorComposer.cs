﻿using Umbraco.Cms.Api.Management.Security;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Security;
using UUGS2025.Business.Authenticator;

namespace UUGS2025.Business.Composers
{
    public class UmbracoUserAppAuthenticatorComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            var identityBuilder = new BackOfficeIdentityBuilder(builder.Services);

            identityBuilder.AddTwoFactorProvider<UmbracoUserAppAuthenticator>(UmbracoUserAppAuthenticator.Name);

            builder.Services.Configure<TwoFactorLoginViewOptions>(UmbracoUserAppAuthenticator.Name, options =>
            {
                options.SetupViewPath = "..\\App_Plugins\\TwoFactorProviders\\twoFactorProviderGoogleAuthenticator.html";
            });
        }
    }
}