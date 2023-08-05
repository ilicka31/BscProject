using Common.Authentication.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.WebSockets;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Common.Authentication
{
    public class JwtTokenHandler
    {
        public const string JWT_SECURITY_KEY = "kljuc";
        private const int JWT_TOKEN_VALIDITY_MINS = 20;

        private readonly List<UserAccount> _userAccounts;
        public JwtTokenHandler()
        {
            _userAccounts = new List<UserAccount>();  
        }

        public AuthenticationResponse GenerateJwtToken(AuthenticationRequest request)
        {
            if (string.IsNullOrEmpty(request.UserName) || string.IsNullOrEmpty(request.Password))
              return null;

            var userAccount = _userAccounts.Where(x => x.UserName == request.UserName && x.Password == request.Password).FirstOrDefault();
            if (userAccount == null)
                return null;
            var expiryTimeStamp = DateTime.Now.AddMinutes(JWT_TOKEN_VALIDITY_MINS);
            var tokenKey = Encoding.ASCII.GetBytes(JWT_SECURITY_KEY);
            var claimsIdentity = new ClaimsIdentity(new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Name, request.UserName),
                new Claim("Role", userAccount.Role)
            });
            var signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature);

            var secutirityTokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claimsIdentity,
                Expires = expiryTimeStamp,
                SigningCredentials = signingCredentials
            };

            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var securityToken = jwtSecurityTokenHandler.CreateToken(secutirityTokenDescriptor);
            var token = jwtSecurityTokenHandler.WriteToken(securityToken);

            return new AuthenticationResponse
            {
                UserName = userAccount.UserName,
                ExpireIn = (int)expiryTimeStamp.Subtract(DateTime.Now).TotalSeconds,
                JwtToken = token,
            };
            
        }
    }
}
