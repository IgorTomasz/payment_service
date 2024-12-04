using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using payment_service.models;
using payment_service.models.DTOs;
using payment_service.services;

namespace payment_service.Controllers
{
	[ApiController]
	[Route("payment/[controller]")]
	public class PaymentController : ControllerBase
	{
		private readonly IPaymentService _paymentService;
		
		public PaymentController(IPaymentService paymentService)
		{
			_paymentService = paymentService;
		}

		[HttpGet("account/all")]
		public async Task<IActionResult> GetAllAccounts()
		{
			return Ok(await _paymentService.GetAllAccounts());
		}

		[HttpGet("transactions/all")]
		public async Task<IActionResult> GetAllTransactions()
		{
			return Ok(await _paymentService.GetAllTransactions());
		}


		[HttpGet("wallet/balance")]
		public async Task<IActionResult> GetBalance(Guid userId)
		{
			decimal balance = await _paymentService.GetBalance(userId);

			if (balance == -1)
			{
				return Conflict("There is no account for that user guid");
			}

			return Ok(balance);
		}

		[HttpGet("wallet/transactions")]
		public async Task<IActionResult> GetUserTransactions(Guid userId)
		{
			return Ok(await _paymentService.GetUserTransactions(userId));
		}

		[HttpPost("account/create")]
		public async Task<IActionResult> CreateAccount(Guid userId)
		{


			bool res = await _paymentService.CreateNewAccount(userId);

			if (res)
			{
				return Created();
			}

			return Conflict("Something went wrong while creating an account");
		}

		[HttpPost("payments/handle-payment")]
		public async Task<IActionResult> Payment(PaymentRequest paymentRequest)
		{
			Guid isAccountExists = await _paymentService.IsAccountExists(paymentRequest.UserId);

			if (isAccountExists == Guid.Empty)
			{
				return BadRequest("There is no account for that user");
			}

			var transactionTypeString = paymentRequest.MetaData["TransactionType"]?.ToString();
			Enum.TryParse<TransactionType>(transactionTypeString, out var transactionType);

			if(transactionType == TransactionType.Withdraw || transactionType == TransactionType.GameBet)
			{
				bool availableFunds = await _paymentService.CheckAvailableFunds(isAccountExists, paymentRequest.Amount);

				if (!availableFunds)
				{
					return BadRequest("No sufficient funds on the account");
				}
			}


			dynamic transaction = await _paymentService.CreateNewTransaction(paymentRequest, isAccountExists);

			if (!transaction.created)
			{
				return Conflict("Something went wrong while creating transaction");
			}

			bool updated = await _paymentService.UpdateAccountAfterTransaction(isAccountExists, transaction.amount, transaction.transaction);
			
			if (!updated)
			{
				return Conflict("Something went wrong while updating account balance");
			}

			bool updateTransaction = await _paymentService.AfterTransactionComplete(transaction.transaction);

			if (!updateTransaction)
			{
				return Conflict("Failed to complete the transaction");
			}

			return Created();
		}

		
	}
}
