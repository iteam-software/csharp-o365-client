using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace iTEAMConsulting.O365
{
    public class AdalFactory : IAdalFactory
    {
        public AuthenticationContext CreateAuthenticationContext(string authority)
        {
            return new AuthenticationContext(authority, false);
        }
    }
}
