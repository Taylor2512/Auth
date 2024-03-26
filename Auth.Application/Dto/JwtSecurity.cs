using Auth.Core.Entity;

using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;


using System.IdentityModel.Tokens.Jwt;

using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Auth.Application.Dto
{
    public class JwtSecurity(IConfiguration _configuration)
    {

        public string GenerateAccessToken(Guid userId)
        {
            return GenerateToken(userId, _configuration["jwt:AccessTokenExpiryTime"]);
        }

        public string GenerateRefreshToken(Guid userId)
        {
            return GenerateToken(userId, _configuration["jwt:RefreshTokenExpiryTime"]);
        }

        public AccessTokenResponse GenerateNewTokens(string refreshToken)
        {
            // Validar el refreshToken
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["jwt:Key"]);
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _configuration["jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = _configuration["jwt:Audience"],
                ValidateLifetime = false // No estamos validando el tiempo de vida aquí
            };

            SecurityToken validatedToken;
            var principal = tokenHandler.ValidateToken(refreshToken, tokenValidationParameters, out validatedToken);

            // Generar nuevo accessToken
            var userId = principal.FindFirst("id").Value;
            var newAccessToken = GenerateAccessToken(Guid.Parse(userId));

            // Generar nuevo refreshToken (invalidar el antiguo)
            var newRefreshToken = GenerateRefreshToken(Guid.Parse(userId));

            return new AccessTokenResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                ExpiresIn = Convert.ToInt32(_configuration["jwt:AccessTokenExpiryTime"])
            };
        }

        private string GenerateToken(Guid userId, string expiryTime)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["jwt:Key"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", userId.ToString()) }),
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(expiryTime)),
                Audience = _configuration["jwt:Audience"],
                Issuer = _configuration["jwt:Issuer"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}