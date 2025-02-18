using Google.Authenticator;
using System.Runtime.Serialization;
using Umbraco.Cms.Core.Models.Membership;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;

namespace UUGS2025.Business.Authenticator
{
    [DataContract]
    public class TwoFactorAuthInfo : ISetupTwoFactorModel
    {
        [DataMember(Name = "qrCodeSetupImageUrl")]
        public string? QrCodeSetupImageUrl { get; set; }

        [DataMember(Name = "secret")]
        public string? Secret { get; set; }
    }

    public class UmbracoUserAppAuthenticator : ITwoFactorProvider
    {
        /// <summary>
        /// The unique name of the ITwoFactorProvider. This is saved in a constant for reusability.
        /// </summary>
        public const string Name = "UmbracoUserAppAuthenticator";

        private readonly IUserService _userService;

        /// <summary>
        /// Initializes a new instance of the <see cref="UmbracoUserAppAuthenticator"/> class.
        /// </summary>
        public UmbracoUserAppAuthenticator(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Gets the unique provider name of ITwoFactorProvider implementation.
        /// </summary>
        /// <remarks>
        /// This value will be saved in the database to connect the member with this  ITwoFactorProvider.
        /// </remarks>
        public string ProviderName => Name;

        /// <summary>
        /// Returns the required data to setup this specific ITwoFactorProvider implementation. In this case it will contain the url to the QR-Code and the secret.
        /// </summary>
        /// <param name="userOrMemberKey">The key of the user or member</param>
        /// <param name="secret">The secret that ensures only this user can connect to the authenticator app</param>
        /// <returns>The required data to setup the authenticator app</returns>
        public Task<ISetupTwoFactorModel> GetSetupDataAsync(Guid userOrMemberKey, string secret)
        {
            IUser? user = _userService.GetByKey(userOrMemberKey);

            ArgumentNullException.ThrowIfNull(user);

            var applicationName = "My application name";
            var twoFactorAuthenticator = new TwoFactorAuthenticator();
            SetupCode setupInfo = twoFactorAuthenticator.GenerateSetupCode(applicationName, user.Username, secret, false);
            return Task.FromResult<ISetupTwoFactorModel>(new TwoFactorAuthInfo()
            {
                QrCodeSetupImageUrl = setupInfo.QrCodeSetupImageUrl,
                Secret = secret
            });
        }

        /// <summary>
        /// Validated the code and the secret of the user.
        /// </summary>
        public bool ValidateTwoFactorPIN(string secret, string code)
        {
            var twoFactorAuthenticator = new TwoFactorAuthenticator();
            return twoFactorAuthenticator.ValidateTwoFactorPIN(secret, code);
        }

        /// <summary>
        /// Validated the two factor setup
        /// </summary>
        /// <remarks>Called to confirm the setup of two factor on the user. In this case we confirm in the same way as we login by validating the PIN.</remarks>
        public bool ValidateTwoFactorSetup(string secret, string token) => ValidateTwoFactorPIN(secret, token);
    }
}