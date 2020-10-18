using System.Activities;
using Anymate.Client;
using Anymate.Models;

using System.ComponentModel;

using TokenValidator = Anymate.Client.TokenValidator;
using Anymate.UiPath.Models;

namespace Anymate.UiPath.API
{
    public class Failure : CodeActivity
    {
        private IAnymateService _apiService;


        [Category("Input")]
        [RequiredArgument]
        public InArgument<IAnymateService> AnymateService { get; set; }


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
        [Category("Output - FlowControl")]
        [DefaultValue(false)]
        public OutArgument<bool> RefreshTokenAsap { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            _apiService = AnymateService.Get(context);
            
           
            var processKey = ProcessKey.Get(context);
            var message = Message.Get(context);
          

            var apiAction = new ApiProcessFailure()
            {
                Message = message,
                ProcessKey = processKey
            };


            var jsonObject = _apiService.Failure<ApiResponse, ApiProcessFailure>(apiAction);

            Response.Set(context, jsonObject.Message);
            Succeeded.Set(context, jsonObject.Succeeded);

        }
    }
}

