using Anymate.Client;

namespace Anymate.UiPath
{
    public static class AnymateClientFactory
    {
        public static IAnymateClient GetClient()
        {
            return new AnymateClient();
        }
    }
}