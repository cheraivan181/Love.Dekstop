using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Love.Db.Base
{
    public class DbProvider
    {
        public IMongoDatabase GetDatabase()
        {
            var mongoClient = new MongoClient(ConfigurationManager.AppSettings.Get("connectionString"));
            return mongoClient.GetDatabase("loveprojclientdb");
        }
    }
}
