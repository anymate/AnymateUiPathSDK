using System;
using System.Activities;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Markup;

using Newtonsoft.Json;

namespace Anymate.UiPath.Tasks
{
    [Description("Used to update data fields on a Task, e.g. if the script should supply more data before sending the Task to Manual.")]
    public class UpdateTask : CodeActivity
    {
        private AnymateClient _anymateClient;

        [Description("Make sure to initiate your AnymateClient and pass the return object along before calling this activity.")]
        [Category("Input")]
        [RequiredArgument]
        public InArgument<AnymateClient> AnymateClient { get; set; }

        [Description("Raw JSON string that will be used to update the Task. Remember to supply a TaskId. Only one of JsonPayload or DictPayload is needed.")]
        [Category("Input - Json")]
        [OverloadGroup("OnlyJson")]
        [DefaultValue(null)]
        [RequiredArgument]
        public InArgument<string> JsonPayload { get; set; }

        [Description("Dictionary that will be used to update the Task. Only one of JsonPayload or DictPayload is needed.")]
        [Category("Input - Dictionary")]
        [OverloadGroup("OnlyJObject")]
        [DefaultValue(null)]
        [RequiredArgument]
        public InArgument<Dictionary<string, string>> DictPayload { get; set; }

        [Description("If you use a DictPayload, then you can use this to add a comment. You can also supply it yourself in the dictionary.")]
        [Category("Input - Dictionary")]
        [OverloadGroup("OnlyJObject")]
        [DefaultValue(null)]
        [DependsOn("DictPayload")]
        public InArgument<string> Comment { get; set; }

        [Description("If you use a DictPayload, then you can use this to add a TaskId. You can also supply it yourself in the dictionary.")]
        [Category("Input")]
        [OverloadGroup("OnlyJObject")]
        [DefaultValue(-1)]
        [DependsOn("DictPayload")]
        public InArgument<long> TaskId { get; set; }


        [Description("The response message from Anymate.")]
        [Category("Output - FlowControl")]
        public OutArgument<string> Message { get; set; }
        [Description("Indicates whether the action was processed as intended or not.")]
        [Category("Output - FlowControl")]
        public OutArgument<bool> Succeeded { get; set; }


        protected override void Execute(CodeActivityContext context)
        {
            _anymateClient = AnymateClient.Get(context);
            if (_anymateClient == null)
                throw new Exception("AnymateClient is null");

            var json = JsonPayload.Get(context);
            if (!string.IsNullOrWhiteSpace(json))
            {
                var result = _anymateClient.UpdateTask<ApiResponse>(json);

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
                var jsonObject = _anymateClient.UpdateTask<ApiResponse>(jsonPayload);
                Message.Set(context, jsonObject.Message);
                Succeeded.Set(context, jsonObject.Succeeded);

            }
        }
    }
}
