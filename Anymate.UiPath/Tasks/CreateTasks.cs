using System;
using System.Activities;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Markup;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Anymate.UiPath.Tasks
{
    [Description("Will create multiple tasks on the specified Process. ProcessKeys can't be null.")]
    public class CreateTasks : CodeActivity
    {
        private AnymateClient _anymateClient;

        [Description("Make sure to initiate your AnymateClient and pass the return object along before calling this activity.")]
        [Category("Input")]
        [RequiredArgument]
        public InArgument<AnymateClient> AnymateClient { get; set; }

        [Description("Raw JSON as input for creating the tasks. This should be a json array. Only one of JsonPayload, DatatablePayload or ListPayload is required.")]
        [Category("Input - Json")]
        [OverloadGroup("OnlyJson")]
        [DefaultValue(null)]
        [RequiredArgument]
        public InArgument<string> JsonPayload { get; set; }

        [Description("DataTable as input for creating the tasks. Only one of JsonPayload, DatatablePayload or ListPayload is required.")]
        [Category("Input - DataTable")]
        [OverloadGroup("OnlyJObject")]
        [DefaultValue(null)]
        [RequiredArgument]
        public InArgument<DataTable> DatatablePayload { get; set; }

        [Description("List of objects for creating the tasks. Only one of JsonPayload, DatatablePayload or ListPayload is required.")]
        [Category("Input - List")]
        [OverloadGroup("OnlyList")]
        [DefaultValue(null)]
        [RequiredArgument]
        public InArgument<IEnumerable<object>> ListPayload { get; set; }

        [Description("ProcessKey identifying the target Process where the tasks should be created.")]
        [Category("Input")]
        [OverloadGroup("OnlyJObject")]
        [OverloadGroup("OnlyJson")]
        [OverloadGroup("OnlyList")]
        [RequiredArgument]
        [DefaultValue(null)]
        public InArgument<string> ProcessKey { get; set; }


        [Description("The response message from Anymate.")]
        [Category("Output - FlowControl")]
        public OutArgument<string> Message { get; set; }
        [Description("Indicates whether the action was processed as intended or not.")]
        [Category("Output - FlowControl")]
        public OutArgument<bool> Succeeded { get; set; }
        [Description("List of TaskIds that has been created.")]
        [Category("Output - FlowControl")]
        public OutArgument<List<long>> CreatedTaskIdList { get; set; }

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

            var datatable = DatatablePayload.Get(context);
            var json = JsonPayload.Get(context);
            var jsonEmpty = string.IsNullOrWhiteSpace(json);
            if (jsonEmpty)
            {
                var dict = DatatablePayload.Get(context);
                if (dict == null)
                {
                    var list = ListPayload.Get(context);
                    json = JsonConvert.SerializeObject(list);
                }
                else
                {
                    json = JsonConvert.SerializeObject(datatable);
                }


            }
            var useCreateTasks = true;
            if (!jsonEmpty)
            {
                var token = JToken.Parse(json);
                if (token is JObject)
                {
                    useCreateTasks = false;
                }
            }

            if (useCreateTasks)
            {
                var result = _anymateClient.CreateTasks<ApiCreateTasksResponse>(json, processKey);
                Message.Set(context, result.Message);
                Succeeded.Set(context, result.Succeeded);
                CreatedTaskIdList.Set(context, result.TaskIds);
            }
            else
            {
                var result = _anymateClient.CreateTask<ApiCreateTaskResponse>(json, processKey);
                Message.Set(context, result.Message);
                Succeeded.Set(context, result.Succeeded);
                CreatedTaskIdList.Set(context, new List<long>() { result.TaskId });
            }


        }

    }
}

