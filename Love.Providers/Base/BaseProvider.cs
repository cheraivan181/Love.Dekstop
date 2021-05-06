using Love.Db;
using Love.Db.Base;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Love.Providers.Base
{
    public class BaseProvider
    {
        private DbProvider dbProvider = new DbProvider();

        protected IMongoCollection<User> Users
        {
            get
            {
                return dbProvider.GetDatabase().GetCollection<User>("Users");
            }
        } 

        protected IMongoCollection<AuthStorage> AuthStorages
        {
            get
            {
                return dbProvider.GetDatabase().GetCollection<AuthStorage>("authStorages");
            }
        }

        protected IMongoCollection<Session> Sessions
        {
            get
            {
                return dbProvider.GetDatabase().GetCollection<Session>("sessions");
            }
        }

        protected IMongoCollection<StrongKey> StrongKeys
        {
            get
            {
                return dbProvider.GetDatabase().GetCollection<StrongKey>("strongKeys");
            }
        }

        protected IMongoCollection<SessionState> SessionStates
        {
            get
            {
                return dbProvider.GetDatabase().GetCollection<SessionState>("sessionStates");
            }
        }
    }
}
