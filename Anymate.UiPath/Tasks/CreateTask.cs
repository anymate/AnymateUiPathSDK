using System;
using System.Activities;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Markup;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Anymate.UiPath.Tasks
{
    [Description("Will create a single task on the specified Process. ProcessKeys can't be null.")]
    public class CreateTask : CodeActivity
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

        [Category("Input - Dictionary")]
        [OverloadGroup("OnlyJObject")]
        [DefaultValue(null)]
        [DependsOn("DictPayload")]
        public InArgument<string> Comment { get; set; }

        [Description("The response message from Anymate.")]
        [Category("Output - FlowControl")]
        public OutArgument<string> Message { get; set; }
        [Description("Indicates whether the action was processed as intended or not.")]
        [Category("Output - FlowControl")]
        public OutArgument<bool> Succeeded { get; set; }
        [Description("TaskId for the Task that was created. If you wish to work with this Task right away, please use the Create And Take Task activity.")]
        [Category("Output - FlowControl")]
        public OutArgument<long> CreatedTaskId { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            _anymateClient = AnymateClient.Get(context);
            if (_anymateClient == null)
                throw new Exception("AnymateClient is null");

            var processKey = ProcessKey.Get(context);
            if (string.IsNullOrWhiteSpace(processKey))
            {
                throw new Exception("Processkey missing");
            }

            var json = JsonPayload.Get(context);
            var jsonEmpty = string.IsNullOrWhiteSpace(json);
            if (jsonEmpty)
            {
                var dict = DictPayload.Get(context);

                var comment = Comment.Get(context);
                if (!string.IsNullOrWhiteSpace(comment))
                {
                    dict[nameof(comment)] = comment;
                }

                json = JsonConvert.SerializeObject(dict);
            }

            var useCreateTask = true;
            if (!jsonEmpty)
            {
                var token = JToken.Parse(json);
                if (token is JArray)
                {
                    useCreateTask = false;
                }
            }

            if (useCreateTask)
            {
                var result = _anymateClient.CreateTask<ApiCreateTaskResponse>(json, processKey);

                Message.Set(context, result.Message);
                Succeeded.Set(context, result.Succeeded);
                CreatedTaskId.Set(context, result.TaskId);
            }
            else
            {
                var result = _anymateClient.CreateTasks<ApiCreateTasksResponse>(json, processKey);
                Message.Set(context, result.Message);
                Succeeded.Set(context, result.Succeeded);
                CreatedTaskId.Set(context, result.TaskIds.FirstOrDefault());
            }
        }
    }
}
