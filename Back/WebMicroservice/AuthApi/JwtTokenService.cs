using AuthApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
namespace AuthApi
{
    public class JwtTokenService
    {
      /*  private readonly List<User> _users = new()
        {
        new("admin", "aDm1n", "Administrator", new[] { "shoes.read" }),
        new("user01", "u$3r01", "User", new[] { "shoes.read" })
        };*/
        private readonly List<User> _users;
        public AuthToken? GenerateAuthToken(LoginModel loginModel)
        {
            var user = _users.FirstOrDefault(u => u.Email == loginModel.Email && u.Password == loginModel.Password);

            if (user is null)
            {
                return null;
            }

            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtExtensions.SecurityKey));
            var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            var expirationTimeStamp = DateTime.Now.AddMinutes(5);

            var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Name, user.Email),
            new Claim("role", user.Role)  
        };

            var tokenOptions = new JwtSecurityToken(
                issuer: "https://localhost:5002",
                claims: claims,
                expires: expirationTimeStamp,
                signingCredentials: signingCredentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

            return new AuthToken(tokenString, (int)expirationTimeStamp.Subtract(DateTime.Now).TotalSeconds);
        }
    }

    public static class JwtExtensions
    {
        public const string SecurityKey = "secretJWTsigningKey@123";

        public static void AddJwtAuthentication(this IServiceCollection services)
        {
            services.AddAuthentication(opt => {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = "https://localhost:5002",
                    ValidateAudience = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecurityKey))
                };
            });
        }

    }
}
