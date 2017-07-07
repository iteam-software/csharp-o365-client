using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTEAMConsulting.O365
{
    public class AuthenticationResultAdapter : IAuthenticationResultAdapter
    {
        public string AccessToken { get; set; }
    }
}
