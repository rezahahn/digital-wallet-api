using DigitalWallet.Application.DTOs.Wallet.Request;
using DigitalWallet.Application.Interfaces;
using DigitalWallet.Core.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace DigitalWallet.API.Controllers;

[ApiVersion("1.0")]
public class WalletsController : BaseApiController
{
    private readonly IWalletService _walletService;

    public WalletsController(IWalletService walletService)
    {
        _walletService = walletService;
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetWallet(int userId)
    {
        try
        {
            var wallet = await _walletService.GetWalletByUserIdAsync(userId);
            return ApiResponse(wallet, "Wallet data retrieved");
        }
        catch (UserNotFoundException ex)
        {
            return ApiError(ex.Message, StatusCodes.Status404NotFound);
        }
        catch (WalletNotFoundException ex)
        {
            return ApiError(ex.Message, StatusCodes.Status404NotFound);
        }
    }

    [HttpPost("topup")]
    public async Task<IActionResult> TopUp([FromBody] TopUpRequest request)
    {
        try
        {
            var result = await _walletService.TopUpAsync(request);
            return ApiResponse(result, "Top up successful");
        }
        catch (UserNotFoundException ex)
        {
            return ApiError(ex.Message, StatusCodes.Status404NotFound);
        }
        catch (WalletOperationException ex)
        {
            return ApiError(ex.Message, StatusCodes.Status400BadRequest);
        }
    }

    [HttpPost("transfer")]
    public async Task<IActionResult> Transfer([FromBody] TransferRequest request)
    {
        try
        {
            var result = await _walletService.TransferAsync(request);
            return ApiResponse(result, "Transfer successful");
        }
        catch (InsufficientBalanceException ex)
        {
            return ApiError(ex.Message, StatusCodes.Status400BadRequest);
        }
        catch (UserNotFoundException ex)
        {
            return ApiError(ex.Message, StatusCodes.Status404NotFound);
        }
        catch (InvalidTransactionException ex)
        {
            return ApiError(ex.Message, StatusCodes.Status400BadRequest);
        }
    }

    [HttpGet("transactions/{walletId}")]
    [ResponseCache(Duration = 30)]
    public async Task<IActionResult> GetWalletTransactions(
    [FromRoute] int walletId,
    [FromQuery] DateTime? fromDate,
    [FromQuery] DateTime? toDate,
    [FromQuery] string? transactionType,
    [FromQuery] string? status,
    [FromQuery] string sortBy = "TransactionDate",
    [FromQuery] bool descending = true,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10)
    {
        try
        {
            // Validasi parameter
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;

            var response = await _walletService.GetWalletTransactionsAsync(
                walletId, fromDate, toDate, transactionType, status,
                sortBy, descending, page, pageSize);

            return ApiResponse(response, "Transactions retrieved successfully");
        }
        catch (WalletNotFoundException ex)
        {
            return ApiError(ex.Message, StatusCodes.Status404NotFound);
        }
        catch (Exception ex)
        {
            return ApiError("Internal server error : " + ex.Message, StatusCodes.Status500InternalServerError);
        }
    }
}