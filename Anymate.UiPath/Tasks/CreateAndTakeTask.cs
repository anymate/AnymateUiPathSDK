﻿using System;
using System.Activities;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Markup;
using Newtonsoft.Json;

namespace Anymate.UiPath.Tasks
{
    public class CreateAndTakeTask : CodeActivity
    {
        private AnymateClient _anymateClient;


        [Category("Input")]
        [RequiredArgument]
        public InArgument<AnymateClient> AnymateClient { get; set; }

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

        [Category("Input")]
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

        protected override void Execute(CodeActivityContext context)
        {
            _anymateClient = AnymateClient.Get(context);
            if (_anymateClient == null)
                throw new Exception("AnymateClient is null");

            var processKey = ProcessKey.Get(context);
            

            var json = JsonPayload.Get(context);
            if (string.IsNullOrWhiteSpace(json))
            {
                var dict = DictPayload.Get(context);


                var comment = Comment.Get(context);
                if (!string.IsNullOrWhiteSpace(comment))
                {
                    dict[nameof(comment)] = comment;
                }


                json = JsonConvert.SerializeObject(dict);
            }


            var result = _anymateClient.CreateAndTakeTask(json, processKey);

            var jsonResult = JsonConvert.DeserializeObject<Dictionary<string, string>>(result);
            var taskId = Convert.ToInt64(jsonResult["taskId"]);
            TaskId.Set(context, taskId);
            GetTask.Set(context, result);
            Succeeded.Set(context, taskId > 0);

        }
    }
}