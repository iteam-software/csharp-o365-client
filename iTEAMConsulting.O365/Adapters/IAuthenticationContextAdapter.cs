using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTEAMConsulting.O365
{
    public interface IAuthenticationContextAdapter
    {
        Task<IAuthenticationResultAdapter> AcquireTokenAsync(string resource, ClientCredential credential);
    }
}
