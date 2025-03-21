using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models.Models;

namespace Models.Configrations
{
    public class ClientConfiguration : IEntityTypeConfiguration<Client>
    {
        public void Configure(EntityTypeBuilder<Client> builder)
        {
            //Primary Key
            builder.HasKey(client => client.ClientId);

            //Relations 
        
        
            //Properties
            builder.Property(c => c.FirstName)
                .HasMaxLength(50)
                .HasColumnName("First Name")
                .HasColumnType("NVARCHAR")
                .IsRequired(true);
            
            
            builder.Property(c => c.LastName)
                .HasMaxLength(50)
                .HasColumnName("Last Name")
                .HasColumnType("NVARCHAR")
                .IsRequired(true);

            builder.Property(c => c.Gender)
                .HasMaxLength(10)
                .HasColumnName("Gender")
                .HasColumnType("NVARCHAR")
                .IsRequired(true);
            
            
            builder.Property(c => c.BirthDate)   
                .HasColumnType("DATE") 
                .IsRequired();




        }
        

        
    }
}
