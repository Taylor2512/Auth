using Microsoft.AspNetCore.Authentication.BearerToken;

namespace Auth.Application.Dto
{
    public class UserDto
    {
        public string? Email { get; set; }
        public string? AccessToken { get;  set; }
        public string? RefreshToken { get;  set; }
        public AccessTokenResponse? Token { get;  set; }
    }
}
