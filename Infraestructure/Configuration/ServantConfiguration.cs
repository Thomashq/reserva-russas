using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RR.Core.Entities;

namespace RR.Infraestructure.Configuration
{
    public class ServantConfiguration
    {
        public void Configure(EntityTypeBuilder<Servant> builder)
        {
            // Table configuration
            builder.ToTable("servant");

            // Primary key
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                .HasColumnName("id")
                .IsRequired()
                .ValueGeneratedOnAdd();

            // AccountId configuration
            builder.Property(x => x.AccountId)
                .HasColumnName("account_id")
                .IsRequired();

            // Ignore collections - serão mapeadas via relacionamentos
            // N:N com Student via StudentAdvisor
            builder.HasMany(s => s.AdvisedStudents)
                   .WithOne(sa => sa.Servant)
                   .HasForeignKey(sa => sa.ServantId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Índices
            builder.HasIndex(x => x.AccountId)
                .IsUnique()
                .HasDatabaseName("ix_servant_account_id");

            // Indexes
            builder.HasIndex(x => x.AccountId)
                .IsUnique()
                .HasDatabaseName("ix_servant_account_id");
        }
    }
}
