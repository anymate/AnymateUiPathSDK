using System.Activities;
using System.ComponentModel;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System;

namespace Anymate.UiPath.Helpers
{
    [Description("Used to get a new Dictionary that can be used when creating a new task.")]
    public class GetCreateTaskDictionary : CodeActivity
    {
        [Description("Will add a optional comment to the Create Task dictionary.")]
        [Category("Input - Optional")]
        [DefaultValue(null)]
        public InArgument<string> Comment { get; set; }

        [Description("Will add a optional activationDate - if not supplied, anymate will use the default activationDate if set otherwise using creation date.")]
        [Category("Input - Optional")]
        [DefaultValue(null)]
        public InArgument<DateTimeOffset?> ActivationDate { get; set; }

        [Description("The CreateTask dictionary.")]
        [Category("Output - FlowControl")]
        public OutArgument<Dictionary<string,string>> Dict { get; set; }



        protected override void Execute(CodeActivityContext context)
        {
            var dict = new Dictionary<string,string>();
            var newNote = Comment.Get(context);
            if(!string.IsNullOrWhiteSpace(newNote))
            {
                dict.Add("comment", newNote);
            }

            var activationDate = ActivationDate.Get(context);
            if(activationDate != null)
            {
                dict.Add("activationDate", activationDate?.ToString("s"));
            }

            Dict.Set(context, dict);
        }
    }
}
