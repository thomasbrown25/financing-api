using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace financing_api.Services.UserService
{
    public class JwtGenerator
    {
        readonly RsaSecurityKey _key;
        public JwtGenerator(string signingKey)
        {
            RSA privateRSA = RSA.Create();
            privateRSA.ImportRSAPrivateKey(Convert.FromBase64String(signingKey), out _);
            _key = new RsaSecurityKey(privateRSA);
        }

        public string CreateUserAuthToken(string userEmail)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Audience = "myApi",
                Issuer = "UserService",
                Subject = new ClaimsIdentity(
                [
                            new Claim(ClaimTypes.Sid, userEmail.ToString())
                ]),
                Expires = DateTime.UtcNow.AddMinutes(60),
                SigningCredentials = new SigningCredentials(_key, SecurityAlgorithms.RsaSha256)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}