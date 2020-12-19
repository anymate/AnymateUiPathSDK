using System;
using System.Activities;
using System.ComponentModel;
using Newtonsoft.Json.Linq;

namespace Anymate.UiPath.Rules
{
    [Description("Will fetch all rules for a given Process.")]
    public class GetRules : CodeActivity
    {
        private AnymateClient _anymateClient;

        [Description("Make sure to initiate your AnymateClient and pass the return object along before calling this activity.")]
        [Category("Input")]
        [RequiredArgument]
        public InArgument<AnymateClient> AnymateClient { get; set; }

        [Description("Which Process to get rules for.")]
        [Category("Input")]
        [RequiredArgument]
        [DefaultValue(null)]
        public InArgument<string> ProcessKey { get; set; }

        [Description("The rules as a raw JSON string.")]
        [Category("Output - Data")]
        public OutArgument<string> JsonString { get; set; }
        [Description("The rules as a JObject using the Newtonsoft.Json library.")]
        [Category("Output - Data")]
        public OutArgument<JObject> JsonObject { get; set; }



        protected override void Execute(CodeActivityContext context)
        {
            _anymateClient = AnymateClient.Get(context);
            if (_anymateClient == null)
                throw new Exception("AnymateClient is null");
           
            var processKey = ProcessKey.Get(context);
            if(string.IsNullOrWhiteSpace(processKey))
            {
                throw new Exception("ProcessKey can't be null or empty.");
            }

            var result = _anymateClient.GetRules(processKey);
            var jsonObject = JObject.Parse(result);

            JsonObject.Set(context, jsonObject);
            JsonString.Set(context, result);
        }
    }
}

