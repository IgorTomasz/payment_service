using System.ComponentModel.DataAnnotations;

namespace payment_service.models.DTOs
{
	public class PaymentRequest
	{
		[Required]
		public Guid UserId { get; set; }
		[Required]
		public decimal Amount { get; set; }
		public Dictionary<string, object> MetaData { get; set; }
	}
}
