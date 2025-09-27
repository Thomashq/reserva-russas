using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RR.Core.Entities;

namespace RR.Infraestructure.Configuration
{
    public class StudentPermissionConfiguration
    {
        public void Configure(EntityTypeBuilder<StudentPermission> builder)
        {
            builder.ToTable("student_permission");

            // Chave primária única
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            // Propriedades da BaseEntity
            builder.Property(x => x.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();

            builder.Property(x => x.UpdatedAt)
                .HasColumnName("updated_at")
                .IsRequired();

            builder.Property(x => x.IsActive)
                .HasColumnName("is_active")
                .HasDefaultValue(true);

            // Propriedades de relacionamento
            builder.Property(x => x.StudentId)
                .HasColumnName("student_id")
                .IsRequired();

            builder.Property(x => x.PermissionId)
                .HasColumnName("permission_id")
                .IsRequired();

            // Índice único composto para evitar duplicatas
            builder.HasIndex(x => new { x.StudentId, x.PermissionId })
                .IsUnique()
                .HasDatabaseName("ix_student_permission_student_permission_unique");

            // Índices individuais para performance
            builder.HasIndex(x => x.StudentId)
                .HasDatabaseName("ix_student_permission_student_id");

            builder.HasIndex(x => x.PermissionId)
                .HasDatabaseName("ix_student_permission_permission_id");

            // Relacionamentos
            builder.HasOne(sp => sp.Student)
                   .WithMany() // ou .WithMany(s => s.Permissions) se existir
                   .HasForeignKey(sp => sp.StudentId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Adicione o relacionamento com Permission se existir a entidade
            // builder.HasOne(sp => sp.Permission)
            //        .WithMany()
            //        .HasForeignKey(sp => sp.PermissionId)
            //        .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
