namespace payment_service.models
{
	public class HttpResponseModel
	{
		public bool Success { get; set; }
		public string? Error { get; set; }
		public object? Message { get; set; }
	}
}
