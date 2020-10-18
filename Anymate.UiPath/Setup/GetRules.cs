﻿using System;
using System.Activities;
using System.ComponentModel;
using Newtonsoft.Json.Linq;

namespace Anymate.UiPath.Setup
{
    public class GetRules : CodeActivity
    {
        private IAnymateService _apiService;


        [Category("Input")]
        [RequiredArgument]
        public InArgument<IAnymateService> AnymateService { get; set; }


        [Category("Input")]
        [RequiredArgument]
        [DefaultValue(null)]
        public InArgument<string> ProcessKey { get; set; }


        [Category("Output - Data")]
        public OutArgument<string> JsonString { get; set; }
        [Category("Output - Data")]
        public OutArgument<JObject> JsonObject { get; set; }



        protected override void Execute(CodeActivityContext context)
        {
            _apiService = AnymateService.Get(context);
            
           
            var processKey = ProcessKey.Get(context);
            if(string.IsNullOrWhiteSpace(processKey))
            {
                throw new Exception("ProcessKey can't be null or empty.");
            }

            var result = _apiService.GetVariables(processKey);
            var jsonObject = JObject.Parse(result);

            JsonObject.Set(context, jsonObject);
            JsonString.Set(context, result);
        }
    }
}
