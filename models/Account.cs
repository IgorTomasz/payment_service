using System.ComponentModel.DataAnnotations;

namespace payment_service.models
{
	public class Account
	{
		[Key]
		public Guid AccountId { get; set; }
		[Required]
		public Guid UserId { get; set; }
		[Required]
		public decimal Balance { get; set; }
		[Required]
		public AccountStatus Status { get; set; }
		[Required]
		public string Currency {  get; set; } = string.Empty;
		[Required]
		public DateTime CreatedAt { get; set; }
		[Required]
		public DateTime UpdatedAt { get; set; }
		public virtual ICollection<Transaction> Transactions { get; set; }
	}

	public enum AccountStatus
	{
		Active, Suspended, Closed
	}
}
