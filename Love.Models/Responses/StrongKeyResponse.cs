using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Love.Models.Responses
{
    public class GetStrongKeyResponse
    {
        [JsonProperty("strongKey")]
        public string StrongKey { get; set; }
    }
}
