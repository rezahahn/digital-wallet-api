using DigitalWallet.Core.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

public class BaseApiController : ControllerBase
{
    protected IActionResult ApiResponse<T>(T data, string message = "", int statusCode = 200)
    {
        var response = new
        {
            Success = statusCode >= 200 && statusCode < 300,
            Message = message,
            Data = data
        };

        return StatusCode(statusCode, response);
    }

    protected IActionResult ApiError(string errorMessage, int statusCode = 400, object? additionalData = null)
    {
        var response = new
        {
            Success = false,
            Message = errorMessage,
            Errors = additionalData
        };

        return StatusCode(statusCode, response);
    }

    protected IActionResult HandleException(Exception ex)
    {
        return ex switch
        {
            ValidationException valEx => ApiError("Validation failed", StatusCodes.Status400BadRequest,
                valEx.Errors.Select(e => e.ErrorMessage).ToList()),
            DatabaseOperationException => ApiError("Database operation failed", StatusCodes.Status500InternalServerError),
            UserNotFoundException => ApiError(ex.Message, StatusCodes.Status404NotFound),
            _ => ApiError("An unexpected error occurred", StatusCodes.Status500InternalServerError)
        };
    }
}