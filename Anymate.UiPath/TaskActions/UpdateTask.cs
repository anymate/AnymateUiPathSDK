﻿using System.Activities;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Markup;
using Anymate.UiPath.Models;
using Newtonsoft.Json;

namespace Anymate.UiPath.TaskActions
{
    public class UpdateTask : CodeActivity
    {
        private IAnymateService _apiService;


        [Category("Input")]
        [RequiredArgument]
        public InArgument<IAnymateService> AnymateService { get; set; }

        [Category("Input - Json")]
        [OverloadGroup("OnlyJson")]
        [DefaultValue(null)]
        [RequiredArgument]
        public InArgument<string> JsonPayload { get; set; }


        [Category("Input - Dictionary")]
        [OverloadGroup("OnlyJObject")]
        [DefaultValue(null)]
        [RequiredArgument]
        public InArgument<Dictionary<string, string>> DictPayload { get; set; }

        [Category("Input")]
        [OverloadGroup("OnlyJObject")]
        [OverloadGroup("OnlyJson")]
        [DefaultValue(null)]
        public InArgument<string> ProcessKey { get; set; }

        [Category("Input - Dictionary")]
        [OverloadGroup("OnlyJObject")]
        [DefaultValue(null)]
        [DependsOn("DictPayload")]
        public InArgument<string> Comment { get; set; }

        [Category("Input")]
        [OverloadGroup("OnlyJObject")]
        [DefaultValue(-1)]
        [DependsOn("DictPayload")]
        public InArgument<long> TaskId { get; set; }



        [Category("Output - FlowControl")]
        public OutArgument<string> Message { get; set; }
        [Category("Output - FlowControl")]
        public OutArgument<bool> Succeeded { get; set; }


        protected override void Execute(CodeActivityContext context)
        {
            _apiService = AnymateService.Get(context);
            

                

            var json = JsonPayload.Get(context);
            if (!string.IsNullOrWhiteSpace(json))
            {
                var result = _apiService.UpdateTask(json);

                Message.Set(context, result.Message);
                Succeeded.Set(context, result.Succeeded);
            }
            else
            {

                var dict = DictPayload.Get(context);


                var newNote = Comment.Get(context);
                if (!string.IsNullOrWhiteSpace(newNote))
                {
                    dict[nameof(newNote)] = newNote;
                }

                var taskId = TaskId.Get(context);
                if (taskId > 0)
                {
                    dict[nameof(taskId)] = taskId.ToString();
                }
                var jsonPayload = JsonConvert.SerializeObject(dict);
                var jsonObject = _apiService.UpdateTask<ApiResponse>(jsonPayload);
                Message.Set(context, jsonObject.Message);
                Succeeded.Set(context, jsonObject.Succeeded);

            }
        }
    }
}