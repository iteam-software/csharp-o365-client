using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace iTEAMConsulting.O365
{
    public class HttpClientAdapter : IHttpClientAdapter
    {
        private readonly HttpClient _client = new HttpClient();

        public HttpClientAdapter(HttpClient client)
        {
            _client = client;
        }

        public TimeSpan Timeout {
            get
            {
                return _client.Timeout;
            }
            set
            {
                _client.Timeout = value;
            }
        }

        public Uri BaseAddress {
            get
            {
                return _client.BaseAddress;
            }
            set
            {
                _client.BaseAddress = value;
            }
        }

        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
        {
            return await _client.SendAsync(request);
        }
    }
}
