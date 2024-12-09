using System.Net;

namespace payment_service.middleware
{
	public class IpFilteringMiddleware
	{
		private readonly RequestDelegate _requestDelegate;
		private readonly HashSet<string> _allowed;

		public IpFilteringMiddleware(RequestDelegate requestDelegate, IConfiguration configuration)
		{
			_requestDelegate = requestDelegate;
			_allowed = new HashSet<string>(configuration.GetSection("Filtering:AllowedIps").Get<string[]>() ?? Array.Empty<string>());
		}

		public async Task Invoke(HttpContext context)
		{
			string ip = context.Connection.RemoteIpAddress.ToString();

			List<IPHostEntry> allowedNames = new List<IPHostEntry>();

			foreach (var item in _allowed)
			{
				allowedNames.Add(Dns.GetHostByName(item));
			}

			List<IPAddress> allowedAddresses = allowedNames.Select(x => x.AddressList.FirstOrDefault()).ToList();

			if (allowedAddresses.Contains(IPAddress.Parse(ip.Split(":").Last())))
			{
				await _requestDelegate(context);
				return;
			}


			context.Response.StatusCode = 403;
			await context.Response.WriteAsync($"Forbidden from {ip}");
			return;
		}
	}
}
