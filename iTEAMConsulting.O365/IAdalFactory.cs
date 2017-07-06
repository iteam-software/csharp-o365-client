using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Text;

namespace iTEAMConsulting.O365
{
    public interface IAdalFactory
    {
        AuthenticationContext CreateAuthenticationContext(string authority);
    }
}
