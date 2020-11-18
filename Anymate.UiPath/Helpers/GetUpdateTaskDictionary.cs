using Newtonsoft.Json.Linq;
using System.Activities;
using System.ComponentModel;

namespace Anymate.UiPath.Helpers
{
    public class GetUpdateTaskJsonObject : CodeActivity
    {

        [Category("Input")]
        [DefaultValue(null)]
        [RequiredArgument]
        public InArgument<long> TaskId { get; set; }

        [Category("Input - Optional")]
        public InArgument<string> Comment { get; set; }

        [Category("Output - FlowControl")]
        public OutArgument<Dictionary<string,string>> Dict { get; set; }



        protected override void Execute(CodeActivityContext context)
        {
            var dict = new Dictionary<string,string>();
            var taskId = TaskId.Get(context);
            dict.Add(nameof(taskId), taskId);

            var newNote = Comment.Get(context);
            if(!string.IsNullOrWhiteSpace(newNote))
            {
                dict.Add("comment", newNote);
            }

            Dict.Set(context, dict);
        }
    }
}
