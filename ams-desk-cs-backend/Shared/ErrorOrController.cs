using ErrorOr;
using Microsoft.AspNetCore.Mvc;

namespace ams_desk_cs_backend.Shared;

public class ErrorOrController : ControllerBase
{
    public IActionResult ErrorOrToResponse<T>(ErrorOr<T> errorOr)
    {
        if (errorOr.IsError)
        {
            return errorOr.FirstError.Type switch
            {
                ErrorType.NotFound => NotFound(errorOr.FirstError.Description),
                ErrorType.Failure => StatusCode(500, errorOr.FirstError.Description),
                ErrorType.Unexpected => StatusCode(500, errorOr.FirstError.Description),
                ErrorType.Validation => BadRequest(errorOr.FirstError.Description),
                ErrorType.Conflict => Conflict(errorOr.FirstError.Description),
                ErrorType.Unauthorized => Unauthorized(errorOr.FirstError.Description),
                ErrorType.Forbidden => Forbid(errorOr.FirstError.Description),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        
        if (errorOr.Value == null) return Ok();
        
        return Ok(errorOr.Value);
    }
}