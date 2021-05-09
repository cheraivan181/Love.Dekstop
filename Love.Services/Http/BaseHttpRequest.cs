using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Love.Services.Http
{
    public class BaseHttpRequest
    {
        public HttpClient httpClient;

        public BaseHttpRequest()
        {
            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(ConfigurationManager.AppSettings.Get("devUrl"));
        }

        protected HttpRequestMessage BuildRequestMessage(string uri,
            HttpMethod method,
            string content = null)
        {
            var httpRequestMessage = new HttpRequestMessage();

            httpRequestMessage.RequestUri = new Uri(uri);
            httpRequestMessage.Method = method;

            if (!string.IsNullOrEmpty(content))
            {
                StringContent httpContent = new StringContent(content, Encoding.UTF8, "application/json");
                httpRequestMessage.Content = httpContent;
            }

            return httpRequestMessage;
        }

        public async Task<string> GetStringFromHttpResultAsync(string uri, HttpMethod method, string content = null)
        {
            var request = BuildRequestMessage(uri, method, content);
            var response = await httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();

            return result;
        }


        private async Task<HttpResponseMessage> MakeSimpleFormHttpRequestAsync(string url, Dictionary<string, string> values)
        {
            var httpRequestMessage = new HttpRequestMessage();
            httpRequestMessage.Method = HttpMethod.Post;

            var form = new MultipartFormDataContent();

            foreach (KeyValuePair<string, string> value in values)
                form.Add(new StringContent(value.Value), value.Key);

            httpRequestMessage.RequestUri = new Uri(url);
            httpRequestMessage.Content = form;

            var response = await httpClient.SendAsync(httpRequestMessage);
            return response;
        }

        public async Task MakeSimpleRequestAsync()
        {
            return;
        }

        public async Task<T> MakeRequestAsync<T>(string url, HttpMethod method, string body)
        {
            string message = await GetStringFromHttpResultAsync(url, method, body);
            return JsonConvert.DeserializeObject<T>(message);
        }

        public async Task<T> MakeFormRequestAsync<T>(string url, Dictionary<string, string> parameters)
        {
            var response = await MakeSimpleFormHttpRequestAsync(url, parameters);
            await HttpErrorHandle(response);
            string content = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<T>(content);
        }

        public async Task MakeRequestAsync(string url, HttpMethod method, string body)
        {
            var message = BuildRequestMessage(url, method, body);
            var response = await httpClient.SendAsync(message);
            await HttpErrorHandle(response);
        }

        protected async Task HttpErrorHandle(HttpResponseMessage httpResponseMessage)
        {
            if (httpResponseMessage.StatusCode == HttpStatusCode.NotFound)
                throw new Exception($"Url adress not found: {httpResponseMessage.RequestMessage.RequestUri}");
            if (httpResponseMessage.StatusCode == HttpStatusCode.InternalServerError)
            {
                var responseMessage = await httpResponseMessage.Content.ReadAsStringAsync();
                throw new Exception($"Internal erver error.\nMessage: {responseMessage}");
            }
        }
    }
}
