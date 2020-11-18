using System.Activities;
using System.ComponentModel;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System;

namespace Anymate.UiPath.Helpers
{
    public class GetCreateTaskJsonObject : CodeActivity
    {

        [Category("Input - Optional")]
        [DefaultValue(null)]
        public InArgument<string> Comment { get; set; }

        [Category("Input - Optional")]
        [DefaultValue(null)]
        public InArgument<DateTime?> ActivationDate { get; set; }

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
