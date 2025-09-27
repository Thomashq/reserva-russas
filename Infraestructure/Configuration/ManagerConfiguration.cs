using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RR.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RR.Infraestructure.Configuration
{
    public class ManagerConfiguration
    {
        public void Configure(EntityTypeBuilder<Manager> builder)
        {
            // Table configuration
            builder.ToTable("manager");

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

            // Ignore ManagedRooms - será mapeado via relacionamento em Rooms
            builder.Ignore(x => x.ManagedRooms);

            // Foreign key relationship with Account
            builder.HasIndex(x => x.AccountId)
                .IsUnique()
                .HasDatabaseName("ix_manager_account_id");

            // relacionamentos
            builder.HasMany(m => m.ManagedRooms)
                  .WithOne(r => r.Manager)
                  .HasForeignKey(r => r.ManagerId)
                  .OnDelete(DeleteBehavior.Restrict); // Protege contra deleção acidental

            // Índices
            builder.HasIndex(x => x.AccountId)
                .IsUnique()
                .HasDatabaseName("ix_manager_account_id");
        }
    }
}
