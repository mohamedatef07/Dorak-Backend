using Data;
using Dorak.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Repositories;

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
            builder.Services.AddScoped(typeof(CenterRepository));
            builder.Services.AddScoped(typeof(ProviderRepository));
            builder.Services.AddScoped(typeof(ProviderAssignmentRepository));
            builder.Services.AddScoped(typeof(RoleRepository));



            var app = builder.Build();

            // Configure the HTTP request pipeline.
            //if (!app.Environment.IsDevelopment())
            //{
            //    app.UseExceptionHandler("/Home/Error");
            //}
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
