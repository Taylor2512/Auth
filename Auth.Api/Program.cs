using Auth.Api.Filters;
using Auth.Api.Midleware;
using Auth.Application.Configuration;
using Auth.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Auth.Api.Model;
using System.Text.Json;
using System.Text.Json.Serialization;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer()
                .AddControllers(option => { option.Filters.Add<ApiExceptionFilter>(); })
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.DictionaryKeyPolicy = null;
                    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
                    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                    options.AllowInputFormatterExceptionMessages = true;
                    options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString;
                    options.JsonSerializerOptions.AllowTrailingCommas = true;
                    options.JsonSerializerOptions.ReadCommentHandling = JsonCommentHandling.Skip;
                    options.JsonSerializerOptions.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
                    options.JsonSerializerOptions.IncludeFields = true;

                })
                ;

builder.Services.RegisterServices().RegisterMappings();
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = JsonSerializer.Serialize(context.ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList());
        var response = new ApiResponse();
        var errorResponse = new ErrorResponse();
        errorResponse.Message = errors;
        response.StatusCode = StatusCodes.Status422UnprocessableEntity;
        response.Error = errorResponse;
        return new ObjectResult(response)
        {
            StatusCode = StatusCodes.Status422UnprocessableEntity
        };
    };
});
builder.Services.AddSwaggerGen().RegisterServices().RegisterMappings();
builder.Services
    .AddDbContext<ApplicationDbContext>(
        b => b.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), option =>
        {
            _ = option.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
        }));
var secret = builder.Configuration["Jwt:Key"];
var key = Encoding.ASCII.GetBytes(secret ?? throw new ArgumentException(" no puede ser null"));
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
app.UseCustomErrorHandling(); // Agrega el middleware personalizado de manejo de errores aqu?
app.MapControllers();
app.Run();
