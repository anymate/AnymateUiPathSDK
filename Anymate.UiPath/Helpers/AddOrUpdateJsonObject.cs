using Newtonsoft.Json.Linq;
using System.Activities;
using System.ComponentModel;

namespace Anymate.UiPath.Helpers
{
    public class AddOrUpdateJsonObject : CodeActivity
    {

        [Category("Misc")]
        [RequiredArgument]
        public InOutArgument<JObject> JObject { get; set; }

        [Category("Misc")]
        [RequiredArgument]
        public InArgument<string> Key { get; set; }

        [Category("Misc")]
        [RequiredArgument]
        public InArgument<string> Value { get; set; }



        protected override void Execute(CodeActivityContext context)
        {
            var dict = JObject.Get(context);
            if (dict == null)
                dict = new JObject();

            dict[Key.Get(context)] = Value.Get(context);

            JObject.Set(context, dict);
        }
    }
}
