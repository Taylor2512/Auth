using Auth.Api.Filters;
using Auth.Api.Midleware;
using Auth.Application.Configuration;
using Auth.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);


builder.Services.AddEndpointsApiExplorer().AddControllers()
//    .ConfigureApiBehaviorOptions(options =>
//{
//    options.InvalidModelStateResponseFactory = context => context.GenerateBadRequest();
    
//})

    ;
builder.Services.AddSwaggerGen().RegisterServices().RegisterMappings();
builder.Services
    .AddDbContext<ApplicationDbContext>(
        b => b.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), option =>
        {
            _ = option.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
        }));
var secret = builder.Configuration["Jwt:Key"];
var key = Encoding.ASCII.GetBytes(secret);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };
        });



WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    _ = app.UseSwagger();
    _ = app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.UseCustomErrorHandling(); // Agrega el middleware personalizado de manejo de errores aquí
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});
app.Run();
