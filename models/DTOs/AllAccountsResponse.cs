using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace payment_service.models.DTOs
{
	public class AllAccountsResponse
	{
		[Key]
		public Guid AccountId { get; set; }
		[Required]
		public Guid UserId { get; set; }
		[Required]
		public decimal Balance { get; set; }
		[Required]
		public string Status { get; set; }
		[Required]
		public string Currency { get; set; } = string.Empty;
		[Required]
		public DateTime CreatedAt { get; set; }
		[Required]
		public DateTime UpdatedAt { get; set; }
		public  ICollection<TransactionResponse> Transactions { get; set; }
	}

	public class TransactionResponse
	{
		public Guid TransactionId { get; set; }
		public decimal Amount { get; set; }
		public DateTime Timestamp { get; set; }
		public string TransactionType { get; set; }
		public string TransactionStatus { get; set; }
		public decimal BalanceAfterTransaction { get; set; }
		public string? PaymentMethod { get; set; }
		public Dictionary<string, object> MetaData { get; set; }

	}
}
