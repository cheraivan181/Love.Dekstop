using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Love.Db
{
    public class StrongKey
    {
        [BsonId]
        public string UserId { get; set; }
        public byte[] Key { get; set; }
    }
}
