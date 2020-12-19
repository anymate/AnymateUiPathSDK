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
    public class CreateTasks : CodeActivity
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

        [Category("Input - DataTable")]
        [OverloadGroup("OnlyJObject")]
        [DefaultValue(null)]
        [RequiredArgument]
        public InArgument<DataTable> DatatablePayload { get; set; }

        [Category("Input - List")]
        [OverloadGroup("OnlyList")]
        [DefaultValue(null)]
        [RequiredArgument]
        public InArgument<IEnumerable<object>> ListPayload { get; set; }

        [Category("Input")]
        [OverloadGroup("OnlyJObject")]
        [OverloadGroup("OnlyJson")]
        [OverloadGroup("OnlyList")]
        [RequiredArgument]
        [DefaultValue(null)]
        public InArgument<string> ProcessKey { get; set; }


        [Category("Output - FlowControl")]
        public OutArgument<string> Message { get; set; }
        [Category("Output - FlowControl")]
        public OutArgument<bool> Succeeded { get; set; }
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

