//using Data;
//using Dorak.Models;
//using Dorak.Models.Models.Wallet;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.IdentityModel.Tokens;
//using Repositories;
//using Services;
//using System.Text;
//using System.Text.Json.Serialization;
//using Hangfire;
//using Hangfire.SqlServer;

//namespace API
//{
//    public class Program
//    {
//        public static void Main(string[] args)
//        {
//            var builder = WebApplication.CreateBuilder(args);

//            // Add services to the container
//            builder.Services.AddControllersWithViews();
//            builder.Services.AddIdentity<User, IdentityRole>().AddEntityFrameworkStores<DorakContext>();
//            builder.Services.AddDbContext<DorakContext>(options =>
//                options.UseLazyLoadingProxies()
//                       .UseSqlServer(builder.Configuration.GetConnectionString("DorakDB")));

//            builder.Services.AddScoped(typeof(CenterRepository));
//            builder.Services.AddScoped(typeof(AccountRepository));
//            builder.Services.AddScoped(typeof(AdminCenterManagement));
//            builder.Services.AddScoped(typeof(ProviderRepository));
//            builder.Services.AddScoped(typeof(ClientRepository));
//            builder.Services.AddScoped(typeof(OperatorRepository));
//            builder.Services.AddScoped(typeof(AdminCenterRepository));
//            builder.Services.AddScoped(typeof(ProviderAssignmentRepository));
//            builder.Services.AddScoped(typeof(RoleRepository));
//            builder.Services.AddScoped(typeof(ProviderCenterService));
//            builder.Services.AddScoped(typeof(ShiftRepository));
//            builder.Services.AddScoped(typeof(ServicesRepository));
//            builder.Services.AddScoped(typeof(AccountServices));
//            builder.Services.AddScoped(typeof(ClientServices));
//            builder.Services.AddScoped(typeof(OperatorServices));
//            builder.Services.AddScoped(typeof(ProviderServices));
//            builder.Services.AddScoped(typeof(ProviderCenterServiceRepository));
//            builder.Services.AddScoped(typeof(AppointmentRepository));
//            builder.Services.AddScoped(typeof(S_Services));
//            builder.Services.AddScoped(typeof(AdminCenterServices));
//            builder.Services.AddScoped(typeof(CommitData));
//            builder.Services.AddScoped(typeof(CenterServices));
//            builder.Services.AddScoped(typeof(ProviderCardService));


//            builder.Services.AddControllers().AddJsonOptions(options =>
//            {
//                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
//            });

//            //Authentication
//            builder.Services.AddAuthentication(option =>
//            {
//                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//                option.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
//                option.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
//                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//            }).AddJwtBearer(option =>
//            {
//                option.SaveToken = true;
//                option.TokenValidationParameters = new TokenValidationParameters()
//                {
//                    ValidateAudience = false,
//                    ValidateIssuer = false,
//                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["JWT:PrivateKey"]))
//                };
//            });


//            builder.Services.AddCors(option => option.AddDefaultPolicy(
//                i => i.AllowAnyMethod().AllowAnyHeader().AllowAnyOrigin()
//                ));



//            builder.Services.AddEndpointsApiExplorer();
//            builder.Services.AddSwaggerGen();
//            var app = builder.Build();


//            // Configure the HTTP request pipeline
//            if (app.Environment.IsDevelopment())
//            {
//                app.UseSwagger();
//                app.UseSwaggerUI();
//            }

//            app.UseStaticFiles();
//            app.UseRouting();
//            app.UseCors();
//            app.UseAuthorization();
//            app.UseAuthentication();
//            app.MapControllerRoute(
//                name: "default",
//                pattern: "{controller=Home}/{action=index}");
//            app.MapControllers();
//            app.Run();
//        }
//    }
//}


using Data;
using Dorak.Models;
using Dorak.Models.Models.Wallet;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Repositories;
using Services;
using System.Text;
using System.Text.Json.Serialization;
using Hangfire;
using Hangfire.SqlServer;

namespace API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container
            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<DorakContext>(options =>
                options.UseLazyLoadingProxies()
                       .UseSqlServer(builder.Configuration.GetConnectionString("DorakDB")));

            builder.Services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<DorakContext>();

            // ?? Hangfire Configuration
            builder.Services.AddHangfire(config => config
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(builder.Configuration.GetConnectionString("DorakDB"), new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.FromSeconds(15),
                    UseRecommendedIsolationLevel = true,
                    DisableGlobalLocks = true
                }));

            builder.Services.AddHangfireServer();

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
            builder.Services.AddScoped(typeof(ServicesRepository));
            builder.Services.AddScoped(typeof(AccountServices));
            builder.Services.AddScoped(typeof(ClientServices));
            builder.Services.AddScoped(typeof(OperatorServices));
            builder.Services.AddScoped(typeof(ProviderServices));
            builder.Services.AddScoped(typeof(ProviderCenterServiceRepository));
            builder.Services.AddScoped(typeof(AppointmentRepository));
            builder.Services.AddScoped(typeof(S_Services));
            builder.Services.AddScoped(typeof(AdminCenterServices));
            builder.Services.AddScoped(typeof(CommitData));
            builder.Services.AddScoped(typeof(CenterServices));
            builder.Services.AddScoped(typeof(ProviderCardService));

            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
            });

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

            builder.Services.AddCors(option => option.AddDefaultPolicy(
                i => i.AllowAnyMethod().AllowAnyHeader().AllowAnyOrigin()
            ));

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseStaticFiles();
            app.UseRouting();
            app.UseCors();
            app.UseAuthentication();
            app.UseAuthorization();

            // ?? Hangfire Dashboard
            app.UseHangfireDashboard();

            // ?? Schedule Recurring Job
            using (var scope = app.Services.CreateScope())
            {
                var providerServices = scope.ServiceProvider.GetRequiredService<ProviderServices>();
                var recurringJobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();

                recurringJobManager.AddOrUpdate(
                    "RegenerateWeeklyAssignmentsJob",
                    () => providerServices.RegenerateWeeklyAssignments(),
                    Cron.Daily);
            }

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=index}");

            app.MapControllers();
            app.Run();
        }
    }
}
