using System.Text;
using Data;
using Dorak.Models;
using Dorak.ViewModels;
using Dorak.ViewModels.AccountViewModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Repositories;
using Services;

namespace API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();

            //DI Containers
            builder.Services.AddDbContext<DorakContext>
                (i=>i.UseLazyLoadingProxies().UseSqlServer(builder.Configuration.GetConnectionString("DorakDB")));
            builder.Services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<DorakContext>();
            builder.Services.AddScoped(typeof(AccountRepository));
            builder.Services.AddScoped(typeof(ProviderRepository));
            builder.Services.AddScoped(typeof(ClientRepository));
            builder.Services.AddScoped(typeof(OperatorRepository));
            builder.Services.AddScoped(typeof(AdminCenterRepository));
            builder.Services.AddScoped(typeof(AdminCenterManagement));
            builder.Services.AddScoped(typeof(RoleRepository));
            builder.Services.AddScoped(typeof(AccountServices));
            builder.Services.AddScoped(typeof(ClientServices));
            builder.Services.AddScoped(typeof(OperatorServices));
            builder.Services.AddScoped(typeof(ProviderServices));
            builder.Services.AddScoped(typeof(AdminCenterServices));
            
            
            //Authentication
            builder.Services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(option =>
            {
                option.SaveToken = true;
                option.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["JWT:PrivateKey"]))
                };
            });


            builder.Services.AddCors(option=>option.AddDefaultPolicy(
                i=>i.AllowAnyMethod().AllowAnyHeader().AllowAnyOrigin()
                ));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.UseStaticFiles();
            app.UseRouting();
            app.UseCors();
            app.UseAuthorization();
            app.UseAuthentication();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=index}");

            app.Run();
        }
    }
}
