using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HackerNews.Persistence.Tests
{
    public class MockHttpMessageHandler : DelegatingHandler
    {
        private Dictionary<string, HttpResponseMessage> _mockResponse;

        public MockHttpMessageHandler(IEnumerable<(string url, object value)> responses)
        {
            var responsesDict = responses.ToDictionary(kv => kv.url, kv =>
            {
                return new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(kv.value), Encoding.UTF8, "application/json")
                };
            });
            _mockResponse = responsesDict;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var url = request.RequestUri.OriginalString;
            var value = _mockResponse.ContainsKey(url) ? _mockResponse[url] : null;
            return await Task.FromResult(value);
        }
    }
}
