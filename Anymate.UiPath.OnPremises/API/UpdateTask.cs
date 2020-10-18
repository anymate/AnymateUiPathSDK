using System.Activities;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Markup;
using Anymate.Client;
using Newtonsoft.Json;
using TokenValidator = Anymate.Client.TokenValidator;

namespace Anymate.UiPath.API
{
    public class UpdateTask : CodeActivity
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

        [Category("Input - Json")]
        [OverloadGroup("OnlyJson")]
        [DefaultValue(null)]
        [RequiredArgument]
        public InArgument<string> JsonPayload { get; set; }


        [Category("Input - Dictionary")]
        [OverloadGroup("OnlyJObject")]
        [DefaultValue(null)]
        [RequiredArgument]
        public InArgument<Dictionary<string, string>> DictPayload { get; set; }

        [Category("Input - Required")]
        [OverloadGroup("OnlyJObject")]
        [OverloadGroup("OnlyJson")]
        [DefaultValue(null)]
        public InArgument<string> ProcessKey { get; set; }

        [Category("Input - Dictionary")]
        [OverloadGroup("OnlyJObject")]
        [DefaultValue(null)]
        [DependsOn("DictPayload")]
        public InArgument<string> Comment { get; set; }

        [Category("Input")]
        [OverloadGroup("OnlyJObject")]
        [DefaultValue(-1)]
        [DependsOn("DictPayload")]
        public InArgument<long> TaskId { get; set; }



        [Category("Output - FlowControl")]
        public OutArgument<string> Message { get; set; }
        [Category("Output - FlowControl")]
        public OutArgument<bool> Succeeded { get; set; }
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
                

            var json = JsonPayload.Get(context);
            if (!string.IsNullOrWhiteSpace(json))
            {
                var result = _apiService.UpdateTask(access_token, json);

                Message.Set(context, result.Message);
                Succeeded.Set(context, result.Succeeded);
            }
            else
            {

                var dict = DictPayload.Get(context);


                var newNote = Comment.Get(context);
                if (!string.IsNullOrWhiteSpace(newNote))
                {
                    dict[nameof(newNote)] = newNote;
                }

                var taskId = TaskId.Get(context);
                if (taskId > 0)
                {
                    dict[nameof(taskId)] = taskId.ToString();
                }
                var jsonPayload = JsonConvert.SerializeObject(dict);
                var jsonObject = _apiService.UpdateTask(access_token, jsonPayload);
                Message.Set(context, jsonObject.Message);
                Succeeded.Set(context, jsonObject.Succeeded);

            }
        }
    }
}
