using Newtonsoft.Json.Linq;
using System.Activities;
using System.Collections.Generic;
using System.ComponentModel;

namespace Anymate.UiPath.Helpers
{
    [Description("Returns a Dictionary that can be used for updating an existing Task.")]
    public class GetUpdateTaskDictionary : CodeActivity
    {
        [Description("TaskId of the Task to update.")]
        [Category("Input")]
        [DefaultValue(null)]
        [RequiredArgument]
        public InArgument<long> TaskId { get; set; }

        [Description("Use this field to optionally add a comment together with the update.")]
        [Category("Input - Optional")]
        public InArgument<string> Comment { get; set; }

        [Description("The dictionary that can be used to update the existing Task.")]
        [Category("Output")]
        public OutArgument<Dictionary<string,string>> Dict { get; set; }



        protected override void Execute(CodeActivityContext context)
        {
            var dict = new Dictionary<string,string>();
            var taskId = TaskId.Get(context);
            dict.Add(nameof(taskId), taskId.ToString());

            var newNote = Comment.Get(context);
            if(!string.IsNullOrWhiteSpace(newNote))
            {
                dict.Add("comment", newNote);
            }

            Dict.Set(context, dict);
        }
    }
}
