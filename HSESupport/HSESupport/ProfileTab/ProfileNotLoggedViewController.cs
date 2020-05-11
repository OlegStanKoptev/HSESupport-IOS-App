using Foundation;
using Microsoft.Identity.Client;
using System;
using System.Linq;
using UIKit;

namespace HSESupport
{
    public partial class ProfileNotLoggedViewController : UIViewController
    {
        public ProfileNotLoggedViewController (IntPtr handle) : base (handle)
        {
        }
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
        }

        async partial void AuthButtonPressed(UIButton sender)
        {
            string[] scopes = new string[] { "https://hsesupportapp.onmicrosoft.com/demoapi/read" };

            string clientId = "d5410d7a-88c3-4d22-a8e7-c1b9ed9ee730";

            var app = PublicClientApplicationBuilder.Create(clientId)
                .WithB2CAuthority("https://hsesupportapp.b2clogin.com/tfp/hsesupportapp.onmicrosoft.com/B2C_1_signUpSignIn")
                .WithIosKeychainSecurityGroup("com.koptevcompany.hsesupportapp")
                .WithRedirectUri($"msal{clientId}://auth")
                .Build();
            var accounts = await app.GetAccountsAsync();
            AuthenticationResult result;
            try
            {
                result = await app.AcquireTokenSilent(scopes, accounts.FirstOrDefault())
                            .ExecuteAsync();
            }
            catch (MsalUiRequiredException)
            {
            result = await app.AcquireTokenInteractive(scopes)
                            .ExecuteAsync();
            }

        }
    }
}