using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Models.Configurations;
using Models.Models;

namespace Models
{
    public class DorakContext : IdentityDbContext
    {
        //Tables
        public virtual DbSet<Center> Centers { get; set; }
        public virtual DbSet<AdminCenterManagement> AdminCentersManagement { get; set; }
        public virtual DbSet<CenterService> CenterServices { get; set; }
        public virtual DbSet<Service> Services { get; set; }
        public virtual DbSet<Appointment> Appointments { get; set; }
        public virtual DbSet<Shift> Shifts { get; set; }
        public virtual DbSet<LiveQueue> LiveQueues { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }
        public virtual DbSet<Wallet> Wallets { get; set; }
        public virtual DbSet<ProviderServices> ProviderServices { get; set }
        public virtual DbSet<ProviderAssignment> ProviderAssignments { get; set }

        //Connect With database
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data source = .; Initial catalog = Dorak; Integrated security= true; trustservercertificate = true; Encrypt= false;");
        }
        //Apply Configurations
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new CenterConfiguration { });
            builder.ApplyConfiguration(new CenterServiceConfiguration { });
            builder.ApplyConfiguration(new ServiceConfiguration { });
            builder.ApplyConfiguration(new AdminCenterManagementConfiguration { });
            builder.ApplyConfiguration(new LiveQueueConfiguration { });
            builder.ApplyConfiguration(new AppointmentConfiguration { });
            builder.ApplyConfiguration(new ShiftConfiguration { });
            builder.ApplyConfiguration(new PaymentConfiguration { });
            builder.ApplyConfiguration(new NotificationConfiguration { });
            builder.ApplyConfiguration(new WalletConfiguration { });
            builder.ApplyConfiguration(new ProviderAssignmentConfiguration { });
            builder.ApplyConfiguration(new ProviderServicesConfiguration { });

        }

    }
}
