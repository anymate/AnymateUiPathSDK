using System.Activities;
using System.ComponentModel;

namespace Anymate.UiPath.General
{

    /// <summary>
    /// Will fetch a new access_token using the Resource Owner Password Grant flow.
    /// </summary>
    public class InitializeClient : CodeActivity
    {
       
        [Category("Input")]
        [RequiredArgument]
        public InArgument<string> ClientId { get; set; }

        [Category("Input")]
        [RequiredArgument]
        public InArgument<string> Secret { get; set; }
        [Category("Input")]
        [RequiredArgument]
        public InArgument<string> Username { get; set; }

        [Category("Input")]
        [RequiredArgument]
        public InArgument<string> Password { get; set; }

        
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
