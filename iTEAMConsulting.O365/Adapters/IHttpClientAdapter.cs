using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace iTEAMConsulting.O365
{
    public interface IHttpClientAdapter
    {
        TimeSpan Timeout { get; set; }
        Uri BaseAddress { get; set; }
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage request);
    }
}
