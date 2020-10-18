using System.Activities;
using Anymate.Client;

using System.ComponentModel;

using TokenValidator = Anymate.Client.TokenValidator;
using Anymate.UiPath.Models;

namespace Anymate.UiPath.API
{
    public class OkToRun : CodeActivity
    {
        private IAnymateService _apiService;


        [Category("Input")]
        [RequiredArgument]
        public InArgument<IAnymateService> AnymateService { get; set; }


        [Category("Input")]
        [RequiredArgument]
        [DefaultValue(null)]
        public InArgument<string> ProcessKey { get; set; }

        [Category("Output")]
        public OutArgument<bool> ItIsOkToRun { get; set; }


        protected override void Execute(CodeActivityContext context)
        {
            _apiService = AnymateService.Get(context);
            
            var processKey = ProcessKey.Get(context);
            
            var jsonObject = _apiService.OkToRun<ApiOkToRun>(processKey);

            ItIsOkToRun.Set(context, jsonObject.OkToRun);

        }
    }
}

