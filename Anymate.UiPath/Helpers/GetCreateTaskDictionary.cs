using System.Activities;
using System.ComponentModel;
using Newtonsoft.Json.Linq;

namespace Anymate.UiPath.Helpers
{
    public class GetCreateTaskJObject : CodeActivity
    {

        [Category("Input - Required")]
        [DefaultValue(null)]
        [RequiredArgument]
        public InArgument<string> ProcessKey { get; set; }

        [Category("Input - Optional")]
        public InArgument<string> Comment { get; set; }

        [Category("Output - FlowControl")]
        public OutArgument<JObject> JObject { get; set; }



        protected override void Execute(CodeActivityContext context)
        {
            var dict = new JObject();
            var processKey = ProcessKey.Get(context);
            dict.Add(nameof(processKey), processKey);
            var newNote = Comment.Get(context);
            if(!string.IsNullOrWhiteSpace(newNote))
            {
                dict.Add(nameof(newNote), newNote);
            }

            JObject.Set(context, dict);
        }
    }
}
