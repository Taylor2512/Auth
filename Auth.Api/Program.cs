using System.Globalization;
using System.Text;
using Auth.Api.Filters;
using Auth.Api.Midleware;
using Auth.Infrastructure.Persistence;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Configura la cultura invariante
CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
CultureInfo.CurrentUICulture = CultureInfo.InvariantCulture;

// Configuración del servicio
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers(option => { option.Filters.Add<ApiExceptionFilter>(); });

// Configuración JSON
builder.Services.AddControllers().AddJsonOptions(options =>
{
    // Configuración JSON personalizada aquí
});

// Configuración del contexto de la base de datos
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), option =>
    {
        option.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
    });
});

// Configuración de la autenticación JWT
var secret = builder.Configuration["Jwt:Key"];
var key = Encoding.ASCII.GetBytes(secret ?? throw new ArgumentException("La clave no puede ser null"));
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

// Configuración de Swagger
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Middleware y rutas
app.UseSwagger();
app.UseSwaggerUI();
app.UseExceptionHandler("/Home/Error");
app.UseHsts();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.UseCustomErrorHandling(); // Agrega el middleware personalizado de manejo de errores aquí
app.MapControllers();
app.Run();
