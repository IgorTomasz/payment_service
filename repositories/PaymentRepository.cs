using Microsoft.EntityFrameworkCore;
using payment_service.context;
using payment_service.models;
using payment_service.models.DTOs;

namespace payment_service.repositories
{
	public interface IPaymentRepository
	{
		public Task<bool> CreateNewAccount(Guid userId);
		public Task<object> CreateNewTransaction(PaymentRequest depositRequest, Guid accountId);
		public Task<decimal> GetUserBalance(Guid userId);
		public Task<Guid> IsAccountExists(Guid userId);
		public Task<bool> UpdateAccountAfterTransaction(Guid accountId, decimal amount, Transaction transaction);
		public Task<List<AllAccountsResponse>> GetAccounts();
		public Task<List<AllTransactionsResponse>> GetTransactions();
		public Task<bool> AfterTransactionComplete(Transaction transaction);
		public Task<bool> SufficientFundsAvailable(Guid accountId, decimal bet);
		public Task<List<Transaction>> GetUserTransactions(Guid userId);
	}
	public class PaymentRepository : IPaymentRepository
	{
		private readonly PaymentDatabaseContext _context;

		public PaymentRepository(PaymentDatabaseContext context)
		{
			_context = context;
		}

		public async Task<decimal> GetUserBalance(Guid userId)
		{
			Account? account = await _context.Accounts.Where(e=> e.UserId==userId).FirstOrDefaultAsync();

			return account == null ? -1 : account.Balance;
		}

		public async Task<bool> CreateNewAccount(Guid userId)
		{
			Guid accountId = Guid.NewGuid();
			Account account = new Account
			{
				UserId = userId,
				AccountId = accountId,
				Balance = 0,
				Status = 0,
				CreatedAt = DateTime.UtcNow.AddHours(1),
				UpdatedAt = DateTime.UtcNow.AddHours(1),
				Currency = "PLN"
			};

			var res = await _context.Accounts.AddAsync(account);

			await _context.SaveChangesAsync();


			return res != null;

		}

		public async Task<Guid> IsAccountExists(Guid userId)
		{
			var acc = await _context.Accounts.Where(e=>e.UserId==userId).FirstOrDefaultAsync();

			return acc != null ? acc.AccountId : Guid.Empty;
		}

		public async Task<object> CreateNewTransaction(PaymentRequest depositRequest, Guid accountId)
		{
			Account account = await _context.Accounts.FindAsync(accountId);	
			Guid transactionId = Guid.NewGuid();
			var paymentMethodString = depositRequest.MetaData["PaymentMethod"]?.ToString();
			var transactionTypeString = depositRequest.MetaData["TransactionType"]?.ToString();

			Enum.TryParse<PaymentMethod>(paymentMethodString, out var paymentMethod);
			Enum.TryParse<TransactionType>(transactionTypeString, out var transactionType);

			decimal amount = depositRequest.Amount;
			switch (transactionType)
			{
				case TransactionType.Deposit: amount = amount < 0 ? amount * -1.0m : amount; break;
				case TransactionType.Withdraw: amount = amount > 0 ? amount * -1.0m : amount; break;
				case TransactionType.GameBet: amount = amount > 0 ? amount * -1.0m : amount; break;
				case TransactionType.GameWin: amount = amount < 0 ? amount * -1.0m : amount; break;
			};

			decimal balanceAfterTransaction = account.Balance + amount;	

			Transaction transaction = new Transaction
			{
				TransactionId = transactionId,
				AccountId = accountId,
				Amount = depositRequest.Amount,
				Timestamp = DateTime.UtcNow.AddHours(1),
				TransactionType = transactionType,
				TransactionStatus = TransactionStatus.Processing,
				BalanceAfterTransaction = balanceAfterTransaction,
				PaymentMethod = paymentMethod,
				MetaData = depositRequest.MetaData
			};

			var res = await _context.Transactions.AddAsync(transaction);

			await _context.SaveChangesAsync();

			return new
			{
				created = res!=null,
				amount = depositRequest.Amount,
				transaction = transaction
			};
		}

		public async Task<bool> AfterTransactionComplete(Transaction transaction)
		{
			Transaction trans = await _context.Transactions.FindAsync(transaction.TransactionId);

			trans.TransactionStatus = TransactionStatus.Completed;

			var res = await _context.SaveChangesAsync();

			return res > 0;
		}

		public async Task<bool> UpdateAccountAfterTransaction(Guid accountId, decimal amountNormal, Transaction transaction)
		{
			Account account = await _context.Accounts.FindAsync(accountId);

			decimal amount = amountNormal;
			switch (transaction.TransactionType)
			{
				case TransactionType.Deposit: amount = amount < 0 ? amount * -1.0m : amount; break;
				case TransactionType.Withdraw: amount = amount > 0 ? amount * -1.0m : amount; break;
				case TransactionType.GameBet: amount = amount > 0 ? amount * -1.0m : amount; break;
				case TransactionType.GameWin: amount = amount < 0 ? amount * -1.0m : amount; break;
			};

			account.Balance += amount;
			account.UpdatedAt = DateTime.UtcNow.AddHours(1);

			var res = await _context.SaveChangesAsync();

			return res > 0;
		}

		public async Task<bool> SufficientFundsAvailable(Guid accountId, decimal bet)
		{
			Account acc = await _context.Accounts.FindAsync(accountId);

			return acc.Balance >= bet;
		}

		public async Task<List<AllAccountsResponse>> GetAccounts()
		{
			var result = await _context.Accounts.Include(e => e.Transactions).ToListAsync();
			return result.Select(e=> new AllAccountsResponse
			{
				AccountId = e.AccountId,
				UserId = e.UserId,
				Balance = e.Balance,
				Status = Enum.GetName<AccountStatus>(e.Status),
				Currency = e.Currency,
				CreatedAt = e.CreatedAt,
				UpdatedAt = e.UpdatedAt,
				Transactions = e.Transactions.Select(t=> new TransactionResponse
				{
					TransactionId = t.TransactionId,
					Amount = t.Amount,
					Timestamp = t.Timestamp,
					TransactionType = Enum.GetName<TransactionType>(t.TransactionType),
					TransactionStatus = Enum.GetName<TransactionStatus>(t.TransactionStatus),
					BalanceAfterTransaction = t.BalanceAfterTransaction,
					PaymentMethod = Enum.GetName(typeof(PaymentMethod), t.PaymentMethod),
					MetaData = t.MetaData
				}).ToList()
			}).ToList();
		}

		public async Task<List<AllTransactionsResponse>> GetTransactions()
		{
			var result = await _context.Transactions.Include(e => e.Account).ToListAsync();
			return result.Select(e=> new AllTransactionsResponse
			{
				TransactionId = e.TransactionId,
				Amount = e.Amount,
				Timestamp = e.Timestamp,
				TransactionType = Enum.GetName<TransactionType>(e.TransactionType),
				TransactionStatus = Enum.GetName<TransactionStatus>(e.TransactionStatus),
				BalanceAfterTransaction = e.BalanceAfterTransaction,
				PaymentMethod = Enum.GetName(typeof(PaymentMethod), e.PaymentMethod),
				MetaData = e.MetaData,
				Account = new AccountResponse
				{
					AccountId = e.Account.AccountId,
					UserId = e.Account.UserId,
					Balance = e.Account.Balance,
					Status = Enum.GetName<AccountStatus>(e.Account.Status),
					Currency = e.Account.Currency,
					CreatedAt = e.Account.CreatedAt,
					UpdatedAt = e.Account.UpdatedAt
				}
			}).ToList();
		}

		public async Task<List<Transaction>> GetUserTransactions(Guid userId)
		{
			return await _context.Transactions.Where(e=> e.Account.UserId == userId).OrderBy(e=>e.Timestamp).ToListAsync();
		}

	}
}
