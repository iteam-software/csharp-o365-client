using System;
using System.Net.Http;

namespace iTEAMConsulting.O365
{
    public class BackchannelFactory : IBackchannelFactory
    {
        public IHttpClientAdapter CreateBackchannel(string baseAddress)
        {
            var client = new HttpClientAdapter(new HttpClient());
            client.Timeout = TimeSpan.FromSeconds(30);
            client.BaseAddress = new Uri(baseAddress);

            return client;
        }
    }
}
