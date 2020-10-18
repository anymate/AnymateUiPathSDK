using System.Activities;
using Anymate.Client;
using Anymate.Models;

using System.ComponentModel;

using TokenValidator = Anymate.Client.TokenValidator;
using Anymate.UiPath.Models;

namespace Anymate.UiPath.API
{
    public class FinishRun : CodeActivity
    {
        private IAnymateClient _apiService;

       
        [Category("Input - OAuth2")]
        [RequiredArgument]
        public InArgument<string> AccessToken { get; set; }


        [Category("Input")]
        [RequiredArgument]
        [DefaultValue(null)]
        public InArgument<long> RunId { get; set; }

        [Category("Input")]
        [DefaultValue(0)]
        public InArgument<int> ExternalEntries { get; set; }

        [Category("Output - FlowControl")]
        public OutArgument<string> Message { get; set; }
        [Category("Output - FlowControl")]
        public OutArgument<bool> Succeeded { get; set; }


        protected override void Execute(CodeActivityContext context)
        {
            _apiService = AnymateClientFactory.GetClient();
            var access_token = AccessToken.Get(context);
            TokenValidator.AccessTokenLooksRight(access_token);
            var runId = RunId.Get(context);
            var externalEntries = ExternalEntries.Get(context);

            var jsonObject = _apiService.FinishRun<ApiResponse, ApiFinishRun>(access_token, new ApiFinishRun() { RunId = runId });
            Message.Set(context, jsonObject.Message);
            Succeeded.Set(context, jsonObject.Succeeded);

        }
    }
}

