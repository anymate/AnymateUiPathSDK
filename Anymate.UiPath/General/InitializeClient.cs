using System.Activities;
using System.ComponentModel;

namespace Anymate.UiPath.General
{

    [Description("Used to Initialize a AnymateClient, which is required in all Anymate API activities.")]
    public class InitializeClient : CodeActivity
    {
        [Description("Your client Id - find it under Settings -> Api Key in Anymate")]
        [Category("Input")]
        [RequiredArgument]
        public InArgument<string> ClientId { get; set; }

        [Description("Your client secret - find it under Settings -> Api Key in Anymate")]
        [Category("Input")]
        [RequiredArgument]
        public InArgument<string> Secret { get; set; }

        [Description("Username of the Mate who will log in.")]
        [Category("Input")]
        [RequiredArgument]
        public InArgument<string> Username { get; set; }

        [Description("Password of the Mate who will log in.")]
        [Category("Input")]
        [RequiredArgument]
        public InArgument<string> Password { get; set; }

        [Description("Returns AnymateClient object - this is needed for all other API activities.")]
        [Category("Output")]
        public OutArgument<AnymateClient> AnymateClient { get; set; }


        protected override void Execute(CodeActivityContext context)
        {
            
            var client_id = ClientId.Get(context);
            var client_secret = Secret.Get(context);
            var username = Username.Get(context);
            var password = Password.Get(context);
            var service = new AnymateClient(client_id, client_secret, username, password);
            AnymateClient.Set(context, service);

        }
    }
}
