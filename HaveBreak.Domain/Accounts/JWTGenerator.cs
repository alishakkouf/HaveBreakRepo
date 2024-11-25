using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HaveBreak.Shared.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace HaveBreak.Domain.Accounts
{
    public class JWTGenerator
    {
        public static string RandomString(int length)
        {
            var random = new Random();
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public static TokenDomain GenerateJWTToken(CreateTokenRequest userInfo, IConfiguration appSettings)
        {
            var key = appSettings["Jwt:Key"];

            if (string.IsNullOrEmpty(key) || key.Length < 32)
            {
                throw new BusinessException("The Jwt:Key must be at least 32 characters long.");
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, userInfo.UserId.ToString()),
                new(JwtRegisteredClaimNames.Sub, userInfo.Email),
                new("IsActive"  , userInfo.IsActive.ToString()),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            for (int i = 0; i < (userInfo?.Roles?.Count ?? 0); i++)
            {
                var claim = new Claim("role", userInfo.Roles[i]);
                claims.Add(claim);
            }
            var token = new JwtSecurityToken(
                issuer: appSettings["Jwt:Issuer"],
                audience: appSettings["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(7),
                signingCredentials: credentials
            );
            string tok = new JwtSecurityTokenHandler().WriteToken(token);

            return new TokenDomain
            {
                AccessToken = tok,
                ExpiresIn = 86400
            };
        }


    }
}
