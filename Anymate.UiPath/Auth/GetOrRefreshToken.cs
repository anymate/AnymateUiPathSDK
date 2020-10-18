using System.Activities;
using System.ComponentModel;

using Anymate.Client;
using Anymate.Models;

namespace Anymate.UiPath.Auth
{
    /// <summary>
    /// Analyses the current access_token, and if needed will refresh it. If no/empty access_token is provided then it will fetch a new access_token.
    /// </summary>
    public class GetOrRefreshToken : CodeActivity
    {
        private IAnymateClient _apiService;
        

        [Category("Input")]
        [RequiredArgument]
        public InArgument<string> CustomerKey { get; set; }

        [Category("Input")]
        [RequiredArgument]
        public InArgument<string> Secret { get; set; }
        [Category("Input")]
        [RequiredArgument]
        public InArgument<string> Username { get; set; }

        [Category("Input")]
        [RequiredArgument]
        public InArgument<string> Password { get; set; }

        [Category("Input")]
        public InArgument<string> CurrentAccessToken { get; set; }

        [Category("Output")]
        public OutArgument<string> NewAccessToken { get; set; }

        [Category("Output - FlowControl")]
        public OutArgument<bool> AuthSucceeded { get; set; }
        [Category("Output - FlowControl")]
        [DefaultValue(false)]
        public OutArgument<bool> SetRefreshNeededToFalse { get; set; }
        [Category("Output - FlowControl")]
        public OutArgument<string> HttpMessage { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            _apiService = AnymateClientFactory.GetClient();
            var access_token = CurrentAccessToken.Get(context);
            var request = new AuthTokenRequest()
            {
                client_id = CustomerKey.Get(context),
                client_secret = Secret.Get(context),
                username = Username.Get(context),
                password = Password.Get(context),
                access_token = access_token
            };

            var response = _apiService.GetOrRefreshAccessToken(request);

            NewAccessToken.Set(context, response.access_token);
            AuthSucceeded.Set(context, response.Succeeded);
            SetRefreshNeededToFalse.Set(context, false);
            HttpMessage.Set(context, response.HttpMessage);
        }
    }
}
