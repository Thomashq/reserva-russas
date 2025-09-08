using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RR.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RR.Infraestructure.Configuration
{
    public class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
    {
        public void Configure(EntityTypeBuilder<AppUser> builder)
        {
            // Tabela de usuários do Identity
            builder.ToTable("rr_users");

            // PK
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                   .HasColumnName("id")
                   .IsRequired()
                   .ValueGeneratedOnAdd(); 

            // Identity "core"
            builder.Property(x => x.UserName)
                   .HasColumnName("user_name")
                   .HasMaxLength(256);

            builder.Property(x => x.NormalizedUserName)
                   .HasColumnName("normalized_user_name")
                   .HasMaxLength(256);

            builder.Property(x => x.Email)
                   .HasColumnName("email")
                   .HasMaxLength(256);

            builder.Property(x => x.NormalizedEmail)
                   .HasColumnName("normalized_email")
                   .HasMaxLength(256);

            builder.Property(x => x.EmailConfirmed)
                   .HasColumnName("email_confirmed")
                   .IsRequired()
                   .HasDefaultValue(false);

            // Hash/Selos/Concorrência
            builder.Property(x => x.PasswordHash)
                   .HasColumnName("password_hash")
                   .HasColumnType("text"); // evita limitar; suporta hashes longos

            builder.Property(x => x.SecurityStamp)
                   .HasColumnName("security_stamp")
                   .HasMaxLength(100);

            builder.Property(x => x.ConcurrencyStamp)
                   .HasColumnName("concurrency_stamp")
                   .HasMaxLength(100);

            builder.Property(x => x.PhoneNumber)
                   .HasColumnName("phone_number")
                   .HasMaxLength(32);

            builder.Property(x => x.PhoneNumberConfirmed)
                   .HasColumnName("phone_number_confirmed")
                   .IsRequired()
                   .HasDefaultValue(false);

            builder.Property(x => x.TwoFactorEnabled)
                   .HasColumnName("two_factor_enabled")
                   .IsRequired()
                   .HasDefaultValue(false);

            builder.Property(x => x.LockoutEnd)
                   .HasColumnName("lockout_end"); 

            builder.Property(x => x.LockoutEnabled)
                   .HasColumnName("lockout_enabled")
                   .IsRequired()
                   .HasDefaultValue(false);

            builder.Property(x => x.AccessFailedCount)
                   .HasColumnName("access_failed_count")
                   .IsRequired()
                   .HasDefaultValue(0);

            builder.Property(x => x.CreatedAt).HasColumnName("created_at");
            builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");
            builder.Property(x => x.IsActive).HasColumnName("is_active");

            builder.Property(x => x.FullName)
                   .HasColumnName("full_name")
                   .HasMaxLength(200);

            builder.HasIndex(x => x.NormalizedUserName)
                   .IsUnique()
                   .HasDatabaseName("ux_rr_users_normalized_user_name");

            builder.HasIndex(x => x.NormalizedEmail)
                   .HasDatabaseName("ix_rr_users_normalized_email");

            // Obs.: índices para created_at/is_active já são criados no ConfigureBaseEntityProperties
        }
    }
}
