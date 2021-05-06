using Love.Models.Requests;
using Love.Providers;
using Love.Services.Crypt;
using Love.Services.Http;
using Love.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Love.Services.Auth
{
    public class SessionService
    {
        private readonly BaseHttpRequest baseHttpRequest;
        private readonly UserProvider userProvider;

        public string UserId { get; set; }

        public SessionService(string userId)
        {
            UserId = userId;

            baseHttpRequest = new BaseHttpRequest();
            userProvider = new UserProvider();
        }

        public async Task MakeSessionAsync()
        {
            var rsa = new RsaService();
            var strongKey = await userProvider.GetStrongKeyAsync(UserId);

            var rsaPair = rsa.GenerateKeys();

            if (strongKey == null)
            {
                var firstSessionRequestModel = new CreateMessangerSessionRequest()
                {
                    PublicKey = rsaPair.publicKey
                };

                string jsonRequest = JsonConvert.SerializeObject(firstSessionRequestModel);

                //var firstSessionResponse = await baseHttpRequest.GetStringFromHttpResultAsync(Urls.CreateFirstSessionUrl, HttpMethod.Post, jsonRequest);
                //var response = JsonConvert.DeserializeObject<CreateFirstMessangerSessionResponse>(firstSessionResponse);


            }
            else
            {

            }
        }
    }
}
