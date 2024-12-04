namespace payment_service.middleware
{
	public class IpFilteringMiddleware
	{
		private readonly RequestDelegate _requestDelegate;
		private readonly HashSet<string> _allowedIps;

		public IpFilteringMiddleware(RequestDelegate requestDelegate, IConfiguration configuration)
		{
			_requestDelegate = requestDelegate;
			_allowedIps = new HashSet<string>(configuration.GetSection("Filtering:AllowedIps").Get<string[]>() ?? Array.Empty<string>());
		}

		public async Task Invoke(HttpContext context)
		{
			string ip = context.Connection.RemoteIpAddress.ToString();

			if (_allowedIps.Contains(ip))
			{
				await _requestDelegate(context);
			}

			context.Response.StatusCode = 403;
			await context.Response.WriteAsync($"Forbidden from {ip}");
			return;
		}
	}
}
