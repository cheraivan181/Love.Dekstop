using Love.Models.Responses;
using Love.Providers;
using Love.Services.Http;
using Love.Utils;
using MongoDB.Bson;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Love.Services.Auth
{
    public class TokenService
    {
        private AuthRequest authRequest;
        private readonly UserProvider userProvider;

        public string UserId { get; set; }
            
        public TokenService(string userId)
        {
            userProvider = new UserProvider();
            UserId = userId;
        }
        
        public async Task<string> MakeAuthTokenAsync(string userId,
            bool isNeedAuth = false)
        {
            var authStorage = await userProvider.GetAuthStorageAsync(userId);

            authRequest = new AuthRequest(authStorage.AcessToken);
            var requestMessage = new HttpRequestMessage();
            requestMessage.Method = HttpMethod.Get;
            requestMessage.RequestUri = new Uri(ConfigurationManager.AppSettings.Get("devUrl") + Urls.GetAuthUserInfoUrl);

            var responseUserInfo = await authRequest.httpClient.SendAsync(requestMessage);

            if (responseUserInfo.StatusCode == HttpStatusCode.OK && !isNeedAuth)
                return authStorage.AcessToken;

            string content = JsonConvert.SerializeObject(new
            {
                refreshToken = authStorage.RefreshToken
            });

            var updateLongSessionRequestMessage = new HttpRequestMessage();

            updateLongSessionRequestMessage.Method = HttpMethod.Post;
            updateLongSessionRequestMessage.RequestUri = new Uri(ConfigurationManager.AppSettings.Get("devUrl") + Urls.LongSessionUpdateUrl);
            updateLongSessionRequestMessage.Content = new StringContent(content, Encoding.UTF8, "application/json");

            var updateLongSessionResponse = await authRequest.httpClient.SendAsync(updateLongSessionRequestMessage);
            if (updateLongSessionResponse.StatusCode == HttpStatusCode.OK)
            {
                var authResult = JsonConvert.DeserializeObject<AuthResult>(await updateLongSessionResponse.Content.ReadAsStringAsync());
                await userProvider.UpdateAuthStorageAsync(UserId, authResult.AccessToken, authResult.RefreshToken);

                return authResult.AccessToken;
            }

            throw new Exception("Cannot update long session");
        }
    }
}
