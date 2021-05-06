using Love.Providers;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Love.Services.StateServices
{ 
    public class SessionStateService
    {
        private static SessionStateService sessionStateService;
        private StateProvider stateProvider = new StateProvider();
        
        public static SessionStateService GetSessionStateService()
        {
            if (sessionStateService == null)
                sessionStateService = new SessionStateService();

            return sessionStateService;
        }

       
        public byte[] StrongKey { get; set; }
        public string ClientPrivateKey { get; set; }
        public string ClientPublicKey { get; set; }
        public string ServerPublicKey { get; set; }
        public string AcessToken { get; set; }

        public void SetStateAsync(byte[] strongKey, string clientPrivateKey, string clientPublicKey,
            string serverPublicKey,
            string acessToken)
        {
            this.StrongKey = strongKey;
            this.ClientPrivateKey = ClientPrivateKey;
            this.ClientPublicKey = clientPublicKey;
            this.ServerPublicKey = serverPublicKey;
            this.AcessToken = acessToken;
        }
    }
}
