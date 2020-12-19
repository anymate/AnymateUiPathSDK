using System;
using System.Activities;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Markup;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Anymate.UiPath.Tasks
{
    [Description("Used to Create a new Task and take it right away. Primarily used if the same script can create and solve the task in one go, or when migrating legacy scripts.")]
    public class CreateAndTakeTask : CodeActivity
    {
        private AnymateClient _anymateClient;

        [Description("Make sure to initiate your AnymateClient and pass the return object along before calling this activity.")]
        [Category("Input")]
        [RequiredArgument]
        public InArgument<AnymateClient> AnymateClient { get; set; }

        [Description("Raw Json of the object for creating the task. Only one of JsonPayload or DictPayload is required.")]
        [Category("Input - Json")]
        [OverloadGroup("OnlyJson")]
        [DefaultValue(null)]
        [RequiredArgument]
        public InArgument<string> JsonPayload { get; set; }

        [Description("Dictionary for creating the task. Only one of JsonPayload or DictPayload is required.")]
        [Category("Input - Dictionary")]
        [OverloadGroup("OnlyJObject")]
        [DefaultValue(null)]
        [RequiredArgument]
        public InArgument<Dictionary<string, string>> DictPayload { get; set; }

        [Description("ProcessKey identifying the target Process where the task should be created.")]
        [Category("Input")]
        [OverloadGroup("OnlyJObject")]
        [OverloadGroup("OnlyJson")]
        [RequiredArgument]
        [DefaultValue(null)]
        public InArgument<string> ProcessKey { get; set; }

        [Description("If using the DictPayload as input, this field can be used to add the Comment if not already added.")]
        [Category("Input - Dictionary")]
        [OverloadGroup("OnlyJObject")]
        [DefaultValue(null)]
        [DependsOn("DictPayload")]
        public InArgument<string> Comment { get; set; }

        [Description("The created Task as a JObject, using the Newtonsoft.Json library.")]
        [Category("Output - Data")]
        public OutArgument<JObject> GetTask { get; set; }
        [Description("The created Task as a raw json string.")]
        [Category("Output - Data")]
        public OutArgument<string> GetTaskAsJson { get; set; }
        [Description("The TaskId of the created Task.")]
        [Category("Output - Data")]
        public OutArgument<long> TaskId { get; set; }

        [Description("Indicates whether the action was processed as intended or not.")]
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

            var jsonObject = JObject.Parse(result);
            var taskId = Convert.ToInt64(jsonObject["taskId"]);
            TaskId.Set(context, taskId);
            GetTask.Set(context, jsonObject);
            GetTaskAsJson.Set(context, result);
            Succeeded.Set(context, taskId > 0);

        }
    }
}
