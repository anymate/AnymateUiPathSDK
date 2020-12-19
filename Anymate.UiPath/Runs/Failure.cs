using System;
using System.Activities;
using System.ComponentModel;


namespace Anymate.UiPath.Runs
{
    [Description("Report a Failure during a Run. Not to be confused with a Task Error. Please see http://docs.anymate.io/developer/API/runs/failure/ for more information.")]
    public class Failure : CodeActivity
    {
        private AnymateClient _anymateClient;

        [Description("Make sure to initiate your AnymateClient and pass the return object along before calling this activity.")]
        [Category("Input")]
        [RequiredArgument]
        public InArgument<AnymateClient> AnymateClient { get; set; }

        [Description("Which Process has encountered a failure.")]
        [Category("Input")]
        [RequiredArgument]
        [DefaultValue(null)]
        public InArgument<string> ProcessKey { get; set; }
        [Description("An optional message describing the failure. Can later be found under Process -> Notifications")]
        [Category("Input")]
        [DefaultValue(null)]
        public InArgument<string> Message { get; set; }

        [Description("Response message from Anymate.")]
        [Category("Output - FlowControl")]
        public OutArgument<string> Response { get; set; }
        [Description("Indicates that the action was processed as intended.")]
        [Category("Output - FlowControl")]
        public OutArgument<bool> Succeeded { get; set; }


        protected override void Execute(CodeActivityContext context)
        {
            _anymateClient = AnymateClient.Get(context);
            if (_anymateClient == null)
                throw new Exception("AnymateClient is null");

            var processKey = ProcessKey.Get(context);
            var message = Message.Get(context);
          

            var apiAction = new ApiProcessFailure()
            {
                Message = message,
                ProcessKey = processKey
            };


            var jsonObject = _anymateClient.Failure<ApiResponse, ApiProcessFailure>(apiAction);

            Response.Set(context, jsonObject.Message);
            Succeeded.Set(context, jsonObject.Succeeded);

        }
    }
}

