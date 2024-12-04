namespace payment_service.middleware
{
	public class GatewayAuthenticationMiddleware
	{
		private readonly RequestDelegate _requestDelegate;
		private readonly string _secret;

		public GatewayAuthenticationMiddleware(RequestDelegate requestDelegate, IConfiguration configuration)
		{
			_requestDelegate = requestDelegate;
			_secret = configuration["ApiKey:Secret"];
		}

		public async Task Invoke(HttpContext context)
		{
			var header = context.Request.Headers.TryGetValue("X-Int-Secret", out var secret);
			if (!secret.Equals(_secret) || string.IsNullOrEmpty(secret) || !header)
			{
				context.Response.StatusCode = 403;
				await context.Response.WriteAsync($"Forbidden");
				return;
			}

			await _requestDelegate(context);
		}
	}
}
