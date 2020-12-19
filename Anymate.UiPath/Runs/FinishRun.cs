using System;
using System.Activities;
using System.ComponentModel;


namespace Anymate.UiPath.Runs
{
    [Description("Used to tell Anymate that a Run has finished. If the Process has Tasks, then TakeNext will do this autoamtically and this activity is not required.")]
    public class FinishRun : CodeActivity
    {
        private AnymateClient _anymateClient;

        [Description("Make sure to initiate your AnymateClient and pass the return object along before calling this activity.")]
        [Category("Input")]
        [RequiredArgument]
        public InArgument<AnymateClient> AnymateClient { get; set; }

        [Description("The runId for the run that is finished.")]
        [Category("Input")]
        [RequiredArgument]
        [DefaultValue(null)]
        public InArgument<long> RunId { get; set; }
        [Description("Option to override Entries - used for Advanced KPI's, please go to Process Settings to learn more about this. If the Process has Tasks, this will be ignored.")]
        [Category("Input - KPI Overrides")]
        [DefaultValue(null)]
        public InArgument<int?> OverwriteEntries { get; set; }
        [Description("Option to override KPI Seconds - used for Advanced KPI's, please go to Process Settings to learn more about this. If the Process has Tasks, this will be ignored.")]
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

            var runId = RunId.Get(context);
            var overwriteSeconds = OverwriteSecondsSaved.Get(context);
            var overwriteEntries = OverwriteEntries.Get(context);

            var jsonObject = _anymateClient.FinishRun<ApiResponse, ApiFinishRun>(new ApiFinishRun() { RunId = runId, OverwriteEntries = overwriteEntries, OverwriteSecondsSaved = overwriteSeconds });
            Message.Set(context, jsonObject.Message);
            Succeeded.Set(context, jsonObject.Succeeded);

        }
    }
}

