using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RR.Core.Entities;

namespace RR.Infraestructure.Configuration
{
    public class StudentAdvisorConfiguration
    {
        public void Configure(EntityTypeBuilder<StudentAdvisor> builder)
        {
            builder.ToTable("student_advisor");

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

            builder.Property(x => x.ServantId)
                .HasColumnName("servant_id")
                .IsRequired();

            // Índice único composto para evitar duplicatas
            builder.HasIndex(x => new { x.StudentId, x.ServantId })
                .IsUnique()
                .HasDatabaseName("ix_student_advisor_student_servant_unique");

            // Índices individuais para performance
            builder.HasIndex(x => x.StudentId)
                .HasDatabaseName("ix_student_advisor_student_id");

            builder.HasIndex(x => x.ServantId)
                .HasDatabaseName("ix_student_advisor_servant_id");

            // Relacionamentos
            builder.HasOne(sa => sa.Student)
                   .WithMany(s => s.Advisors)
                   .HasForeignKey(sa => sa.StudentId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(sa => sa.Servant)
                   .WithMany(s => s.AdvisedStudents)
                   .HasForeignKey(sa => sa.ServantId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
