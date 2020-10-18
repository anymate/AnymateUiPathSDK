using System.Activities;
using System.ComponentModel;

using Anymate;
using Anymate.Models;
namespace Anymate.UiPath.Auth
{

    /// <summary>
    /// Will fetch a new access_token using the Resource Owner Password Grant flow.
    /// </summary>
    public class InitializeService : CodeActivity
    {
       
        [Category("Input")]
        [RequiredArgument]
        public InArgument<string> CustomerKey { get; set; }

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
        public OutArgument<IAnymateService> AnymateService { get; set; }
       

        protected override void Execute(CodeActivityContext context)
        {
            
            var client_id = CustomerKey.Get(context);
            var client_secret = Secret.Get(context);
            var username = Username.Get(context);
            var password = Password.Get(context);
            var service = new AnymateService(client_id, client_secret, username, password);
            AnymateService.Set(context, service);

        }
    }
}
