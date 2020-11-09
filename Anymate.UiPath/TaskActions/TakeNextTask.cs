using System;
using System.Activities;
using System.ComponentModel;
using Newtonsoft.Json.Linq;

namespace Anymate.UiPath
{
    public class TakeNextTask : CodeActivity
    {
        private AnymateClient _anymateClient;


        [Category("Input")]
        [RequiredArgument]
        public InArgument<AnymateClient> AnymateClient { get; set; }


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



        protected override void Execute(CodeActivityContext context)
        {
            _anymateClient = AnymateClient.Get(context);
            if (_anymateClient == null)
                throw new Exception("AnymateClient is null");

            var processKey = ProcessKey.Get(context);
            if(string.IsNullOrWhiteSpace(processKey))
            {
                throw new System.Exception("ProcessKey can't be null or empty.");
            }

            var result = _anymateClient.TakeNext(processKey);
            var jsonObject = JObject.Parse(result);

            var taskId = Convert.ToInt64(jsonObject["taskId"]);
            QueueIsEmpty.Set(context, taskId < 0);
            TaskId.Set(context, taskId);
            JsonObject.Set(context, jsonObject);
            JsonString.Set(context, result);
        }
    }
}

