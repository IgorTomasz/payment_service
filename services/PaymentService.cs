using payment_service.models;
using payment_service.models.DTOs;
using payment_service.repositories;

namespace payment_service.services
{
	public interface IPaymentService
	{
		public Task<bool> CreateNewAccount(Guid userId);
		public Task<object> CreateNewTransaction(PaymentRequest depositRequest, Guid accountId);
		public Task<decimal> GetBalance(Guid userId);
		public Task<Guid> IsAccountExists(Guid userId);
		public Task<bool> UpdateAccountAfterTransaction(Guid accountId, decimal amount, TransactionResponse transaction);
		public Task<List<AllAccountsResponse>> GetAllAccounts();
		public Task<List<AllTransactionsResponse>> GetAllTransactions();
		public Task<bool> AfterTransactionComplete(TransactionResponse transaction);
		public Task<bool> CheckAvailableFunds(Guid accountId, decimal bet);
		public Task<List<Transaction>> GetUserTransactions(Guid userId);
	}

	public class PaymentService : IPaymentService
	{
		private readonly IPaymentRepository _paymentRepository;

		public PaymentService(IPaymentRepository paymentRepository)
		{
			_paymentRepository = paymentRepository;
		}

		public async Task<decimal> GetBalance(Guid userId)
		{
			return await _paymentRepository.GetUserBalance(userId);
		}

		public async Task<bool> CreateNewAccount(Guid userId)
		{
			return await _paymentRepository.CreateNewAccount(userId);
		}

		public async Task<Guid> IsAccountExists(Guid userId)
		{
			return await _paymentRepository.IsAccountExists(userId);
		}

		public async Task<object> CreateNewTransaction(PaymentRequest depositRequest, Guid accountId)
		{
			return await _paymentRepository.CreateNewTransaction(depositRequest, accountId);
		}

		public async Task<bool> UpdateAccountAfterTransaction(Guid accountId, decimal amount, TransactionResponse transaction)
		{
			return await _paymentRepository.UpdateAccountAfterTransaction(accountId, amount, transaction);
		}

		public async Task<List<AllAccountsResponse>> GetAllAccounts()
		{
			return await _paymentRepository.GetAccounts();
		}

		public async Task<List<AllTransactionsResponse>> GetAllTransactions()
		{
			return await _paymentRepository.GetTransactions();
		}

		public async Task<bool> AfterTransactionComplete(TransactionResponse transaction)
		{
			return await _paymentRepository.AfterTransactionComplete(transaction);
		}

		public async Task<bool> CheckAvailableFunds(Guid accountId, decimal bet)
		{
			return await _paymentRepository.SufficientFundsAvailable(accountId, bet);
		}

		public async Task<List<Transaction>> GetUserTransactions(Guid userId)
		{
			return await _paymentRepository.GetUserTransactions(userId);
		}
	}
}
