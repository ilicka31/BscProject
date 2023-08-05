using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Authentication.Models
{
    public class AuthenticationResponse
    {
        public string UserName { get; set; }
        public string JwtToken { get; set; }
        public int ExpireIn { get; set; }
    }
}
