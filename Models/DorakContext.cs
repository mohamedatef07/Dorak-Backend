using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Models.Configrations;
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
        }

    }
}
