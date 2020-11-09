using Newtonsoft.Json.Linq;
using System.Activities;
using System.ComponentModel;

namespace Anymate.UiPath
{
    public class GetUpdateTaskDictionary : CodeActivity
    {

        [Category("Input")]
        [DefaultValue(null)]
        [RequiredArgument]
        public InArgument<long> TaskId { get; set; }

        [Category("Input - Optional")]
        public InArgument<string> Comment { get; set; }

        [Category("Output - FlowControl")]
        public OutArgument<JObject> JObject { get; set; }



        protected override void Execute(CodeActivityContext context)
        {
            var dict = new JObject();
            var taskId = TaskId.Get(context);
            dict.Add(nameof(taskId), taskId);

            var newNote = Comment.Get(context);
            if(!string.IsNullOrWhiteSpace(newNote))
            {
                dict.Add(nameof(newNote), newNote);
            }

            JObject.Set(context, dict);
        }
    }
}
