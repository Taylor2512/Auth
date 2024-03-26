using Auth.Application.Dto;
using Auth.Application.Dto.Request;
using Auth.Core.Entity;
using Auth.Shared.Extensions.Mapping;

using AutoMapper;

namespace Auth.Application.Mapper
{
    public class UserMapper : Profile
    {
        public UserMapper()
        {
            _ = CreateMap<UserRequest, UserDto>().IgnoreIfEmpty();
            _ = CreateMap<Users, UserRequest>().IgnoreIfEmpty();
            _ = CreateMap<Users, UserDto>().IgnoreIfEmpty();
        }
    }
}
