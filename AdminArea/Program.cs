using Data;
using Dorak.Models;
using Dorak.Models.Models.Wallet;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Repositories;
using Services;

namespace AdminArea
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the DI container.
            builder.Services.AddDbContext<DorakContext>(options => options.UseLazyLoadingProxies()
           .UseSqlServer(builder.Configuration.GetConnectionString("DorakDB")));
            builder.Services.AddIdentity<User, IdentityRole>().AddEntityFrameworkStores<DorakContext>();
            builder.Services.AddControllersWithViews();
            builder.Services.AddScoped(typeof(ProviderAssignmentRepository));
            builder.Services.AddScoped(typeof(ProviderRepository));
            builder.Services.AddScoped(typeof(ServicesRepository));
            builder.Services.AddScoped(typeof(CenterRepository));
            builder.Services.AddScoped(typeof(ProviderCenterServiceRepository));

            builder.Services.AddScoped(typeof(CenterServices));
            builder.Services.AddScoped(typeof(ProviderServices));
            builder.Services.AddScoped(typeof(S_Services));
            builder.Services.AddScoped(typeof(ShiftRepository));
            builder.Services.AddDbContext<DorakContext>(options => options.UseLazyLoadingProxies()
                       .UseSqlServer(builder.Configuration.GetConnectionString("DorakDB")));
            builder.Services.AddScoped(typeof(RoleRepository));
            builder.Services.AddScoped(typeof(CommitData));



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
