//using Microsoft.AspNetCore.Mvc;

//namespace DigitalWallet.API.Controllers
//{
//    [ApiVersion("1.0")]
//    public class LoansController : BaseApiController
//    {
//        private readonly ILoanService _loanService;

//        public LoansController(ILoanService loanService)
//        {
//            _loanService = loanService;
//        }

//        [HttpPost("apply")]
//        public async Task<IActionResult> ApplyLoan([FromBody] ApplyLoanRequest request)
//        {
//            try
//            {
//                var result = await _loanService.ApplyLoanAsync(request);
//                return ApiResponse(result, "Loan application submitted");
//            }
//            catch (Exception ex)
//            {
//                return ApiError(ex.Message);
//            }
//        }

//        [HttpGet("user/{userId}")]
//        public async Task<IActionResult> GetUserLoans(int userId)
//        {
//            try
//            {
//                var loans = await _loanService.GetUserLoansAsync(userId);
//                return ApiResponse(loans, "User loans retrieved");
//            }
//            catch (Exception ex)
//            {
//                return ApiError(ex.Message);
//            }
//        }

//        [HttpPost("repay")]
//        public async Task<IActionResult> RepayLoan([FromBody] RepayLoanRequest request)
//        {
//            try
//            {
//                var result = await _loanService.RepayLoanAsync(request);
//                return ApiResponse(result, "Loan repayment successful");
//            }
//            catch (Exception ex)
//            {
//                return ApiError(ex.Message);
//            }
//        }
//    }
//}
