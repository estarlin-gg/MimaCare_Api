using Microsoft.AspNetCore.Mvc;
using MimaCare.Application.Common;

namespace MimaCare.Presentation.Common
{
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected ActionResult Success(
            string message = "Success")
        {
            return Ok(new ApiResponse<object>
            {
                StatusCode = 200,
                Message = message,
                Data = null
            });
        }

        protected ActionResult Success<T>(
            T data,
            string message = "Success")
        {
            return Ok(new ApiResponse<T>
            {
                StatusCode = 200,
                Message = message,
                Data = data
            });
        }

        protected ActionResult CreatedResponse<T>(
            T data,
            string message = "Created")
        {
            return StatusCode(201,
                new ApiResponse<T>
                {
                    StatusCode = 201,
                    Message = message,
                    Data = data
                });
        }
    }
}