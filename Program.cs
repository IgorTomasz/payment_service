
using Microsoft.EntityFrameworkCore;
using payment_service.context;
using payment_service.middleware;
using payment_service.repositories;
using payment_service.services;

namespace payment_service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddDbContext<PaymentDatabaseContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("Default")));
            builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
            builder.Services.AddScoped<IPaymentService, PaymentService>();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

			/**
             * Authorization with gateway api by ip filtering
             */
			app.UseMiddleware<IpFilteringMiddleware>();

			/**
             * Authorization with gateway api by secret key
             */
			app.UseMiddleware<GatewayAuthenticationMiddleware>();

			app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
