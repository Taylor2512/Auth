using Auth.Application.Dto;
using Auth.Application.Dto.Request;
using Auth.Application.Interface;
using Auth.Application.Security;
using Auth.Core.Entity;
using Auth.Infrastructure.Persistence;

using AutoMapper;

using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Auth.Application.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly JwtSecurity _jwtSecurity;
        private readonly PasswordHandler _passwordHandler; // Inyectar PasswordHandler

        public UserService(
            ApplicationDbContext context,
            IConfiguration configuration,
            IMapper mapper,
            JwtSecurity jwtSecurity,
            PasswordHandler passwordHandler) // Agregar PasswordHandler al constructor
        {
            _context = context;
            _configuration = configuration;
            _mapper = mapper;
            _jwtSecurity = jwtSecurity;
            _passwordHandler = passwordHandler; // Asignar PasswordHandler
        }

        public async Task<UserDto> Login(UserRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.IsActive && x.Email == request.Email);
            if (user == null)
            {
                throw new InvalidDataException("Usuario no encontrado");
            }
            else
            {
                if (_passwordHandler.ValidatePassword(request.Password, user.Password, user.Salt))
                {
                    string accessToken = _jwtSecurity.GenerateAccessToken(user.Id);
                    string refreshToken = _jwtSecurity.GenerateRefreshToken(user.Id);

                    UserDto userDto = _mapper.Map<UserDto>(user);
                    AccessTokenResponse tokenResponse = new()
                    {
                        AccessToken = accessToken,
                        RefreshToken = refreshToken,
                        ExpiresIn = Convert.ToInt32(_configuration["jwt:AccessTokenExpiryTime"])
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

            byte[] salt = _passwordHandler.GenerateSalt();
            string hashedPassword = _passwordHandler.HashPassword(request.Password, salt);

            Users newUser = _mapper.Map<Users>(request);
            newUser.Salt = salt;
            newUser.Password = hashedPassword;

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();
        }

        public async Task<AccessTokenResponse> GenerateRefreshToken(string refreshToken)
        {
            var newTokens = _jwtSecurity.GenerateNewTokens(refreshToken);
            return await Task.FromResult(newTokens);
        }
    }
}