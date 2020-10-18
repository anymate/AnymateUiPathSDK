
using System;
using System.Activities;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Markup;
using Anymate.Client;
using Newtonsoft.Json;

namespace Anymate.UiPath.API
{
    public class CreateAndTakeTask : CodeActivity
    {
        private IAnymateClient _apiService;


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

        [Category("Output - Data")]
        public OutArgument<string> GetTask { get; set; }

        [Category("Output - Data")]
        public OutArgument<long> TaskId { get; set; }

        [Category("Output - FlowControl")]
        public OutArgument<bool> Succeeded { get; set; }
        [Category("Output - FlowControl")]
        [DefaultValue(false)]
        public OutArgument<bool> RefreshTokenAsap { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            _apiService = AnymateClientFactory.GetClient();
            var processKey = ProcessKey.Get(context);
            if (string.IsNullOrWhiteSpace(processKey))
            {
                throw new Exception("Missing processKey");
                //dict[nameof(processKey)] = processKey;
            }
            var access_token = AccessToken.Get(context);
            if (!TokenValidator.RefreshNotNeeded(access_token))
                RefreshTokenAsap.Set(context, true);

            TokenValidator.AccessTokenLooksRight(access_token);

            var json = JsonPayload.Get(context);
            if (string.IsNullOrWhiteSpace(json))
            {
                var dict = DictPayload.Get(context);


                var newNote = Comment.Get(context);
                if (!string.IsNullOrWhiteSpace(newNote))
                {
                    dict[nameof(newNote)] = newNote;
                }


                json = JsonConvert.SerializeObject(dict);
            }


            var result = _apiService.CreateAndTakeTask(access_token, json, processKey);

            var jsonResult = JsonConvert.DeserializeObject<Dictionary<string, string>>(result);
            var taskId = Convert.ToInt64(jsonResult["taskId"]);
            TaskId.Set(context, taskId);
            GetTask.Set(context, result);
            Succeeded.Set(context, taskId > 0);

        }
    }
}
