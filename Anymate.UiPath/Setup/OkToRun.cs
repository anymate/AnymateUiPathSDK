using System;
using System.Activities;
using System.ComponentModel;


namespace Anymate.UiPath
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
            if (_anymateClient == null)
                throw new Exception("AnymateClient is null");

            var processKey = ProcessKey.Get(context);
            
            var jsonObject = _anymateClient.OkToRun<ApiOkToRun>(processKey);

            ItIsOkToRun.Set(context, jsonObject.OkToRun);

        }
    }
}

