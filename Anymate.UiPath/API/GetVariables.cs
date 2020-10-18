
using System;
using System.Activities;
using System.ComponentModel;
using Newtonsoft.Json.Linq;
using Anymate.Client;

using TokenValidator = Anymate.Client.TokenValidator;

namespace Anymate.UiPath.API
{
    public class GetVariables : CodeActivity
    {
        private IAnymateClient _apiService;

       
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
            _apiService = AnymateClientFactory.GetClient();
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

