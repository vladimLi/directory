using Microsoft.AspNetCore.Mvc;
using Shared;

namespace DirectoryService.Web.Extensions;

public static class ResponseExtensions
{
    public static ActionResult ToResponse(this Errors errors)
    {
        if (!errors.Any())
        {
            return new ObjectResult(errors)
            {
                StatusCode = StatusCodes.Status500InternalServerError,
            };
        }
        var distinctErrorTypes = errors
            .Select(e => e.Type)
            .Distinct()
            .ToList();
        
        int statusCode  = distinctErrorTypes.Count > 1
            ? StatusCodes.Status500InternalServerError
            : GetStatusCodeFromErrorType(distinctErrorTypes.FirstOrDefault());

        return new ObjectResult(errors)
        {
            StatusCode = statusCode
        };
    }

    public static int GetStatusCodeFromErrorType(ErrorType errorType)
    {
        return errorType switch
        {
            ErrorType.VALIDATION => StatusCodes.Status400BadRequest,
            ErrorType.NOT_FOUND => StatusCodes.Status404NotFound,
            ErrorType.CONFLICT => StatusCodes.Status409Conflict,
            ErrorType.FAILURE => StatusCodes.Status500InternalServerError,
            _ => StatusCodes.Status500InternalServerError
        };
    }
}