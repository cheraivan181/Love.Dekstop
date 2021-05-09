using Love.Db;
using Love.Providers;
using Love.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Love.Services.Http
{
    public class AuthRequest : BaseHttpRequest
    {
        public AuthRequest(string acessToken) : base()
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", acessToken);
        }

    }
}
