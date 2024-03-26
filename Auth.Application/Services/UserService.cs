using Auth.Application.Dto;
using Auth.Application.Dto.Request;
using Auth.Application.Interface;
using Auth.Core.Entity;
using Auth.Infrastructure.Persistence;

using AutoMapper;

using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Auth.Application.Services
{
    public class UserService(ApplicationDbContext _context, IConfiguration _configuration, IMapper _mapper, JwtSecurity _jwtSecurity) : IUserService
    {
        public async Task<UserDto> Login(UserRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.IsActive && x.Email == request.Email);
            if (user == null)
            {
                throw new InvalidDataException("Usuario no encontrado");
            }
            else
            {
                if (request.ValidatePassword(user.Password, user.Salt))
                {
                    // Generar token de acceso
                    string accessToken = _jwtSecurity.GenerateAccessToken(user.Id);
                    // Generar token de actualización
                    string refreshToken = _jwtSecurity.GenerateRefreshToken(user.Id);

                    // Devolver el usuario y los tokens generados
                    UserDto userDto = _mapper.Map<UserDto>(user);
                    AccessTokenResponse tokenResponse= new() { 
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    ExpiresIn =Convert.ToInt32( _configuration["jwt:AccessTokenExpiryTime"]),

                    };
                     userDto.Token = tokenResponse;
 
                    return userDto;
                }
                else
                {
                    throw new InvalidDataException("Contraseña incorrecta");
                }
            }
        }

        private bool IfExistEmail(string email)
        {
            return _context.Users.Any(x => x.Email == email);
        }

        public async Task RegistryUser(UserRequest request)
        {
            if (IfExistEmail(request.Email))
            {
                throw new InvalidDataException("El correo ya está registrado");
            }

            request.Excecute();
            Users usuarionuevo = _mapper.Map<Users>(request);
            _ = _context.Users.Add(usuarionuevo);
            _ = await _context.SaveChangesAsync();
        }
        public async Task<AccessTokenResponse> GenerateRefreshToken(string refreshToken)
        {
            var newTokens = _jwtSecurity.GenerateNewTokens(refreshToken);
         return newTokens;

        }
    }
}