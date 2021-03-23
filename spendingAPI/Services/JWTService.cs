using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace spendingAPI.Services
{
    public class JWTService : IJWTService
    {
        public string ExtractUserIdFromToken(string authHeader)
        {
            var tokenString = authHeader.Substring(7);
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.ReadJwtToken(tokenString);

            return token.Claims.ToList()[1].Value;
        }

        public dynamic GenerateToken(IdentityUser user)
        {
            //var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("TokenKey")));
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("jasdvjasdpdvjsadjvpsamdvpsofapojasfsdpjcasdmovpadsmvpasdvmsa"));
            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[] {
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                }),
                Expires = DateTime.UtcNow.AddDays(31),
                SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return new { token = tokenHandler.WriteToken(token) };
        }
    }
}
