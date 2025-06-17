using Data;
using Dorak.Models;
using Hangfire;
using Hangfire.SqlServer;
using Hubs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Repositories;
using Services;
using Stripe;
using System.Text;
using System.Text.Json.Serialization;
using Serilog;
using System.Diagnostics;

namespace API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container
            builder.Services.AddControllersWithViews();


            builder.Services.AddDbContext<DorakContext>(options =>
                options.UseLazyLoadingProxies()
                       .UseSqlServer(builder.Configuration.GetConnectionString("DorakDB")).LogTo(log=> Debug.WriteLine($"=========\n{log}"),LogLevel.Information).EnableSensitiveDataLogging());
            // logging
            Serilog.Log.Logger = new LoggerConfiguration()
                .WriteTo.File(@"Logs\DorakLog.txt",
                        rollingInterval: RollingInterval.Day,
                        restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Warning,
                        retainedFileCountLimit: null,
                        fileSizeLimitBytes: null,
                        rollOnFileSizeLimit: true).CreateLogger();

            builder.Host.UseSerilog();

            // Dependency Injections
            builder.Services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<DorakContext>();


            var stripeSecretKey = builder.Configuration.GetSection("Stripe:SecretKey").Value;
            if (string.IsNullOrEmpty(stripeSecretKey))
            {
                throw new InvalidOperationException("Stripe SecretKey is not configured in appsettings.json.");
            }
            StripeConfiguration.ApiKey = stripeSecretKey;


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
            builder.Services.AddScoped<ShiftServices>();
            builder.Services.AddScoped(typeof(AppointmentRepository));
            builder.Services.AddScoped(typeof(AppointmentServices));
            builder.Services.AddScoped(typeof(TemperoryClientRepository));
            builder.Services.AddScoped(typeof(PaymentRepository));
            builder.Services.AddScoped(typeof(PaymentServices));
            builder.Services.AddScoped(typeof(ReviewRepository));
            builder.Services.AddScoped(typeof(ReviewServices));
            builder.Services.AddTransient<MailKitEmailSender>();
            builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
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

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAngularApp", builder =>
                {
                    builder.WithOrigins("http://localhost:4200")
                           .AllowAnyHeader()
                           .AllowAnyMethod()
                           .AllowCredentials();
                });
            });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "API.PortalApp", Version = "v1" });
                #region JWT Token
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
                      Enter 'Bearer' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'Bearer 12345abcdef'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
              {
                {
                  new OpenApiSecurityScheme
                  {
                    Reference = new OpenApiReference
                      {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                      },
                      Scheme = "oauth2",
                      Name = "Bearer",
                      In = ParameterLocation.Header,

                    },
                    new List<string>()
                  }
                });

                #endregion
            });

            builder.Services.AddSignalR()
                .AddJsonProtocol(options =>
                {
                    options.PayloadSerializerOptions.PropertyNamingPolicy = null;
                    options.PayloadSerializerOptions.PropertyNameCaseInsensitive = false;
                });

            //builder.Services.AddScoped<GlobalErrorHandlerMiddleware>();
            builder.Services.AddScoped<TransactionMiddleware>();

            var app = builder.Build();
            //app.UseMiddleware<GlobalErrorHandlerMiddleware>();
            app.UseMiddleware<TransactionMiddleware>();
            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                string[] roles = { "SuperAdmin", "Admin", "Operator", "Provider", "Client" };
                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        await roleManager.CreateAsync(new IdentityRole(role));
                    }
                }
            }


            // Configure the HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseStaticFiles();
            app.UseRouting();
            app.UseCors("AllowAngularApp");
            app.UseAuthentication();
            app.UseAuthorization();

            // ?? Hangfire Dashboard
            app.UseHangfireDashboard();

            // ?? Schedule Recurring Job
            using (var scope = app.Services.CreateScope())
            {
                var providerServices = scope.ServiceProvider.GetRequiredService<ProviderServices>();
                var recurringJobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();
                var paymentServices = scope.ServiceProvider.GetRequiredService<PaymentServices>();
                recurringJobManager.AddOrUpdate(
                    "RegenerateWeeklyAssignmentsJob",
                    () => providerServices.RegenerateWeeklyAssignments(),
                    "0 */6 * * *");
                recurringJobManager.AddOrUpdate(
                    "UpdatePendingPaymentsJob",
                    () => paymentServices.UpdatePendingPayments(),
                    "0 */10 * * *");
                var appointmentServices = scope.ServiceProvider.GetRequiredService<AppointmentServices>();


                recurringJobManager.AddOrUpdate(
                    "CancelUnpaidAppointmentsJob",
                    () => appointmentServices.CancelUnpaidAppointments(),
                    "0 0 * * *");  //daily 
            }

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=index}");


            RecurringJob.AddOrUpdate<ReviewServices>(
             "update-provider-ratings",
              service => service.UpdateAllProvidersAverageRating(),
               Cron.Monthly);
            app.MapHub<QueueHub>("/queueHub");
            app.MapHub<ShiftListHub>("/shiftListHub");
            app.MapControllers();
            app.Run();
        }
    }
}
