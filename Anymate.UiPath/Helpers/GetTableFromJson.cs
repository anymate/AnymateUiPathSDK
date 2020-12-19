using System;
using System.Activities;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using Newtonsoft.Json.Linq;

namespace Anymate.UiPath.Helpers
{
    [Description("A helper function to read tables (lists with objects) from inside a json object.")]
    public class GetTableFromJson : CodeActivity
    {
        [Description("Input json object where we should read the table from. Only one of JsonString or JsonObject is required")]
        [Category("Input Raw Json")]
        [DefaultValue(null)]
        [OverloadGroup("RawJson")]
        [RequiredArgument]
        public InArgument<string> JsonString { get; set; }

        [Description("Input json object where we should read the table from. Only one of JsonString or JsonObject is required")]
        [Category("Input JObject")]
        [DefaultValue(null)]
        [OverloadGroup("JObject")]
        [RequiredArgument]
        public InArgument<JObject> JsonObject { get; set; }

        [Description("The Key from where we should take the table.")]
        [Category("Input")]
        [DefaultValue(null)]
        [OverloadGroup("RawJson")]
        [OverloadGroup("JObject")]
        [RequiredArgument]
        public InArgument<string> ArrayKey { get; set; }

        [Description("The json table serialized as a list of dictionaries with string keys and object values.")]
        [Category("Output")]
        public OutArgument<List<Dictionary<string, object>>> JsonTableOutput { get; set; }

        [Description("The json table serialized as a datatable.")]
        [Category("Output")]
        public OutArgument<DataTable> DataTableOutput { get; set; }



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
            var dt = jsonObject[arrayKey]?.ToObject<DataTable>();
            if (array == null)
                array = new List<Dictionary<string, object>>();
            JsonTableOutput.Set(context, array);
            if (dt == null)
                dt = new DataTable();
            DataTableOutput.Set(context, dt);

        }
    }
}
