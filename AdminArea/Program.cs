using Data;
using Dorak.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Repositories;
using Services;
using System.Diagnostics;

namespace AdminArea
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<DorakContext>(options =>
            options.UseLazyLoadingProxies()
           .UseSqlServer(builder.Configuration.GetConnectionString("DorakDB")).LogTo(log => Debug.WriteLine($"====MVC=====\n{log}"), LogLevel.Information).EnableSensitiveDataLogging());
            // Dependency Injections
            builder.Services.AddScoped(typeof(CenterRepository));
            builder.Services.AddScoped(typeof(AccountRepository));
            builder.Services.AddScoped(typeof(AdminCenterManagement));
            builder.Services.AddScoped(typeof(ProviderRepository));
            builder.Services.AddScoped(typeof(ClientRepository));
            builder.Services.AddScoped(typeof(OperatorRepository));
            builder.Services.AddScoped(typeof(AdminCenterRepository));
            builder.Services.AddScoped(typeof(ProviderAssignmentRepository));
            builder.Services.AddScoped(typeof(RoleRepository));
            builder.Services.AddScoped(typeof(ProviderCenterService));
            builder.Services.AddScoped(typeof(ShiftRepository));
            builder.Services.AddScoped(typeof(ShiftServices));
            builder.Services.AddScoped(typeof(LiveQueueRepository));
            builder.Services.AddScoped(typeof(LiveQueueServices));
            builder.Services.AddScoped(typeof(ServicesRepository));
            builder.Services.AddScoped(typeof(AccountServices));
            builder.Services.AddScoped(typeof(ClientServices));
            builder.Services.AddScoped(typeof(OperatorServices));
            builder.Services.AddScoped(typeof(ProviderServices));
            builder.Services.AddScoped(typeof(ProviderCenterServiceRepository));
            builder.Services.AddScoped(typeof(AppointmentRepository));
            builder.Services.AddScoped(typeof(WalletRepository));
            builder.Services.AddScoped(typeof(S_Services));
            builder.Services.AddScoped(typeof(AdminCenterServices));
            builder.Services.AddScoped(typeof(CommitData));
            builder.Services.AddScoped(typeof(CenterServices));
            builder.Services.AddScoped<UserManager<User>>();
            builder.Services.AddScoped<SignInManager<User>>();
            builder.Services.AddScoped(typeof(ProviderServices));
            builder.Services.AddScoped(typeof(ShiftServices));
            builder.Services.AddScoped(typeof(AppointmentRepository));
            builder.Services.AddScoped(typeof(AppointmentServices));
            builder.Services.AddScoped(typeof(TemperoryClientRepository));
            builder.Services.AddScoped(typeof(PaymentRepository));
            builder.Services.AddScoped(typeof(PaymentServices));
            builder.Services.AddScoped(typeof(ReviewRepository));
            builder.Services.AddScoped(typeof(ReviewServices));
            builder.Services.AddScoped(typeof(NotificationServices)); // Register NotificationServices as a singleton
            builder.Services.AddScoped(typeof(NotificationRepository));
            builder.Services.AddSingleton(typeof(NotificationSignalRService));

            // Dependency Injections
            builder.Services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<DorakContext>();

            builder.Services.AddSignalR();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
