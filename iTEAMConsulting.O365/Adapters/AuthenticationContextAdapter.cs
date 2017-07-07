using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Threading.Tasks;

namespace iTEAMConsulting.O365
{
    public class AuthenticationContextAdapter : IAuthenticationContextAdapter
    {
        private readonly AuthenticationContext _context;

        public AuthenticationContextAdapter(AuthenticationContext context)
        {
            _context = context;
        }

        public async Task<IAuthenticationResultAdapter> AcquireTokenAsync(string resource, ClientCredential credential)
        {
            return (await _context.AcquireTokenAsync(resource, credential)).ToAdapterResult();
        }
    }
}
