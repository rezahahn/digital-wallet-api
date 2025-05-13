using DigitalWallet.Application.DTOs.User;
using DigitalWallet.Application.Interfaces;
using DigitalWallet.Core.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace DigitalWallet.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class UsersController : BaseApiController
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] CreateUserDto createUserDto)
    {
        try
        {
            var user = await _userService.CreateUserAsync(createUserDto);
            return ApiResponse(user, "User registered successfully", StatusCodes.Status201Created);
        }
        catch (EmailAlreadyExistsException ex)
        {
            return ApiError(ex.Message, StatusCodes.Status400BadRequest);
        }
        catch (PhoneNumberAlreadyExistsException ex)
        {
            return ApiError(ex.Message, StatusCodes.Status400BadRequest);
        }
        catch (Exception ex)
        {
            return ApiError("Registration failed", StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [HttpPost("verify")]
    public async Task<IActionResult> VerifyKyc([FromBody] VerifyKycDto verifyKycDto)
    {
        try
        {
            var result = await _userService.VerifyKycAsync(verifyKycDto);
            return ApiResponse(result, "KYC verification successful");
        }
        catch (UserNotFoundException ex)
        {
            return ApiError(ex.Message, StatusCodes.Status404NotFound);
        }
        catch (Exception ex)
        {
            return ApiError("KYC verification failed", StatusCodes.Status500InternalServerError);
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(int id)
    {
        try
        {
            var user = await _userService.GetUserByIdAsync(id);
            return ApiResponse(user, "User data retrieved");
        }
        catch (UserNotFoundException ex)
        {
            return ApiError(ex.Message, StatusCodes.Status404NotFound);
        }
    }
}