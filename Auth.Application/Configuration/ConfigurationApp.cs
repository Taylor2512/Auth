using Auth.Application.Dto;
using Auth.Application.Interface;
using Auth.Application.Services;

using Microsoft.Extensions.DependencyInjection;

using System.Reflection;

namespace Auth.Application.Configuration
{
    public static class ConfigurationApp
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
          services.AddScoped<IUserService, UserService>().AddScoped<JwtSecurity>();

            return services;
        }
        public static IServiceCollection RegisterMappings(this IServiceCollection services)
        {
            _ = services.AddAutoMapper(Assembly.GetExecutingAssembly());

            return services;
        }
    }
}
