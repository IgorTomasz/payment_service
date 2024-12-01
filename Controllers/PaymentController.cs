using Microsoft.AspNetCore.Mvc;
using payment_service.services;

namespace payment_service.Controllers
{
	[ApiController]
	[Route("payment/[controller]")]
	public class PaymentController
	{
		private readonly IPaymentService _paymentService;
		
		public PaymentController(IPaymentService paymentService)
		{
			_paymentService = paymentService;
		}

		[HttpPost("payments/deposit")]
		public async Task<IActionResult> Deposit()
		{

		}
		[HttpPost("payments/withdraw")]
		public async Task<IActionResult> Withdraw()
		{

		}

		[HttpGet("wallet/balance/{userIdString}")]
		public async Task<IActionResult> GetBalance()
		{

		}
	}
}
