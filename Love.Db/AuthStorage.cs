using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Love.Db
{
    public class AuthStorage
    {
        [BsonId]
        public string UserId { get; set; }
        public string AcessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
