using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RR.Core.Entities;

namespace RR.Infrastructure.Configurations
{
    public class AccountConfiguration : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            // Table configuration
            builder.ToTable("account");

            // Primary key
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                .HasColumnName("id")
                .IsRequired()
                .ValueGeneratedOnAdd();

            // UserName configuration
            builder.Property(x => x.UserName)
                .HasColumnName("user_name")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.UserId)
                .HasColumnName("user_id")
                .HasMaxLength(300)
                .IsRequired(); // <- tornar obrigatório para 1:1

            // Mail configuration
            builder.Property(x => x.Mail)
                .HasColumnName("mail")
                .HasMaxLength(255)
                .IsRequired();

            // Phone configuration (opcional)
            builder.Property(x => x.Phone)
                .HasColumnName("phone")
                .HasMaxLength(20)
                .IsRequired(false);

            // AccountPermission configuration
            builder.Property(x => x.AccountPermission)
                .HasColumnName("account_permission")
                .IsRequired()
                .HasDefaultValue(0);

            // Indexes
            builder.HasIndex(x => x.UserName)
                .IsUnique()
                .HasDatabaseName("ix_account_user_name");

            builder.HasIndex(x => x.Mail)
                .IsUnique()
                .HasDatabaseName("ix_account_mail");

            builder.HasIndex(x => x.Phone)
                .HasDatabaseName("ix_account_phone")
                .HasFilter("phone IS NOT NULL"); // Índice parcial apenas para valores não nulos

            builder.HasIndex(x => x.UserId)
                .IsUnique()
                .HasDatabaseName("ix_account_user_id_unique");

            // Constraints personalizadas podem ser adicionadas via migrations se necessário
            // Para validar o tamanho mínimo da senha, é melhor fazer isso na camada de domínio/aplicação

            builder.HasOne(a => a.AppUser)
                .WithOne(u => u.Account)
                .HasForeignKey<Account>(a => a.UserId)       // FK está em Account.user_id
                .HasPrincipalKey<AppUser>(u => u.Id)         // PK principal é AppUser.Id (padrão do Identity)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(a => a.Manager)
                   .WithOne(m => m.Account)
                   .HasForeignKey<Manager>(m => m.AccountId)
                   .OnDelete(DeleteBehavior.Cascade);

            // 1:1 com Servant (opcional)
            builder.HasOne(a => a.Servant)
                   .WithOne(s => s.Account)
                   .HasForeignKey<Servant>(s => s.AccountId)
                   .OnDelete(DeleteBehavior.Cascade);

            // 1:1 com Student (opcional)
            builder.HasOne(a => a.Student)
                   .WithOne(s => s.Account)
                   .HasForeignKey<Student>(s => s.AccountId)
                   .OnDelete(DeleteBehavior.Cascade);

            // 1:N com Reservations
            builder.HasMany(a => a.Reservations)
                   .WithOne(r => r.Account)
                   .HasForeignKey(r => r.AccountId)
                   .OnDelete(DeleteBehavior.Restrict); // Não deletar account se tiver reservas
        }
    }
}