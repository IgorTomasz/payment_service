using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace payment_service.models.DTOs
{
	public class AllTransactionsResponse
	{
		public Guid TransactionId { get; set; }
		public decimal Amount { get; set; }
		public DateTime Timestamp { get; set; }
		public string TransactionType { get; set; }
		public string TransactionStatus { get; set; }
		public decimal BalanceAfterTransaction { get; set; }
		public string? PaymentMethod { get; set; }
		public Dictionary<string, object> MetaData { get; set; }
		public AccountResponse Account { get; set; }
	}

	public class AccountResponse
	{
		public Guid AccountId { get; set; }
		public Guid UserId { get; set; }
		public decimal Balance { get; set; }
		public string Status { get; set; }
		public string Currency { get; set; } = string.Empty;
		public DateTime CreatedAt { get; set; }
		public DateTime UpdatedAt { get; set; }
	}
}
