using System;
using System.Activities;
using System.ComponentModel;
using Anymate.Client;
using Newtonsoft.Json.Linq;
using TokenValidator = Anymate.Client.TokenValidator;

namespace Anymate.UiPath.API
{
    public class TakeNextTask : CodeActivity
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
        public OutArgument<long> TaskId { get; set; }
        [Category("Output - FlowControl")]
        public OutArgument<bool> QueueIsEmpty { get; set; }
        [Category("Output - FlowControl")]
        [DefaultValue(false)]
        public OutArgument<bool> RefreshTokenAsap { get; set; }



        protected override void Execute(CodeActivityContext context)
        {
            var onPremisesAuthUri = OnPremisesAuthUri.Get(context);
            var onPremisesClientUri = OnPremisesClientUri.Get(context);

            _apiService = AnymateClientFactory.GetClient(onPremisesAuthUri, onPremisesClientUri);

            var access_token = AccessToken.Get(context);
            if (!TokenValidator.RefreshNotNeeded(access_token))
                RefreshTokenAsap.Set(context, true);
            TokenValidator.AccessTokenLooksRight(access_token);
            var processKey = ProcessKey.Get(context);
            if(string.IsNullOrWhiteSpace(processKey))
            {
                throw new System.Exception("ProcessKey can't be null or empty.");
            }

            var result = _apiService.TakeNext(access_token, processKey);
            var jsonObject = JObject.Parse(result);

            var taskId = Convert.ToInt64(jsonObject["taskId"]);
            QueueIsEmpty.Set(context, taskId < 0);
            TaskId.Set(context, taskId);
            JsonObject.Set(context, jsonObject);
            JsonString.Set(context, result);
        }
    }
}

