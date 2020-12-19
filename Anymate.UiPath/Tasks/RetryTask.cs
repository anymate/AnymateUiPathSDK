using System;
using System.Activities;
using System.ComponentModel;


namespace Anymate.UiPath.Tasks
{
    [Description("Used to tell Anymate to retry the task later, if the option is enabled on the Process.")]
    public class RetryTask : CodeActivity
    {
        private AnymateClient _anymateClient;

        [Description("Make sure to initiate your AnymateClient and pass the return object along before calling this activity.")]
        [Category("Input")]
        [RequiredArgument]
        public InArgument<AnymateClient> AnymateClient { get; set; }


        [Description("TaskId identifying the Task that should be Retried later.")]
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

        [Description("Option to override the ActivationDate. Use this if the Mate should not retry the task right away but instead in 10 minutes, tomorrow or something else.")]
        [Category("Input")]
        [DefaultValue(null)]
        public InArgument<DateTimeOffset?> ActivationDate { get; set; }

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
            var activationDate = ActivationDate.Get(context);
            var overwriteSecondsSaved = OverwriteSecondsSaved.Get(context);
            var overwriteEntries = OverwriteEntries.Get(context);

            var apiAction = new ApiRetryAction()
            {
                Reason = reason,
                TaskId = taskId,
                Comment = newNote,
                OverwriteEntries = overwriteEntries,
                OverwriteSecondsSaved = overwriteSecondsSaved,
                ActivationDate = activationDate
            };



            var jsonObject = _anymateClient.Retry<ApiResponse, ApiRetryAction>(apiAction);
            Message.Set(context, jsonObject.Message);
            Succeeded.Set(context, jsonObject.Succeeded);

        }
    }
}

