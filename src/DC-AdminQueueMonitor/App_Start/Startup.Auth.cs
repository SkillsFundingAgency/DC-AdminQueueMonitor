using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using DC_AdminQueueMonitor;
using System.IdentityModel.Tokens;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Owin.Security.WsFederation;
using System.Threading.Tasks;
using Microsoft.Owin.Security.Notifications;
using Microsoft.IdentityModel.Protocols.WsFederation;
using System.Linq;
using System.Configuration;

namespace DC_AdminQueueMonitor
{
    public partial class Startup
    {
        // For more information on configuring authentication, please visit https://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Configure the db context, user manager and signin manager to use a single instance per request
            //app.CreatePerOwinContext(ApplicationDbContext.Create);
            //app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            //app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            // Configure the sign in cookie
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "Cookies", // DefaultAuthenticationTypes.ApplicationCookie,
                //LoginPath = new PathString("/Account/Login"),
                Provider = new CookieAuthenticationProvider
                {
                    // Enables the application to validate the security stamp when the user logs in.
                    // This is a security feature which is used when you change a password or add an external login to your account.  
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
                        validateInterval: TimeSpan.FromMinutes(30),
                        regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                }                
            });
            //            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Enables the application to temporarily store user information when they are verifying the second factor in the two-factor authentication process.
            //            app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));

            // Enables the application to remember the second login verification factor such as phone or email.
            // Once you check this option, your second step of verification during the login process will be remembered on the device where you logged in from.
            // This is similar to the RememberMe option when you log in.
            //            app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);

            // Uncomment the following lines to enable logging in with third party login providers
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            //app.UseTwitterAuthentication(
            //   consumerKey: "",
            //   consumerSecret: "");

            //app.UseFacebookAuthentication(
            //   appId: "",
            //   appSecret: "");

            //app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            //{
            //    ClientId = "",
            //    ClientSecret = ""
            //});

            string realm = ConfigurationManager.AppSettings["IdentityRealm"];
            string metadata = ConfigurationManager.AppSettings["IdentityMetadata"];


//            var realm = "urn:local";// dcol-dev-queue-monitor
//            var metadata = "https://dcol-identityserver.azurewebsites.net/wsfederation";//CloudConfigurationManager.GetSetting("IdamsADFSMetadata");

            //todo: may need more options here
            var options = new WsFederationAuthenticationOptions
            {
                Wtrealm = realm,
                MetadataAddress = metadata,
                CallbackPath = new PathString("/"),
                SignInAsAuthenticationType = "Cookies",
                Notifications = new WsFederationAuthenticationNotifications
                {
                    SecurityTokenValidated = notification => SecurityTokenValidated(notification)
                }
                //,Wreply = "?"
                //,SignOutWreply = "?"
                
            };

            app.UseWsFederationAuthentication(options);
        }

        private async Task SecurityTokenValidated(SecurityTokenValidatedNotification<WsFederationMessage, WsFederationAuthenticationOptions> notification)
        {
            
//            logger.Info("SecurityTokenValidated notification called");

            ///var identity = notification.AuthenticationTicket.Identity;
            //ar IsAdmin = identity.Claims.FirstOrDefault(claim => claim.Type == "http://schemas.portal.com/usertype")?.Value == "admin";

            // usertype -- 

            //var id = identity.Claims.FirstOrDefault(claim => claim.Type == (DasClaimTypes.Upn))?.Value;
            //var displayName = identity.Claims.FirstOrDefault(claim => claim.Type == (DasClaimTypes.DisplayName))?.Value;
            //var ukprn = identity.Claims.FirstOrDefault(claim => claim.Type == (DasClaimTypes.Ukprn))?.Value;
            //var email = identity.Claims.FirstOrDefault(claim => claim.Type == (DasClaimTypes.Email))?.Value;

           // long parsedUkprn;
            //            if (!long.TryParse(ukprn, out parsedUkprn))
            //            {
            ////                logger.Info($"Unable to parse Ukprn \"{ukprn}\" from claims for user \"{id}\"");
            //                return;
            //            }

            //            await orchestrator.SaveIdentityAttributes(id, parsedUkprn, displayName, email);
        }
    }
}