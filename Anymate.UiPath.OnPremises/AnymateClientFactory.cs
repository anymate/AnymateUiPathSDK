using System;
using Anymate.Client;

namespace Anymate.UiPath
{
    public static class AnymateClientFactory
    {
        public static IAnymateClient GetClient(string onPremisesAuthUri, string onPremisesClientUri)
        {
           
            if (!string.IsNullOrWhiteSpace(onPremisesAuthUri) || !string.IsNullOrWhiteSpace(onPremisesClientUri))
                throw new Exception("Client Uri and Auth Uri must both be set or both be empty.");

            
             return new AnymateClient(onPremisesClientUri, onPremisesAuthUri);
            
        }
    }
}