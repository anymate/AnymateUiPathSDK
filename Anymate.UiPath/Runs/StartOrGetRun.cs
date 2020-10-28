using System.Activities;
using System.ComponentModel;

namespace Anymate.UiPath.Runs
{
    public class StartOrGetRun : CodeActivity
    {
        private AnymateClient _anymateClient;


        [Category("Input")]
        [RequiredArgument]
        public InArgument<AnymateClient> AnymateClient { get; set; }


        [Category("Input")]
        [RequiredArgument]
        [DefaultValue(null)]
        public InArgument<string> ProcessKey { get; set; }

        [Category("Output")]
        public OutArgument<long> RunId { get; set; }


        protected override void Execute(CodeActivityContext context)
        {
            _anymateClient = AnymateClient.Get(context);
            
            var processKey = ProcessKey.Get(context);
            
            var jsonObject = _anymateClient.StartOrGetRun<Models.ApiNewRun>(processKey);

            RunId.Set(context, jsonObject.RunId);

        }
    }
}

