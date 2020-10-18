using System;
using System.Activities;
using System.ComponentModel;
using Anymate.Client;
using Newtonsoft.Json.Linq;
using TokenValidator = Anymate.Client.TokenValidator;

namespace Anymate.UiPath.API
{
    public class GetVariables : CodeActivity
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
        public InArgument<string> ProcessKey { get; set; }


        [Category("Output - Data")]
        public OutArgument<string> JsonString { get; set; }
        [Category("Output - Data")]
        public OutArgument<JObject> JsonObject { get; set; }
        [Category("Output - Data")]
        public OutArgument<bool> RefreshTokenAsap { get; set; }



        protected override void Execute(CodeActivityContext context)
        {
            var onPremisesAuthUri = OnPremisesAuthUri.Get(context);
            var onPremisesClientUri = OnPremisesClientUri.Get(context);

            _apiService = AnymateClientFactory.GetClient(onPremisesAuthUri, onPremisesClientUri);

            var access_token = AccessToken.Get(context);
            RefreshTokenAsap.Set(context, !TokenValidator.RefreshNotNeeded(access_token));
            TokenValidator.AccessTokenLooksRight(access_token);
            var processKey = ProcessKey.Get(context);
            if(string.IsNullOrWhiteSpace(processKey))
            {
                throw new Exception("ProcessKey can't be null or empty.");
            }

            var result = _apiService.GetVariables(access_token, processKey);
            var jsonObject = JObject.Parse(result);

            JsonObject.Set(context, jsonObject);
            JsonString.Set(context, result);
        }
    }
}

