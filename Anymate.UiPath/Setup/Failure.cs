using System;
using System.Activities;
using System.ComponentModel;
using Anymate.UiPath.Models;

namespace Anymate.UiPath.Runs
{
    public class Failure : CodeActivity
    {
        private AnymateClient _anymateClient;


        [Category("Input")]
        [RequiredArgument]
        public InArgument<AnymateClient> AnymateClient { get; set; }


        [Category("Input")]
        [RequiredArgument]
        [DefaultValue(null)]
        public InArgument<string> ProcessKey { get; set; }

        [Category("Input")]
        [DefaultValue(null)]
        public InArgument<string> Message { get; set; }

        
        [Category("Output - FlowControl")]
        public OutArgument<string> Response { get; set; }
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

