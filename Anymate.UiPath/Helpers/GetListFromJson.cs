using System;
using System.Activities;
using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json.Linq;

namespace Anymate.UiPath.Helpers
{
    [Description("A helper function to read lists from inside json objects.")]
    public class GetListFromJson : CodeActivity
    {
        [Description("Input json string where we should read the List from. Only one of JsonString or JsonObject is required")]
        [Category("Input Raw Json")]
        [DefaultValue(null)]
        [OverloadGroup("RawJson")]
        [RequiredArgument]
        public InArgument<string> JsonString { get; set; }

        [Description("Input json object where we should read the List from. Only one of JsonString or JsonObject is required")]
        [Category("Input JObject")]
        [DefaultValue(null)]
        [OverloadGroup("JObject")]
        [RequiredArgument]
        public InArgument<JObject> JsonObject { get; set; }
        [Description("The Key from where we should take the Json Array.")]
        [Category("Input")]
        [DefaultValue(null)]
        [OverloadGroup("RawJson")]
        [OverloadGroup("JObject")]
        [RequiredArgument]
        public InArgument<string> ArrayKey { get; set; }

        [Description("Returns an array with strings.")]
        [Category("Output")]
        public OutArgument<List<string>> JsonListOutput { get; set; }



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

            //var array = jsonObject.GetValue(arrayKey, StringComparison.InvariantCultureIgnoreCase);
            foreach (var item in jsonObject)
            {
                if (item.Key.Equals(arrayKey, StringComparison.InvariantCultureIgnoreCase))
                {
                    arrayKey = item.Key;
                    break;
                }

            }
            var array = jsonObject[arrayKey]?.ToObject<List<string>>();
            if (array == null)
                array = new List<string>();

            JsonListOutput.Set(context, array);

        }
    }
}
