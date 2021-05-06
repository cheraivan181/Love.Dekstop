using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Love.Db
{
    public class User
    {
        [BsonId]
        public string Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
    }
}
