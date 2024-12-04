using Microsoft.EntityFrameworkCore;
using payment_service.models;
using System.Text.Json;

namespace payment_service.context
{
	public class PaymentDatabaseContext : DbContext
	{
		public PaymentDatabaseContext(DbContextOptions dbContextOptions) : base(dbContextOptions) { }

		public DbSet<Account> Accounts { get; set; }
		public DbSet<Transaction> Transactions { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<Transaction>().Property(e => e.MetaData).HasColumnType("nvarchar(max)").HasConversion(
				c => JsonSerializer.Serialize(c, (JsonSerializerOptions)null),
				c => JsonSerializer.Deserialize<Dictionary<string, object>>(c, (JsonSerializerOptions)null));
		}
	}
}
