using System.Activities;
using Anymate.Client;
using Anymate.Models;

using System.ComponentModel;

using TokenValidator = Anymate.Client.TokenValidator;
using Anymate.UiPath.Models;

namespace Anymate.UiPath.API
{
    public class Failure : CodeActivity
    {
        private IAnymateClient _apiService;

     

        [Category("Input - OAuth2")]
        [RequiredArgument]
        public InArgument<string> AccessToken { get; set; }


        [Category("Input")]
        [RequiredArgument]
        [DefaultValue(null)]
        public InArgument<string> ProcessKey { get; set; }

        [Category("Input")]
        [DefaultValue(null)]
        public InArgument<string> Message { get; set; }

        
        [Category("Output - FlowControl")]
        public OutArgument<string> Response { get; set; }
        [Category("Output - FlowControl")]
        public OutArgument<bool> Succeeded { get; set; }
        [Category("Output - FlowControl")]
        [DefaultValue(false)]
        public OutArgument<bool> RefreshTokenAsap { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            _apiService = AnymateClientFactory.GetClient();
            var access_token = AccessToken.Get(context);
            if (!TokenValidator.RefreshNotNeeded(access_token))
                RefreshTokenAsap.Set(context, true);
            TokenValidator.AccessTokenLooksRight(access_token);
            var processKey = ProcessKey.Get(context);
            var message = Message.Get(context);
          

            var apiAction = new ApiProcessFailure()
            {
                Message = message,
                ProcessKey = processKey
            };


            var jsonObject = _apiService.Failure<ApiResponse, ApiProcessFailure>(access_token, apiAction);

            Response.Set(context, jsonObject.Message);
            Succeeded.Set(context, jsonObject.Succeeded);

        }
    }
}

