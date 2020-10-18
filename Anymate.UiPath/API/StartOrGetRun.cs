using System.Activities;
using Anymate.Client;

using System.ComponentModel;

using TokenValidator = Anymate.Client.TokenValidator;

namespace Anymate.UiPath.API
{
    public class StartOrGetRun : CodeActivity
    {
        private IAnymateClient _apiService;

        
        [Category("Input - OAuth2")]
        [RequiredArgument]
        public InArgument<string> AccessToken { get; set; }


        [Category("Input")]
        [RequiredArgument]
        [DefaultValue(null)]
        public InArgument<string> ProcessKey { get; set; }

        [Category("Output")]
        public OutArgument<long> RunId { get; set; }


        protected override void Execute(CodeActivityContext context)
        {
            _apiService = AnymateClientFactory.GetClient();
            var access_token = AccessToken.Get(context);
            TokenValidator.AccessTokenLooksRight(access_token);
            var processKey = ProcessKey.Get(context);
            
            var jsonObject = _apiService.StartOrGetRun<Models.ApiNewRun>(access_token, processKey);

            RunId.Set(context, jsonObject.RunId);

        }
    }
}

