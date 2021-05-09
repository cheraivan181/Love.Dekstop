using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Love.Db
{
    public class Session
    {
        [BsonId]
        public string UserId { get; set; }
        public string ClientPublicKey { get; set; }
        public string ClientPrivateKey { get; set; }
        public string ServerPublicKey { get; set; }
    }
}
