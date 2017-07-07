using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace iTEAMConsulting.O365
{
    public static class AuthenticationResultExtensions
    {
        public static IAuthenticationResultAdapter ToAdapterResult(this AuthenticationResult result)
        {
            return new AuthenticationResultAdapter
            {
                AccessToken = result.AccessToken
            };
        }
    }
}
