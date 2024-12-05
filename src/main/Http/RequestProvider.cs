﻿//MIT License

//Copyright(c) .NET Foundation and Contributors

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.
//
// https://github.com/dotnet-architecture/eShopOnContainers
//
// Modifications copyright(C) 2018 ei8/Elmer Bool

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace neurUL.Common.Http
{
    [Obsolete("Use IHttpClientFactory instead? See ei8.EventSourcing.Client.")]
    public class RequestProvider : IRequestProvider
    {
        private readonly JsonSerializerSettings serializerSettings;
        private HttpClientHandler clientHandler;

        public RequestProvider()
        {
            this.serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                NullValueHandling = NullValueHandling.Ignore
            };
            this.serializerSettings.Converters.Add(new StringEnumConverter());
        }

        public async Task<TResult> GetAsync<TResult>(string uri, string bearerToken = "", CancellationToken token = default(CancellationToken))
        {
            return await RequestProvider.SendRequest<TResult>(
                this.GetHttpClientWithBearerToken(bearerToken),
                WebRequestMethods.Http.Get,
                uri,
                this.serializerSettings,
                token: token
                );
        }

        public async Task<TResult> PostAsync<TResult>(string uri, TResult data, string token = "", string header = "")
        {
            HttpClient httpClient = GetHttpClientWithBearerToken(token);

            if (!string.IsNullOrEmpty(header))
            {
                AddHeaderParameter(httpClient, header);
            }

            var content = new StringContent(JsonConvert.SerializeObject(data));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = await httpClient.PostAsync(uri, content);

            await RequestProvider.HandleResponse(response, uri);
            string serialized = await response.Content.ReadAsStringAsync();

            TResult result = await Task.Run(() =>
                JsonConvert.DeserializeObject<TResult>(serialized, serializerSettings));

            return result;
        }

        public async Task<TResult> PostAsync<TResult>(string uri, string data, string clientId, string clientSecret)
        {
            HttpClient httpClient = GetHttpClientWithBearerToken(string.Empty);

            if (!string.IsNullOrWhiteSpace(clientId) && !string.IsNullOrWhiteSpace(clientSecret))
            {
                AddBasicAuthenticationHeader(httpClient, clientId, clientSecret);
            }

            var content = new StringContent(data);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
            HttpResponseMessage response = await httpClient.PostAsync(uri, content);

            await RequestProvider.HandleResponse(response, uri);
            string serialized = await response.Content.ReadAsStringAsync();

            TResult result = await Task.Run(() =>
                JsonConvert.DeserializeObject<TResult>(serialized, serializerSettings));

            return result;
        }

        public async Task<TResult> PostAsync<TResult>(string uri, object data, string bearerToken = "", CancellationToken token = default(CancellationToken), params KeyValuePair<string, string>[] headers)
            => await RequestProvider.SendRequest<TResult>(
                this.GetHttpClientWithBearerToken(bearerToken),
                WebRequestMethods.Http.Post,
                uri,
                this.serializerSettings,
                data,
                token,
                headers
                );

        public async Task<TResult> PutAsync<TResult>(string uri, object data, string bearerToken = "", CancellationToken token = default(CancellationToken), params KeyValuePair<string, string>[] headers)
            => await RequestProvider.SendRequest<TResult>(
                this.GetHttpClientWithBearerToken(bearerToken),
                WebRequestMethods.Http.Put,
                uri,
                this.serializerSettings,
                data,
                token,
                headers
                );

        public async Task<TResult> PatchAsync<TResult>(string uri, object data, string bearerToken = "", CancellationToken token = default(CancellationToken), params KeyValuePair<string, string>[] headers)
            => await RequestProvider.SendRequest<TResult>(
                this.GetHttpClientWithBearerToken(bearerToken),
                "PATCH",
                uri,
                this.serializerSettings,
                data,
                token,
                headers
                );

        public async Task<TResult> DeleteAsync<TResult>(string uri, object data, string bearerToken = "", CancellationToken token = default(CancellationToken), params KeyValuePair<string, string>[] headers)
            => await RequestProvider.SendRequest<TResult>(
                this.GetHttpClientWithBearerToken(bearerToken),
                "DELETE",
                uri,
                this.serializerSettings,
                data,
                token,
                headers
                );

        private static async Task<TResult> SendRequest<TResult>(HttpClient httpClient, string method, string uri, JsonSerializerSettings serializerSettings, object data = default, CancellationToken token = default(CancellationToken), params KeyValuePair<string, string>[] headers)
        {

            HttpRequestMessage msg = new HttpRequestMessage
            {
                Method = new HttpMethod(method),
                RequestUri = new Uri(uri, UriKind.Absolute)
            };
            headers.ToList().ForEach(h => msg.Headers.Add(h.Key, h.Value));

            if (data != null)
            {
                var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                msg.Content = content;
            }

            var response = await httpClient.SendAsync(msg, token);

            await RequestProvider.HandleResponse(response, uri);
            string serialized = await response.Content.ReadAsStringAsync();

            TResult result = default;
            
            if (!string.IsNullOrEmpty(serialized))
                result = JsonConvert.DeserializeObject<TResult>(serialized, serializerSettings);

            return result;
        }

        private HttpClient httpClient;

        private HttpClient GetHttpClientWithBearerToken(string bearerToken = "")
        {
            var result = this.HttpClient;
            if (!string.IsNullOrEmpty(bearerToken))
                result.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

            return result;
        }

        private void AddHeaderParameter(HttpClient httpClient, string parameter)
        {
            if (httpClient == null)
                return;

            if (string.IsNullOrEmpty(parameter))
                return;

            httpClient.DefaultRequestHeaders.Add(parameter, Guid.NewGuid().ToString());
        }

        private void AddBasicAuthenticationHeader(HttpClient httpClient, string clientId, string clientSecret)
        {
            if (httpClient == null)
                return;

            if (string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(clientSecret))
                return;

            httpClient.DefaultRequestHeaders.Authorization = new BasicAuthenticationHeaderValue(clientId, clientSecret);
        }

        private async static Task HandleResponse(HttpResponseMessage response, string uri)
        {
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var exceptionMessage = $"Uri: {uri}; Response: {content}";

                if (response.StatusCode == HttpStatusCode.Forbidden ||
                    response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new ServiceAuthenticationException(exceptionMessage);
                }

                throw new HttpRequestExceptionEx(response.StatusCode, exceptionMessage);
            }
        }

        public HttpClient HttpClient
        {
            get
            {
                if (this.httpClient == null)
                    this.httpClient = new HttpClient(this.clientHandler);
                else
                    this.httpClient.DefaultRequestHeaders.Clear();

                this.httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                return this.httpClient;
            }
        }

        public void SetHttpClientHandler(HttpClientHandler clientHandler)
        {
            this.clientHandler = clientHandler;
        }
    }
}
