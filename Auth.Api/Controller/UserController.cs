using Auth.Application.Dto;
using Auth.Application.Dto.Request;
using Auth.Application.Interface;

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
            try
            {
                await userService.RegistryUser(request);
                return Ok("Usuario registrado con éxito");

            }
            catch (InvalidDataException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("Login")]
        [ResponseCache(Duration = 10, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> Login([FromBody] UserRequest request)
        {
            try
            {
                UserDto user = await userService.Login(request);
                return Ok(user);
            }
            catch (InvalidDataException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

    }
}
