using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace iTEAMConsulting.O365
{
    public class AdalFactory : IAdalFactory
    {
        public IAuthenticationContextAdapter CreateAuthenticationContext(string authority)
        {
            var context = new AuthenticationContext(authority, false);
            return new AuthenticationContextAdapter(context);
        }
    }
}
