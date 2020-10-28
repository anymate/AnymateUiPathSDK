using System.Activities;
using System.ComponentModel;
using Anymate.UiPath.Models;

namespace Anymate.UiPath.Runs
{
    public class OkToRun : CodeActivity
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
        public OutArgument<bool> ItIsOkToRun { get; set; }


        protected override void Execute(CodeActivityContext context)
        {
            _anymateClient = AnymateClient.Get(context);
            
            var processKey = ProcessKey.Get(context);
            
            var jsonObject = _anymateClient.OkToRun<ApiOkToRun>(processKey);

            ItIsOkToRun.Set(context, jsonObject.OkToRun);

        }
    }
}

