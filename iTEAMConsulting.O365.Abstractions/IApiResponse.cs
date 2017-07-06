using System;
using System.Collections.Generic;
using System.Text;

namespace iTEAMConsulting.O365.Abstractions
{
    public interface IApiResponse
    {
        int StatusCode { get; }
        bool Success { get; }
    }
}
