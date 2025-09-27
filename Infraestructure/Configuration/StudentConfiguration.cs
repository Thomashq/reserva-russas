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
    public class StudentConfiguration
    {
        public void Configure(EntityTypeBuilder<Student> builder)
        {
            // Table configuration
            builder.ToTable("student");

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

            // N:N com Servant via StudentAdvisor
            builder.HasMany(s => s.Advisors)
                   .WithOne(sa => sa.Student)
                   .HasForeignKey(sa => sa.StudentId)
                   .OnDelete(DeleteBehavior.Cascade);

            // 1:N com StudentPermission
            builder.HasMany(s => s.Permissions)
                   .WithOne(sp => sp.Student)
                   .HasForeignKey(sp => sp.StudentId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Índices
            builder.HasIndex(x => x.AccountId)
                .IsUnique()
                .HasDatabaseName("ix_student_account_id");
        }
    }
}
