using System.Activities;
using System.ComponentModel;
using Anymate.UiPath.Models;

namespace Anymate.UiPath.Runs
{
    public class FinishRun : CodeActivity
    {
        private IAnymateService _apiService;


        [Category("Input")]
        [RequiredArgument]
        public InArgument<IAnymateService> AnymateService { get; set; }


        [Category("Input")]
        [RequiredArgument]
        [DefaultValue(null)]
        public InArgument<long> RunId { get; set; }

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
            _apiService = AnymateService.Get(context);
            
            var runId = RunId.Get(context);
            var overwriteSeconds = OverwriteSecondsSaved.Get(context);
            var overwriteEntries = OverwriteEntries.Get(context);

            var jsonObject = _apiService.FinishRun<ApiResponse, ApiFinishRun>(new ApiFinishRun() { RunId = runId, OverwriteEntries= overwriteEntries, OverwriteSecondsSaved = overwriteSeconds });
            Message.Set(context, jsonObject.Message);
            Succeeded.Set(context, jsonObject.Succeeded);

        }
    }
}

