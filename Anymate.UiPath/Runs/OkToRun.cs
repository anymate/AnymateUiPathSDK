using System;
using System.Activities;
using System.ComponentModel;


namespace Anymate.UiPath.Runs
{
    [Description("Use this Activity to check if it is ok to start work on a given Process before starting anything else.")]
    public class OkToRun : CodeActivity
    {
        private AnymateClient _anymateClient;

        [Description("Make sure to initiate your AnymateClient and pass the return object along before calling this activity.")]
        [Category("Input")]
        [RequiredArgument]
        public InArgument<AnymateClient> AnymateClient { get; set; }

        [Description("Which Process to check.")]
        [Category("Input")]
        [RequiredArgument]
        [DefaultValue(null)]
        public InArgument<string> ProcessKey { get; set; }
        [Description("Returns true if it is ok to start working, false if the Mate should not continue.")]
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

