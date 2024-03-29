﻿using Love.Db;
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
    public class UserProvider : BaseProvider
    {
        public async Task<List<User>> GetAllUsersAsync()
        {
            var filter = new BsonDocument();
            var users = await Users.Find(filter).ToListAsync();
           
            return users;
        }

        public async Task<StrongKey> GetStrongKeyAsync(string id)
        {
            var filter = new BsonDocument("_id", id);
            var strongKey = await StrongKeys.Find(filter)
                .FirstOrDefaultAsync();

            return strongKey;
        }

        public async Task CreateSessionAsync(Session session)
        {
            await Sessions.InsertOneAsync(session);
        }

        public async Task<Session> GetSessionAsync(string id)
        {
            var filter = new BsonDocument("_id", id);
            var session = await Sessions.Find(filter)
                .FirstOrDefaultAsync();

            return session;
        }

        public async Task<AuthStorage> GetAuthStorageAsync(string id)
        {
            var filter = new BsonDocument("_id", id);
            var session = await AuthStorages.Find(filter)
                .FirstOrDefaultAsync();

            return session;
        }

        public async Task CreateStrongKeyAsync(string id, byte[] strongKey)
        {
            await StrongKeys.InsertOneAsync(new StrongKey() 
            {
                UserId = id,
                Key = strongKey
            });
        }

        public async Task CreateUserAsync(string id, string login, string hashPassword)
        {
            await Users.InsertOneAsync(new User()
            {
                Id = id,
                Login = login,
                Password = hashPassword
            });
        }

        public async Task СreateOrUpdateAuthStorageAsync(string id, string acessToken, string refresh)
        {
            var authStorage = new AuthStorage()
            {
                UserId = id,
                AcessToken = acessToken,
                RefreshToken = refresh
            };

            var filter = new BsonDocument("_id", id);

            var user = await AuthStorages.Find(filter)
                .FirstOrDefaultAsync();

            if (user != null)
            {
                await UpdateAuthStorageAsync(id, acessToken, refresh);
                return;
            }

            await AuthStorages.InsertOneAsync(authStorage);
        }

        public async Task UpdateAuthStorageAsync(string id,
            string acessToken,
            string refreshToken)
        {
            var filter = new BsonDocument("_id", id);
            await AuthStorages.UpdateOneAsync(filter, Builders<AuthStorage>.Update.Set(x => x.AcessToken, acessToken)
                .Set(x => x.RefreshToken, refreshToken));
        }

    }
}
