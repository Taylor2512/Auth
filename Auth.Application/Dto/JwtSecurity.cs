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
    /// <summary>
    /// Represents a class that provides JWT security functionality.
    /// </summary>
    public class JwtSecurity(IConfiguration Configuration)
    {
        /// <summary>
        /// Generates an access token for the specified user ID.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>The generated access token.</returns>
        public string GenerateAccessToken(Guid userId)
        {
            return GenerateToken(userId, Configuration["jwt:AccessTokenExpiryTime"] ?? throw new ArgumentException("No se ha ingresado el AccessTokenExpiryTime"));
        }

        /// <summary>
        /// Generates a refresh token for the specified user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>The generated refresh token.</returns>
        public string GenerateRefreshToken(Guid userId)
        {
            return GenerateToken(userId, Configuration["jwt:RefreshTokenExpiryTime"] ?? throw new ArgumentException("No se ha ingresado el RefreshTokenExpiryTime"));
        }

        public AccessTokenResponse GenerateNewTokens(string refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken))
            {
                throw new ArgumentException("El refreshToken no puede ser nulo o vacío");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(Configuration["jwt:Key"] ?? throw new ArgumentException("No se ha ingresado la clave secreta jwt"));
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = Configuration["jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = Configuration["jwt:Audience"],
                ValidateLifetime = false // No estamos validando el tiempo de vida aquí
            };

            SecurityToken validatedToken;
            var principal = tokenHandler.ValidateToken(refreshToken, tokenValidationParameters, out validatedToken);

            var userIdClaim = principal.FindFirst("id");
            if (userIdClaim == null)
            {
                throw new ArgumentException("No se encontró el claim 'id' en el refreshToken");
            }

            var userId = Guid.Parse(userIdClaim.Value);

            var newAccessToken = GenerateAccessToken(userId);
            var newRefreshToken = GenerateRefreshToken(userId);

            return new AccessTokenResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                ExpiresIn = Convert.ToInt32(Configuration["jwt:AccessTokenExpiryTime"])
            };
        }

        /// <summary>
        /// Generates a JWT token for the specified user ID and expiry time.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="expiryTime">The expiry time for the token.</param>
        /// <returns>The generated JWT token.</returns>
        private string GenerateToken(Guid userId, string expiryTime)
        {
            if (string.IsNullOrEmpty(expiryTime))
            {
                throw new ArgumentException("El expiryTime no puede ser nulo o vacío");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(Configuration["jwt:Key"] ?? throw new ArgumentException("No se ha ingresado la clave secreta jwt"));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", userId.ToString()) }),
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(expiryTime)),
                Audience = Configuration["jwt:Audience"],
                Issuer = Configuration["jwt:Issuer"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}