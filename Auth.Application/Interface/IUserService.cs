using Auth.Application.Dto;
using Auth.Application.Dto.Request;

namespace Auth.Application.Interface
{
    public interface IUserService
    {
        Task<UserDto> Login(UserRequest request);
        Task RegistryUser(UserRequest request);
    }
}
