using System.Text;
using System;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using Google.Apis.Auth.OAuth2.Responses;
using static Google.Apis.Auth.OAuth2.Web.AuthorizationCodeWebApp;
using System.Linq;
using Stardust.Flux.DataAccess;
using Stardust.Flux.AuthenticateService;
using Stardust.Flux.Server.Options;

namespace Stardust.Flux.Server.Services.Youtube

{
    public class AuthentificationService
    {

        private readonly YoutubeApiOptions apiOptions;
        private readonly DataContext publishContext;
        private readonly ITempStorage tempStorage;

        public AuthentificationService(IOptions<YoutubeApiOptions> options, DataContext publishContext, ITempStorage tempStorage)
        {
            apiOptions = options.Value;
            this.publishContext = publishContext;
            this.tempStorage = tempStorage;
        }

        public void CheckForAccount(string accountId)
        {
            if (!publishContext.YoutubeAccounts.Any(x => x.Key == accountId))
            {
                throw new NoAccountFoundException(accountId);
            }
        }

        public async Task<UserCredential> GetCredential(string description, string accountId, string[] scopes, CancellationToken cancellationToken)
        {

            var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
               new ClientSecrets { ClientId = apiOptions.ClientId, ClientSecret = apiOptions.ClientSecrets },
               scopes,
                accountId ?? Guid.NewGuid().ToString(), cancellationToken, new EFDataStore(publishContext));
            if (credential.Token.IsExpired(Google.Apis.Util.SystemClock.Default))
            {
                await credential.RefreshTokenAsync(CancellationToken.None);
            }

            return credential;
        }

        public async Task<AuthResult> AuthorizeAsync(string userId, CancellationToken taskCancellationToken, params string[] scopes)
        {
            var flow = GetGoogleAuthorizationCodeFlow(null, scopes);
            // Try to load a token from the data store.
            var token = await flow.LoadTokenAsync(userId, taskCancellationToken).ConfigureAwait(false);

            // Check if a new authorization code is needed.
            if (ShouldRequestAuthorizationCode(token, flow))
            {
                throw new InvalidOperationException($"{userId} is not authorized for {string.Join(',', scopes)}");
            }

            var credential = new UserCredential(flow, userId, token);

            if (credential.Token.IsExpired(Google.Apis.Util.SystemClock.Default))
            {
                var refreshResult = await credential.RefreshTokenAsync(CancellationToken.None);
            }

            return new AuthResult { Credential = credential };
        }

        /// <summary>
        /// Determines the need for retrieval of a new authorization code, based on the given token and the 
        /// authorization code flow.
        /// </summary>
        public static bool ShouldRequestAuthorizationCode(TokenResponse token, IAuthorizationCodeFlow flow)
        {
            // If the flow includes a parameter that requires a new token, if the stored token is null or it doesn't
            // have a refresh token and the access token is expired we need to retrieve a new authorization code.
            return flow.ShouldForceTokenRetrieval() || token == null || token.RefreshToken == null
                && token.IsExpired(flow.Clock);
        }


        // This is a method we'll use to obtain the authorization code flow
        public AuthorizationCodeFlow GetGoogleAuthorizationCodeFlow(string description, params string[] scopes)
        {

            var clientSecrets = new ClientSecrets { ClientId = apiOptions.ClientId, ClientSecret = apiOptions.ClientSecrets };
            var initializer = new GoogleAuthorizationCodeFlow.Initializer { ClientSecrets = clientSecrets, Scopes = scopes, DataStore = new EFDataStore(publishContext, description) };

            var googleAuthorizationCodeFlow = new GoogleAuthorizationCodeFlow(initializer);
            return googleAuthorizationCodeFlow;
        }

        // HttpContext context, string userId, 
        public string GetAuthorizationUrl(string id, string description, params string[] scopes)
        {
            // Now, let's grab the AuthorizationCodeFlow that will generate a unique authorization URL to redirect our user to
            var googleAuthorizationCodeFlow = GetGoogleAuthorizationCodeFlow(description, scopes);

            var codeRequestUrl = googleAuthorizationCodeFlow.CreateAuthorizationCodeRequest(apiOptions.RedirectUrl);
            codeRequestUrl.ResponseType = "code";

            // Build the url
            var authorizationUrl = codeRequestUrl.Build();
            tempStorage.Set("user_id", id);
            tempStorage.Set("scopes", JsonConvert.SerializeObject(scopes));
            // Give it back to our caller for the redirect
            return authorizationUrl.AbsoluteUri;
        }

        public async Task RevokeToken(string accountId, params string[] scopes)
        {
            CheckForAccount(accountId);
            // Now, let's grab the AuthorizationCodeFlow that will generate a unique authorization URL to redirect our user to
            var googleAuthorizationCodeFlow = GetGoogleAuthorizationCodeFlow(null, scopes);
            var account = await publishContext.YoutubeAccounts.AsQueryable().FirstOrDefaultAsync(x => x.Key == accountId);
            if (account == null)
                throw new InvalidOperationException("Account not found for this id");
            try
            {
                await googleAuthorizationCodeFlow.RevokeTokenAsync(accountId, account.AccessToken, CancellationToken.None);
            }
            finally
            {
                account.IdToken = null;
                account.AccessToken = null;
                account.RefreshToken = null;
                account.IssuedUtc = null;
                account.ExpiresInSeconds = null;
                account.Scope = null;
                account.TokenType = null;
                publishContext.SaveChanges();
            }
        }

        public async Task DeleteToken(string accountId, params string[] scopes)
        {
            CheckForAccount(accountId);
            // Now, let's grab the AuthorizationCodeFlow that will generate a unique authorization URL to redirect our user to
            var googleAuthorizationCodeFlow = GetGoogleAuthorizationCodeFlow(null, scopes);
            var account = await publishContext.YoutubeAccounts.AsQueryable().FirstOrDefaultAsync(x => x.Key == accountId);
            if (account == null)
                throw new InvalidOperationException("Account not found for this id");
            try
            {
                await googleAuthorizationCodeFlow.DeleteTokenAsync(accountId, CancellationToken.None);
            }
            finally
            {
                publishContext.Remove(account);
                publishContext.SaveChanges();
            }
        }


        public async Task<string> GetYoutubeAuthenticationToken(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                /* 
                    This means the user canceled and did not grant us access. In this case, there will be a query parameter
                    on the request URL called 'error' that will have the error message. You can handle this case however.
                    Here, we'll just not do anything, but you should write code to handle this case however your application
                    needs to.
                */
            }

            // The userId is the ID of the user as it relates to YOUR application (NOT their Youtube Id).
            // This is the User ID that you assigned them whenever they signed up or however you uniquely identify people using your application     
            var userId = tempStorage.Get<string>("user_id");
            var scopes = JsonConvert.DeserializeObject<string[]>(tempStorage.Get<string>("scopes"));

            // We need to build the same redirect url again. Google uses this for validaiton I think...? Not sure what it's used for
            // at this stage, I just know we need it :)


            // Now, let's ask Youtube for our OAuth token that will let us do awesome things for the user

            var googleAuthorizationCodeFlow = GetGoogleAuthorizationCodeFlow(null, scopes);
            var token = await googleAuthorizationCodeFlow.ExchangeCodeForTokenAsync(userId, code, apiOptions.RedirectUrl, CancellationToken.None);

            // Now, you need to store this token in rlation to your user. So, however you save your user data, just make sure you
            // save the token for your user. This is the token you'll use to build up the UserCredentials needed to act on behalf
            // of the user.
            var tokenJson = JsonConvert.SerializeObject(token);
            //var dataStore = new EFDataStore(publishContext);

            // Now that we've got access to the user's YouTube account, let's get back
            // to our application :)
            return userId;
        }

    }
}