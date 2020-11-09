using System;
using System.Activities;
using System.ComponentModel;

namespace Anymate.UiPath
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
            if (_anymateClient == null)
                throw new Exception("AnymateClient is null");

            var processKey = ProcessKey.Get(context);
            
            var jsonObject = _anymateClient.StartOrGetRun<ApiNewRun>(processKey);

            RunId.Set(context, jsonObject.RunId);

        }
    }
}

