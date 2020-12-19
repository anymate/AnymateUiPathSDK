using System.Activities;
using System.ComponentModel;

namespace Anymate.UiPath.General
{

    /// <summary>
    /// Will fetch a new access_token using the Resource Owner Password Grant flow.
    /// </summary>
    [Description("Used to Initialize a AnymateClient, which is required in all Anymate API activities. Only use this if you have an on-premises anymate installation.")]
    public class InitializeClientOnPremises : CodeActivity
    {
        [Description("A custom client URI if you are not using Anymate Cloud")]
        [Category("Input")]
        [RequiredArgument]
        public InArgument<string> ClientUri { get; set; }

        [Description("A custom authentication URI if you are not using Anymate Cloud")]
        [Category("Input")]
        [RequiredArgument]
        public InArgument<string> AuthUri { get; set; }

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
            var client_uri = ClientUri.Get(context);
            var auth_uri = AuthUri.Get(context);
            var service = new AnymateClient(client_id, client_secret, username, password, client_uri, auth_uri);
            AnymateClient.Set(context, service);

        }
    }
}
