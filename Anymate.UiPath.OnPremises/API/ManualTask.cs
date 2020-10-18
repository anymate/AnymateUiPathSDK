using System.Activities;
using System.ComponentModel;
using Anymate.Client;
using Anymate.Models;
using TokenValidator = Anymate.Client.TokenValidator;

namespace Anymate.UiPath.API
{
    public class ManualTask : CodeActivity
    {
        private IAnymateClient _apiService;

        [Category("On Premises Configuration")]
        [DefaultValue(null)]
        [RequiredArgument]
        public InArgument<string> OnPremisesAuthUri { get; set; }
        [Category("On Premises Configuration")]
        [RequiredArgument]
        [DefaultValue(null)]
        public InArgument<string> OnPremisesClientUri { get; set; }

        [Category("Input - OAuth2")]
        [RequiredArgument]
        public InArgument<string> AccessToken { get; set; }


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


        [Category("Output - FlowControl")]
        public OutArgument<string> Message { get; set; }
        [Category("Output - FlowControl")]
        public OutArgument<bool> Succeeded { get; set; }
        [Category("Output - FlowControl")]
        [DefaultValue(false)]
        public OutArgument<bool> RefreshTokenAsap { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var onPremisesAuthUri = OnPremisesAuthUri.Get(context);
            var onPremisesClientUri = OnPremisesClientUri.Get(context);

            _apiService = AnymateClientFactory.GetClient(onPremisesAuthUri, onPremisesClientUri);

            var access_token = AccessToken.Get(context);
            if (!TokenValidator.RefreshNotNeeded(access_token))
                RefreshTokenAsap.Set(context, true);
            TokenValidator.AccessTokenLooksRight(access_token);
            var taskId = TaskId.Get(context);
            var reason = Reason.Get(context);
            var newNote = Comment.Get(context);

            var apiAction = new AnymateTaskAction()
            {
                Reason = reason,
                TaskId = taskId,
                Comment = newNote
            };

  
            var jsonObject = _apiService.Manual(access_token, apiAction);
            
            Message.Set(context, jsonObject.Message);
            Succeeded.Set(context, jsonObject.Succeeded);

        }
    }
}

