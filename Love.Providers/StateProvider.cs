using Love.Db;
using Love.Providers.Base;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Love.Providers
{
    public class StateProvider : BaseProvider
    {

        public Task<SessionState> GetStateAsync()
        {
            return SessionStates.Find(new BsonDocument())
                .FirstOrDefaultAsync();
        }

        public async Task SetStateAsync(byte[] strongKey, string clientPrivateKey, string clientPublicKey,
           string serverPublicKey,
           string acessToken)
        {
            var sessionState = await SessionStates.Find(new BsonDocument())
                .FirstOrDefaultAsync();
            
            sessionState = new SessionState();
            bool isNewSession = false;

            if (sessionState == null)
            {
                sessionState = new SessionState();
                isNewSession = true;
            }

            sessionState.StrongKey = strongKey;
            sessionState.ClientPrivateKey = clientPrivateKey;
            sessionState.ClientPublicKey = clientPublicKey;
            sessionState.ServerPublicKey = serverPublicKey;
            sessionState.AcessToken = acessToken;

            if (isNewSession)
                await SessionStates.InsertOneAsync(sessionState);
            else
                await SessionStates.UpdateOneAsync(new BsonDocument(), 
                    Builders<SessionState>.Update.Set(x => x.StrongKey, strongKey)
                        .Set(x => x.ClientPrivateKey, clientPrivateKey)
                        .Set(x => x.ClientPublicKey, clientPublicKey)
                        .Set(x => x.ServerPublicKey, serverPublicKey)
                        .Set(x => x.AcessToken, acessToken));
        }
    }
}
