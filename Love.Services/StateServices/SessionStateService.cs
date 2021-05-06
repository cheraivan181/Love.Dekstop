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

        public Task GetStateAsync()
        {
            return stateProvider.GetStateAsync();
        }

        public async Task SetStateAsync(byte[] strongKey, string clientPrivateKey, string clientPublicKey,
            string serverPublicKey,
            string acessToken)
        {
            await stateProvider.SetStateAsync(strongKey, clientPrivateKey, clientPublicKey, serverPublicKey, acessToken);
        }
    }
}
