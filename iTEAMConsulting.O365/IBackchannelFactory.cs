using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace iTEAMConsulting.O365
{
    public interface IBackchannelFactory
    {
        IHttpClientAdapter CreateBackchannel(string baseAddress);
    }
}
