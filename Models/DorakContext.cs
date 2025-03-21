using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Models.Models;

namespace Models
{
    public class DorakContext : IdentityDbContext
    {
        //Tables
        public virtual DbSet<Center> Centers { get; set; }
        public virtual DbSet<AdminCenterManagement> AdminCenterManagement { get; set; }
        public virtual DbSet<CenterService> CenterServices { get; set; }
        public virtual DbSet<Service> Services { get; set; }
        public virtual DbSet<ProviderServices> ProviderServices {get ; set }
        public virtual DbSet<ProviderAssignment> ProviderAssignments {get ; set }
}


protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
    optionsBuilder.UseSqlServer("Data source = .; Initial catalog = Dorak; Integrated security= true; trustservercertificate = true; Encrypt= false;");
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new ProviderAssignmentConfiguration { });
            builder.ApplyConfiguration(new ProviderServicesConfiguration { });



        }

    }
}
