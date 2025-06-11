using Microsoft.AspNetCore.Mvc;

namespace Freelance_Project.Controllers;

public class BaseApiController : ControllerBase
{
    public IActionResult Success(object data, string message = "Success")
    {
        return Ok(new
        {
            success = true,
            message,
            data,
            errors = (object)null
        });
    }
}