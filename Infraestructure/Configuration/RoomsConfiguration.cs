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
    public class RoomsConfiguration
    {
        public void Configure(EntityTypeBuilder<Rooms> builder)
        {
            // Table configuration
            builder.ToTable("rooms");

            // Primary key
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                .HasColumnName("id")
                .IsRequired()
                .ValueGeneratedOnAdd();

            // Name configuration
            builder.Property(x => x.Name)
                .HasColumnName("name")
                .HasMaxLength(200)
                .IsRequired();

            // Capacity configuration
            builder.Property(x => x.Capacity)
                .HasColumnName("capacity")
                .IsRequired()
                .HasDefaultValue(1);

            // ManagerId configuration
            builder.Property(x => x.ManagerId)
                .HasColumnName("manager_id")
                .IsRequired();

            // Indexes
            builder.HasIndex(x => x.Name)
                .IsUnique()
                .HasDatabaseName("ix_rooms_name");

            builder.HasIndex(x => x.ManagerId)
                .HasDatabaseName("ix_rooms_manager_id");

            builder.HasIndex(x => x.Capacity)
                .HasDatabaseName("ix_rooms_capacity");

            // 1:N com Reservations - Uma sala pode ter várias reservas
            builder.HasMany(r => r.Reservations)
                   .WithOne(res => res.Room)
                   .HasForeignKey(res => res.RoomId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Índices
            builder.HasIndex(x => x.Name)
                .IsUnique()
                .HasDatabaseName("ix_rooms_name");

            builder.HasIndex(x => x.ManagerId)
                .HasDatabaseName("ix_rooms_manager_id");

            // Check constraint for positive capacity
            builder.ToTable(t => t.HasCheckConstraint("ck_rooms_positive_capacity", "capacity > 0"));

        }
    }
}
