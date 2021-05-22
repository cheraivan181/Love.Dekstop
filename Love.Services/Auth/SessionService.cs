using Love.Db;
using Love.Models.Requests;
using Love.Models.Responses;
using Love.Providers;
using Love.Services.Crypt;
using Love.Services.Http;
using Love.Services.StateServices;
using Love.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Love.Services.Auth
{
    public class SessionService
    {
        private readonly TokenService tokenService;
        private readonly UserProvider userProvider;

        public string UserId { get; set; }

        public SessionService(string userId)
        {
            UserId = userId;

            userProvider = new UserProvider();
            tokenService = new TokenService(userId);
        }

        public async Task MakeSessionAsync(string acessToken, string refreshToken = null)
        {
            var rsa = new RsaService();
            var aes = new AesCrypt();

            var rsaPair = rsa.GenerateKeys();

            var strongKeyRequest = new
            {
                publicKey = rsaPair.publicKey
            };

            var authRequest = new AuthRequest(acessToken);

            string strongKeyJsonRequest = JsonConvert.SerializeObject(strongKeyRequest);
            var strongKeyRequestMessage = authRequest.BuildRequestMessage(ConfigurationManager.AppSettings.Get("devUrl") + Urls.GetStrongKeyUrl, HttpMethod.Post, strongKeyJsonRequest);

            var strongKeyResponseMessage = await authRequest.httpClient.SendAsync(strongKeyRequestMessage);

            if (strongKeyResponseMessage.StatusCode == HttpStatusCode.NotFound)
            {
                var firstSessionRequestModel = new CreateMessangerSessionRequest()
                {
                    PublicKey = rsaPair.publicKey
                };

                string jsonRequest = JsonConvert.SerializeObject(firstSessionRequestModel);

                var firstSessionResponse = await authRequest.GetStringFromHttpResultAsync(ConfigurationManager.AppSettings.Get("devUrl") + Urls.CreateFirstSessionUrl, HttpMethod.Post, jsonRequest);
                var response = JsonConvert.DeserializeObject<CreateFirstMessangerSessionResponse>(firstSessionResponse);

                string decryptedAesKey = rsa.Decrypt(rsaPair.privateKey, response.CryptedAes);

                byte[] decryptedAesKeyBuffer = decryptedAesKey.FromUrlSafeBase64();

                await userProvider.CreateStrongKeyAsync(UserId, decryptedAesKeyBuffer);

                string newToken = await tokenService.MakeAuthTokenAsync(UserId, true);
                authRequest = new AuthRequest(newToken);

                rsaPair = rsa.GenerateKeys();
               
                string cryptedPublicKey = aes.Crypt(decryptedAesKeyBuffer.ToUrlSafeBase64(), rsaPair.publicKey);
                var sessionRequestModel = new CreateMessangerSessionRequest()
                {
                    PublicKey = cryptedPublicKey
                };

                jsonRequest = JsonConvert.SerializeObject(sessionRequestModel);

                var httpRequest = authRequest.BuildRequestMessage(ConfigurationManager.AppSettings.Get("devUrl") + Urls.CreateSessionUrl, HttpMethod.Post, jsonRequest);
                var sessionResponse = await authRequest.httpClient.SendAsync(httpRequest);

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
            else if (!string.IsNullOrEmpty(refreshToken) && strongKeyResponseMessage.StatusCode == HttpStatusCode.OK)
            {
                var strongKeyResponse = JsonConvert.DeserializeObject<GetStrongKeyResponse>(
                    await strongKeyResponseMessage.Content.ReadAsStringAsync()
                );

                var decryptedStrongKey = rsa.Decrypt(rsaPair.privateKey, strongKeyResponse.StrongKey);
                await userProvider.CreateStrongKeyAsync(UserId, decryptedStrongKey.FromUrlSafeBase64()); 

                rsaPair = rsa.GenerateKeys();
                var cryptedPublicKey = aes.Crypt(decryptedStrongKey, rsaPair.publicKey);

                var sessionRequest = new CreateMessangerSessionRequest() 
                {
                    PublicKey = cryptedPublicKey
                };

                string jsonSessionRequest = JsonConvert.SerializeObject(sessionRequest);
                var sessionResponse = await authRequest.MakeRequestAsync<CreateMessangerSessionResponse>(ConfigurationManager.AppSettings.Get("devUrl") + Urls.CreateSessionUrl, HttpMethod.Post, jsonSessionRequest);

                string decryptedPublicKey = aes.Decrypt(decryptedStrongKey, sessionResponse.ServerPublicKey);
                string decryptedSessionId = aes.Decrypt(decryptedStrongKey, sessionResponse.SessionId);

                await userProvider.CreateSessionAsync(new Session()
                {
                    ClientPrivateKey = rsaPair.privateKey,
                    ClientPublicKey = rsaPair.publicKey,
                    ServerPublicKey = decryptedPublicKey,
                    UserId = UserId,
                    SessionId = decryptedSessionId
                });
            }
        }

        public async Task SetSessionAsync(string acessToken)
        {
            var authRequest = new AuthRequest(acessToken);
            var session = await userProvider.GetSessionAsync(UserId);
            var aes = new AesCrypt();

            var stateContainer = StateContainer.GetStateContainer();

            string cryptedSession = aes.Crypt(stateContainer.sessionStateService.StrongKey, session.SessionId);

            await authRequest.MakeRequestAsync(ConfigurationManager.AppSettings.Get("devUrl") + Urls.SetServerSessionUrl + cryptedSession, HttpMethod.Get, null);
        }
    }
}
