using System.Activities;
using System.ComponentModel;
using Anymate.Client;
using Anymate.Models;
using TokenValidator = Anymate.Client.TokenValidator;

namespace Anymate.UiPath.API
{
    public class FinishRun : CodeActivity
    {
        private IAnymateClient _apiService;

        [Category("On Premises Configuration")]
        [DefaultValue(null)]
        [RequiredArgument]
        public InArgument<string> OnPremisesAuthUri { get; set; }
        [Category("On Premises Configuration")]
        [RequiredArgument]
        [DefaultValue(null)]
        public InArgument<string> OnPremisesClientUri { get; set; }
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
            var onPremisesAuthUri = OnPremisesAuthUri.Get(context);
            var onPremisesClientUri = OnPremisesClientUri.Get(context);

            _apiService = AnymateClientFactory.GetClient(onPremisesAuthUri, onPremisesClientUri);

            var access_token = AccessToken.Get(context);
            TokenValidator.AccessTokenLooksRight(access_token);
            var runId = RunId.Get(context);
            var externalEntries = ExternalEntries.Get(context);

            var jsonObject = _apiService.FinishRun(access_token, new AnymateFinishRun() { RunId = runId });
            Message.Set(context, jsonObject.Message);
            Succeeded.Set(context, jsonObject.Succeeded);

        }
    }
}

