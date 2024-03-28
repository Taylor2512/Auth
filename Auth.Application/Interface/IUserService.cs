using Auth.Application.Dto;
using Auth.Application.Dto.Request;

using Microsoft.AspNetCore.Authentication.BearerToken;

namespace Auth.Application.Interface
{
    public interface IUserService
    {
        Task<UserDto> Login(UserRequest request);
        Task RegistryUser(UserRequest request);
        Task<AccessTokenResponse> GenerateRefreshToken(string refreshToken);
    }
}
