using Auth.Application.Dto;
using Auth.Application.Dto.Request;
using Auth.Application.Interface;

using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Mvc;




namespace Auth.Api.Controller
{
    [Route("[controller]")]
    [ApiController]
    public class UserController(IUserService userService) : ControllerBase
    {
        [HttpPost("RegistryUser")]
        public async Task<IActionResult> RegistryUser([FromBody] UserRequest request)
        {
            
                await userService.RegistryUser(request);
                return Ok("Usuario registrado con éxito");
          
        }

        [HttpPost("Login")]
        [ResponseCache(Duration = 10, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> Login([FromBody] UserRequest request)
        {
            
                UserDto user = await userService.Login(request);
                return Ok(user);
           
        }
        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
           
                AccessTokenResponse token = await userService.GenerateRefreshToken(request.RefreshToken);
                return Ok(token);
           
        }

    }
}
