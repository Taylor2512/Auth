using Auth.Api.Model;

using Microsoft.AspNetCore.Mvc;

namespace Auth.Api.Extensions
{
    public class ControllerBase: Microsoft.AspNetCore.Mvc.ControllerBase
    {
        [NonAction]
        public virtual ObjectResult Ok(object value)
        {
            var response = new ApiResponse<object>();
            response.Success = true;
            response.StatusCode = StatusCodes.Status200OK;
            response.Data = value;
            return base.Ok(response);
        }
        [NonAction]
        public virtual ObjectResult Ok()
        {
            var response = new ApiResponse();
            response.Success = true;
            response.StatusCode = StatusCodes.Status200OK;
            return base.Ok(response);
        }
        [NonAction]
        public virtual ObjectResult BadRequest(object value)
        {
            var response = new ApiResponse<object>();
            response.Success = false;
            response.StatusCode = StatusCodes.Status400BadRequest;
            response.Data = value;
            return base.BadRequest(response);
        }
        
    }
}
