﻿using System.Activities;
using System.ComponentModel;
using Anymate.UiPath.Models;

namespace Anymate.UiPath.TaskActions
{
    public class SolveTask : CodeActivity
    {
        private AnymateClient _anymateClient;


        [Category("Input")]
        [RequiredArgument]
        public InArgument<AnymateClient> AnymateClient { get; set; }


        [Category("Input")]
        [RequiredArgument]
        [DefaultValue(null)]
        public InArgument<long> TaskId { get; set; }

        [Category("Input")]
        [DefaultValue(null)]
        public InArgument<string> Reason { get; set; }

        [Category("Input")]
        [DefaultValue(null)]
        public InArgument<string> Comment { get; set; }
        [Category("Input - KPI Overrides")]
        [DefaultValue(null)]
        public InArgument<int?> OverwriteEntries { get; set; }

        [Category("Input - KPI Overrides")]
        [DefaultValue(null)]
        public InArgument<int?> OverwriteSecondsSaved { get; set; }

        [Category("Output - FlowControl")]
        public OutArgument<string> Message { get; set; }
        [Category("Output - FlowControl")]
        public OutArgument<bool> Succeeded { get; set; }


        protected override void Execute(CodeActivityContext context)
        {
            _anymateClient = AnymateClient.Get(context);
            
           
            var taskId = TaskId.Get(context);
            var reason = Reason.Get(context);
            var newNote = Comment.Get(context);
            var overwriteSecondsSaved = OverwriteSecondsSaved.Get(context);
            var overwriteEntries = OverwriteEntries.Get(context);

            var apiAction = new ApiAction()
            {
                Reason = reason,
                TaskId = taskId,
                Comment = newNote,
                OverwriteEntries = overwriteEntries,
                OverwriteSecondsSaved = overwriteSecondsSaved
            };



            var jsonObject = _anymateClient.Solved<ApiResponse, ApiAction>(apiAction);
            
            Message.Set(context, jsonObject.Message);
            Succeeded.Set(context, jsonObject.Succeeded);

        }
    }
}

