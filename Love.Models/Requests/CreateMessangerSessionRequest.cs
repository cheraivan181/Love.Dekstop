﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Love.Models.Requests
{
    public class CreateMessangerSessionRequest
    {
        [JsonProperty("PublicKey")]
        public string PublicKey { get; set; } // публичный ключ для клиента
    }
}
