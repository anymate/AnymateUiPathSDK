using System;
using System.Activities;
using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json.Linq;

namespace Anymate.UiPath.Helpers
{
    public class GetTableFromJson : CodeActivity
    {

        [Category("Input Raw Json")]
        [DefaultValue(null)]
        [OverloadGroup("RawJson")]
        [RequiredArgument]
        public InArgument<string> JsonString { get; set; }

        [Category("Input JObject")]
        [DefaultValue(null)]
        [OverloadGroup("JObject")]
        [RequiredArgument]
        public InArgument<JObject> JsonObject { get; set; }

        [Category("Input")]
        [DefaultValue(null)]
        [OverloadGroup("RawJson")]
        [OverloadGroup("JObject")]
        [RequiredArgument]
        public InArgument<string> ArrayKey { get; set; }


        [Category("Output")]
        public OutArgument<List<Dictionary<string, object>>> JsonTableOutput { get; set; }



        protected override void Execute(CodeActivityContext context)
        {
            var jsonString = JsonString.Get(context);

            var jsonObject = JsonObject.Get(context);
            if (string.IsNullOrWhiteSpace(jsonString) && jsonObject == null)
            {
                throw new ArgumentNullException("Both JsonString and JsonObject is null. One must be set.");
            }
            var arrayKey = ArrayKey.Get(context);
            if (string.IsNullOrWhiteSpace(arrayKey))
                throw new ArgumentNullException("ArrayKey must be set. It is used to find the array in the json object");

            if (jsonObject == null)
            {
                jsonObject = JObject.Parse(jsonString);
            }
            foreach (var item in jsonObject)
            {
                if (item.Key.Equals(arrayKey, StringComparison.InvariantCultureIgnoreCase))
                {
                    arrayKey = item.Key;
                    break;
                }

            }
            //var array = jsonObject.GetValue(arrayKey, StringComparison.InvariantCultureIgnoreCase)?.Value<List<Dictionary<string, object>>>();
            var array = jsonObject[arrayKey]?.ToObject<List<Dictionary<string, object>>>();
            
            if (array == null)
                array = new List<Dictionary<string, object>>();
            JsonTableOutput.Set(context, array);

        }
    }
}
