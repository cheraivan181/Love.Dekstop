using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Love.Models.Responses
{
    public class CreateFirstMessangerSessionResponse
    {
        public string ServerPublicKey { get; set; }
        public string CryptedAes { get; set; }

    }
    public class CreateMessangerSessionResponse
    {
        public string ServerPublicKey { get; set; }
        public string SessionId { get; set; }
    }
}
