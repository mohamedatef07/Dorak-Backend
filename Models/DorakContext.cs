using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Models.Configrations;
using Models.Configurations;
using Models.Models;

namespace Models
{
    public class DorakContext : IdentityDbContext<User>
    {
        //Tables
        public virtual DbSet<Center> Centers { get; set; }
        public virtual DbSet<AdminCenterManagement> AdminCentersManagement { get; set; }
        public virtual DbSet<CenterService> CenterServices { get; set; }
        public virtual DbSet<Service> Services { get; set; }
        public virtual DbSet<Client> Clients { get; set; }
        public virtual DbSet<Provider> Providers { get; set; }
        public virtual DbSet<ProviderCertifications> ProviderCertifications { get; set; }
        public virtual DbSet<TemporaryClient> TemporaryClients { get; set; }

        //Connect With database
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies().UseSqlServer("Data source = .; Initial catalog = Dorak; Integrated security= true; trustservercertificate = true; Encrypt= false;");
        }
        //Apply Configurations
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new CenterConfiguration { });
            builder.ApplyConfiguration(new CenterServiceConfiguration { });
            builder.ApplyConfiguration(new ServiceConfiguration { });
            builder.ApplyConfiguration(new AdminCenterManagementConfiguration { });
            builder.ApplyConfiguration(new ClientConfiguration { });
            builder.ApplyConfiguration(new ProviderConfiguration { });
            builder.ApplyConfiguration(new ProviderCertificationsConfiguration { });
            builder.ApplyConfiguration(new TemporaryClientConfiguration { });


            base.OnModelCreating(builder);
        }

    }
}
