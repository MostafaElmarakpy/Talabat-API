using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Service;

namespace Talabat.Service
{
    public class TokenService : ITokenService
    {
        public IConfiguration Configuration { get; }
        public TokenService(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        public async Task<string> CreateTokenAsync(AppUser user, UserManager<AppUser> userManager)
        {
            var authClaims = new List<Claim>()
            {
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.GivenName, user.DisplayName),

                //new(ClaimTypes.NameIdentifier, user.Id),   
                //new(ClaimTypes.Name, user.UserName),       
                //new("jti", Guid.NewGuid().ToString()),     

            
            };
            var userRoles = await userManager.GetRolesAsync(user);
            foreach (var role in userRoles)
                authClaims.Add(new Claim(ClaimTypes.Role, role));

            //Secret Key
            var authkey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:SecretKey"]));

            var token = new JwtSecurityToken(
                issuer: Configuration["Jwt:ValidIssuer"],
                audience: Configuration["Jwt:ValidAudience"],
                expires: DateTime.Now.AddDays(double.Parse(Configuration["Jwt:TokenLifeTime"])),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authkey, SecurityAlgorithms.HmacSha256Signature)
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
