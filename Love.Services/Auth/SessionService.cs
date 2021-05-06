using Love.Db;
using Love.Models.Requests;
using Love.Models.Responses;
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
        private readonly TokenService tokenService;
        private readonly UserProvider userProvider;

        public string UserId { get; set; }

        public SessionService(string userId)
        {
            UserId = userId;

            baseHttpRequest = new BaseHttpRequest();
            userProvider = new UserProvider();
            tokenService = new TokenService(userId);
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

                var firstSessionResponse = await baseHttpRequest.GetStringFromHttpResultAsync(Urls.CreateFirstSessionUrl, HttpMethod.Post, jsonRequest);
                var response = JsonConvert.DeserializeObject<CreateFirstMessangerSessionResponse>(firstSessionResponse);

                string decryptedAesKey = rsa.Decrypt(rsaPair.privateKey, response.CryptedAes);

                byte[] decryptedAesKeyBuffer = decryptedAesKey.FromUrlSafeBase64();
                await tokenService.MakeAuthTokenAsync(UserId, true);

                rsaPair = rsa.GenerateKeys();
                var aes = new AesCrypt();

                string cryptedPublicKey = aes.Crypt(decryptedAesKeyBuffer.ToUrlSafeBase64(), rsaPair.publicKey);
                var sessionRequestModel = new CreateMessangerSessionRequest()
                {
                    PublicKey = cryptedPublicKey
                };

                jsonRequest = JsonConvert.SerializeObject(sessionRequestModel);

                var httpRequest = baseHttpRequest.BuildRequestMessage(Urls.CreateSessionUrl, HttpMethod.Post, jsonRequest);
                var sessionResponse = await baseHttpRequest.httpClient.SendAsync(httpRequest);

                sessionResponse.EnsureSuccessStatusCode();

                var session = JsonConvert.DeserializeObject<CreateMessangerSessionResponse>(await sessionResponse.Content.ReadAsStringAsync());
                string decryptedServerPublicKey = aes.Decrypt(decryptedAesKey, session.ServerPublicKey);
                string decryptedSessionId = aes.Decrypt(decryptedAesKey, session.SessionId);

                await userProvider.CreateSessionAsync(new Session()
                {
                    ClientPrivateKey = rsaPair.privateKey,
                    ServerPublicKey = decryptedServerPublicKey,
                    ClientPublicKey = rsaPair.publicKey,
                    UserId = UserId,
                    SessionId = decryptedSessionId
                });
            }
        }
    }
}
