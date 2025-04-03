using Data;
using Dorak.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Repositories;
using Services;
using System.Text.Json.Serialization;

namespace API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container
            builder.Services.AddControllersWithViews();
            builder.Services.AddScoped(typeof(CenterRepository));
            builder.Services.AddScoped(typeof(ProviderRepository));
            builder.Services.AddScoped(typeof(ProviderServices));
            builder.Services.AddScoped(typeof(ProviderAssignmentRepository));
            builder.Services.AddScoped(typeof(ProviderScheduleRepository));
            builder.Services.AddScoped(typeof(CommitData));

            builder.Services.AddIdentity<User, IdentityRole>().AddEntityFrameworkStores<DorakContext>();
            builder.Services.AddDbContext<DorakContext>(options =>
                options.UseLazyLoadingProxies()
                       .UseSqlServer(builder.Configuration.GetConnectionString("DorakDB")));
            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
            });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            
            var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DorakContext>();
            CommitData.Initialize(context);  

            // Configure the HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
