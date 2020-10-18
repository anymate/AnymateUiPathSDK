using System.Activities;
using System.ComponentModel;

using Anymate.Client;

using Anymate.Models;
namespace Anymate.UiPath.Auth
{

    /// <summary>
    /// Will fetch a new access_token using the Resource Owner Password Grant flow.
    /// </summary>
    public class GetAccessToken : CodeActivity
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



        [Category("Output")]
        public OutArgument<string> NewAccessToken { get; set; }
        [Category("Output")]
        public OutArgument<string> NewRefreshToken { get; set; }

        [Category("Output - FlowControl")]
        public OutArgument<bool> AuthSucceeded { get; set; }
        [Category("Output - FlowControl")]
        public OutArgument<string> HttpMessage { get; set; }
        [Category("Output - FlowControl")]
        [DefaultValue(false)]
        public OutArgument<bool> SetRefreshNeededToFalse { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            _apiService = AnymateClientFactory.GetClient();
            var request = new AuthTokenRequest()
            {
                client_id = CustomerKey.Get(context),
                client_secret = Secret.Get(context),
                username = Username.Get(context),
                password = Password.Get(context)
            };

            var token = _apiService.GetAuthTokenPasswordFlow(request);
            NewAccessToken.Set(context, token.access_token);
            NewRefreshToken.Set(context, token.refresh_token);
            AuthSucceeded.Set(context, token.Succeeded);
            SetRefreshNeededToFalse.Set(context, false);
            HttpMessage.Set(context, token.HttpMessage);
        }
    }
}
