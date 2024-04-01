using Auth.Application.Dto;
using Auth.Application.Interface;
using Auth.Application.Security;
using Auth.Application.Services;
using Auth.Shared.Services;

using Microsoft.Extensions.DependencyInjection;

using System.Reflection;

namespace Auth.Application.Configuration
{
    public static class ConfigurationApp
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
          services.AddTransient<IUserService, UserService>().AddTransient<IEmailSender, EmailSender>()
                .AddTransient<JwtSecurity>()
                .AddTransient< PasswordHandler>();
            services.AddTransient<IRabbitMQManager, RabbitMQManager>();

            return services;
        }
        public static IServiceCollection RegisterMappings(this IServiceCollection services)
        {
            _ = services.AddAutoMapper(Assembly.GetExecutingAssembly());

            return services;
        }
    }
}
