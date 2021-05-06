using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Love.Db
{
    public class SessionState
    {
        public ObjectId Id { get; set; }
        public byte[] StrongKey { get; set; }
        public string ClientPrivateKey { get; set; }
        public string ClientPublicKey { get; set; }
        public string ServerPublicKey { get; set; }
        public string AcessToken { get; set; }
    }
}
