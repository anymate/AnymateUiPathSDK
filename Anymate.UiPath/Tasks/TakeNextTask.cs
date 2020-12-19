using System;
using System.Activities;
using System.ComponentModel;
using Newtonsoft.Json.Linq;

namespace Anymate.UiPath.Tasks
{
    [Description("Will return the next Task from the top of the queue on the specified Process. If there is no more Tasks, TaskId will be -1. This activity automatically starts and finish runs.")]
    public class TakeNextTask : CodeActivity
    {
        private AnymateClient _anymateClient;

        [Description("Make sure to initiate your AnymateClient and pass the return object along before calling this activity.")]
        [Category("Input")]
        [RequiredArgument]
        public InArgument<AnymateClient> AnymateClient { get; set; }

        [Description("The ProcessKey identifies where you take the task from.")]
        [Category("Input")]
        [RequiredArgument]
        [DefaultValue(null)]
        public InArgument<string> ProcessKey { get; set; }

        [Description("The Task as a raw json string.")]
        [Category("Output - Data")]
        public OutArgument<string> JsonString { get; set; }
        [Description("The Task as a JObject, using the Newtonsoft.Json library.")]
        [Category("Output - Data")]
        public OutArgument<JObject> JsonObject { get; set; }
        [Description("The TaskId of the Task. If TaskId > 0, it means there was a Task. TaskId = -1 means that no task was found.")]
        [Category("Output - Data")]
        public OutArgument<long> TaskId { get; set; }
        [Description("Returns true if the queue is empty and the Mate can stop working.")]
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

