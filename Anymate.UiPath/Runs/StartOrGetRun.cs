using System;
using System.Activities;
using System.ComponentModel;

namespace Anymate.UiPath.Runs
{
    [Description("Used to start a run on a Process in Anymate. If the Process has Tasks, then TakeNext will do this autoamtically and this activity is not required.")]
    public class StartOrGetRun : CodeActivity
    {
        private AnymateClient _anymateClient;

        [Description("Make sure to initiate your AnymateClient and pass the return object along before calling this activity.")]
        [Category("Input")]
        [RequiredArgument]
        public InArgument<AnymateClient> AnymateClient { get; set; }

        [Description("The ProcessKey identifies you want to start or resume a run from.")]
        [Category("Input")]
        [RequiredArgument]
        [DefaultValue(null)]
        public InArgument<string> ProcessKey { get; set; }
        [Description("The RunId of the Run.")]
        [Category("Output")]
        public OutArgument<long> RunId { get; set; }


        protected override void Execute(CodeActivityContext context)
        {
            _anymateClient = AnymateClient.Get(context);
            if (_anymateClient == null)
                throw new Exception("AnymateClient is null");

            var processKey = ProcessKey.Get(context);
            
            var jsonObject = _anymateClient.StartOrGetRun<ApiNewRun>(processKey);

            RunId.Set(context, jsonObject.RunId);

        }
    }
}

