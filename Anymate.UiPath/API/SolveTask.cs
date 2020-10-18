using System.Activities;
using System.ComponentModel;

using Anymate.Client;
using Anymate.Models;
using Anymate.UiPath.Models;
using TokenValidator = Anymate.Client.TokenValidator;

namespace Anymate.UiPath.API
{
    public class SolveTask : CodeActivity
    {
        private IAnymateClient _apiService;

      
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
        [Category("Output - FlowControl")]
        [DefaultValue(false)]
        public OutArgument<bool> RefreshTokenAsap { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            _apiService = AnymateClientFactory.GetClient();
            var access_token = AccessToken.Get(context);
            if (!TokenValidator.RefreshNotNeeded(access_token))
                RefreshTokenAsap.Set(context, true);
            TokenValidator.AccessTokenLooksRight(access_token);
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



            var jsonObject = _apiService.Solved<ApiResponse, ApiAction>(access_token, apiAction);
            
            Message.Set(context, jsonObject.Message);
            Succeeded.Set(context, jsonObject.Succeeded);

        }
    }
}

