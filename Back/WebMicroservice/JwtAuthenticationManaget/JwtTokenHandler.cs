using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JwtAuthenticationManaget
{
    internal class JwtTokenHandler
    {
        public const string JWT_SECURITY_KEY = Configuration.GetSection("SecurityKey")
    }
}
