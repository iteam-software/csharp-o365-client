using iTEAMConsulting.O365.Abstractions;

namespace iTEAMConsulting.O365
{
    public class LoginResponse : ILoginResponse
    {
        private readonly string _accessToken;
        private readonly bool _success;
        private readonly int _statusCode;

        public LoginResponse(string accessToken)
        {
            _accessToken = accessToken;
            _success = !string.IsNullOrWhiteSpace(accessToken);
            _statusCode = _success ? 200 : -1;
        }

        public string AccessToken => _accessToken;

        public int StatusCode => _statusCode;

        public bool Success => _success;
    }
}
