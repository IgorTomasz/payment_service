using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace payment_service.models
{
	public class Transaction
	{
		[Key]
		public Guid TransactionId { get; set; }
		[Required]
		public Guid AccountId { get; set; }
		[Required]
		public decimal Amount { get; set; }
		[Required]
		public DateTime Timestamp { get; set; }
		[Required]
		public TransactionType TransactionType { get; set; }
		[Required]
		public TransactionStatus TransactionStatus { get; set; }
		public PaymentMethod? PaymentMethod { get; set; }
		public Dictionary<string, object> MetaData { get; set; }
		[ForeignKey(nameof(AccountId))]
		public virtual Account Account { get; set; }
	}

	public enum TransactionType
	{
		Deposit, Withdraw, GameBet, GameWin
	}

	public enum TransactionStatus
	{
		Pending, Completed, Failed, Processing, Canceled
	}

	public enum PaymentMethod
	{
		Card, Paypal, Blik
	}
}
