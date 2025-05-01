//using Microsoft.AspNetCore.Mvc;

//namespace DigitalWallet.API.Controllers
//{
//    [ApiVersion("1.0")]
//    public class InvestmentsController : BaseApiController
//    {
//        private readonly IInvestmentService _investmentService;

//        public InvestmentsController(IInvestmentService investmentService)
//        {
//            _investmentService = investmentService;
//        }

//        [HttpGet("products")]
//        public async Task<IActionResult> GetInvestmentProducts()
//        {
//            try
//            {
//                var products = await _investmentService.GetInvestmentProductsAsync();
//                return ApiResponse(products, "Investment products retrieved");
//            }
//            catch (Exception ex)
//            {
//                return ApiError(ex.Message);
//            }
//        }

//        [HttpPost("subscribe")]
//        public async Task<IActionResult> SubscribeInvestment([FromBody] SubscribeInvestmentRequest request)
//        {
//            try
//            {
//                var result = await _investmentService.SubscribeInvestmentAsync(request);
//                return ApiResponse(result, "Investment subscription successful");
//            }
//            catch (Exception ex)
//            {
//                return ApiError(ex.Message);
//            }
//        }

//        [HttpGet("user/{userId}")]
//        public async Task<IActionResult> GetUserInvestments(int userId)
//        {
//            try
//            {
//                var investments = await _investmentService.GetUserInvestmentsAsync(userId);
//                return ApiResponse(investments, "User investments retrieved");
//            }
//            catch (Exception ex)
//            {
//                return ApiError(ex.Message);
//            }
//        }
//    }
//}
