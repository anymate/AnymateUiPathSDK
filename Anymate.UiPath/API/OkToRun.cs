using System.Activities;
using Anymate.Client;

using System.ComponentModel;

using TokenValidator = Anymate.Client.TokenValidator;
using Anymate.UiPath.Models;

namespace Anymate.UiPath.API
{
    public class OkToRun : CodeActivity
    {
        private IAnymateClient _apiService;

      
        [RequiredArgument]
        public InArgument<string> AccessToken { get; set; }


        [Category("Input")]
        [RequiredArgument]
        [DefaultValue(null)]
        public InArgument<string> ProcessKey { get; set; }

        [Category("Output")]
        public OutArgument<bool> ItIsOkToRun { get; set; }


        protected override void Execute(CodeActivityContext context)
        {
            _apiService = AnymateClientFactory.GetClient();
            var access_token = AccessToken.Get(context);
            TokenValidator.AccessTokenLooksRight(access_token);
            var processKey = ProcessKey.Get(context);
            
            var jsonObject = _apiService.OkToRun<ApiOkToRun>(access_token, processKey);

            ItIsOkToRun.Set(context, jsonObject.OkToRun);

        }
    }
}

