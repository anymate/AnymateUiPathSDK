using System.Activities;
using System.ComponentModel;

namespace Anymate.UiPath.Runs
{
    public class StartOrGetRun : CodeActivity
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
        public OutArgument<long> RunId { get; set; }


        protected override void Execute(CodeActivityContext context)
        {
            _apiService = AnymateService.Get(context);
            
            var processKey = ProcessKey.Get(context);
            
            var jsonObject = _apiService.StartOrGetRun<Models.ApiNewRun>(processKey);

            RunId.Set(context, jsonObject.RunId);

        }
    }
}

