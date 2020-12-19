using System;
using System.Activities;
using System.ComponentModel;


namespace Anymate.UiPath.Tasks
{
    [Description("Used when a business user has to take a look at the Task. Typically used for special cases or business rules where the script can't finish the Task.")]
    public class ManualTask : CodeActivity
    {
        private AnymateClient _anymateClient;

        [Description("Make sure to initiate your AnymateClient and pass the return object along before calling this activity.")]
        [Category("Input")]
        [RequiredArgument]
        public InArgument<AnymateClient> AnymateClient { get; set; }


        [Description("TaskId identifying the Task that is sent to Manual.")]
        [Category("Input")]
        [RequiredArgument]
        [DefaultValue(null)]
        public InArgument<long> TaskId { get; set; }

        [Description("Add a custom reason.")]
        [Category("Input")]
        [DefaultValue(null)]
        public InArgument<string> Reason { get; set; }

        [Description("Add a comment.")]
        [Category("Input")]
        [DefaultValue(null)]
        public InArgument<string> Comment { get; set; }
        [Description("Option to override Entries - used for Advanced KPI's, please go to Process Settings to learn more about this.")]
        [Category("Input - KPI Overrides")]
        [DefaultValue(null)]
        public InArgument<int?> OverwriteEntries { get; set; }

        [Description("Option to override KPI Seconds - used for Advanced KPI's, please go to Process Settings to learn more about this.")]
        [Category("Input - KPI Overrides")]
        [DefaultValue(null)]
        public InArgument<int?> OverwriteSecondsSaved { get; set; }


        [Description("The response message from Anymate.")]
        [Category("Output - FlowControl")]
        public OutArgument<string> Message { get; set; }
        [Description("Indicates whether the action was processed as intended or not.")]
        [Category("Output - FlowControl")]
        public OutArgument<bool> Succeeded { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            _anymateClient = AnymateClient.Get(context);
            if (_anymateClient == null)
                throw new Exception("AnymateClient is null");

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

  
            var jsonObject = _anymateClient.Manual<ApiResponse, ApiAction>(apiAction);
            
            Message.Set(context, jsonObject.Message);
            Succeeded.Set(context, jsonObject.Succeeded);

        }
    }
}

