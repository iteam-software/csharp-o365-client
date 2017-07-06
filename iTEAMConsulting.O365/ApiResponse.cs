using iTEAMConsulting.O365.Abstractions;

namespace iTEAMConsulting.O365
{
    public class ApiResponse : IApiResponse
    {
        private readonly int _statusCode;
        private readonly bool _success;

        public ApiResponse(int statusCode)
        {
            _statusCode = statusCode;
            _success = statusCode >= 200 && statusCode < 400;
        }

        public int StatusCode => _statusCode;

        public bool Success => _success;
    }
}
