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
    public class ReservationConfiguration
    {
        public void Configure(EntityTypeBuilder<Reservation> builder)
        {
            // Table configuration
            builder.ToTable("reservation");

            // Primary key
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                .HasColumnName("id")
                .IsRequired()
                .ValueGeneratedOnAdd();

            // RoomId configuration
            builder.Property(x => x.RoomId)
                .HasColumnName("room_id")
                .IsRequired();

            // AccountId configuration
            builder.Property(x => x.AccountId)
                .HasColumnName("account_id")
                .IsRequired();

            // StartTime configuration
            builder.Property(x => x.StartTime)
                .HasColumnName("start_time")
                .HasColumnType("timestamp with time zone")
                .IsRequired();

            // EndTime configuration
            builder.Property(x => x.EndTime)
                .HasColumnName("end_time")
                .HasColumnType("timestamp with time zone")
                .IsRequired();

            // Indexes for better query performance
            builder.HasIndex(x => x.RoomId)
                .HasDatabaseName("ix_reservation_room_id");

            builder.HasIndex(x => x.AccountId)
                .HasDatabaseName("ix_reservation_account_id");

            builder.HasIndex(x => x.StartTime)
                .HasDatabaseName("ix_reservation_start_time");

            builder.HasIndex(x => x.EndTime)
                .HasDatabaseName("ix_reservation_end_time");

            // Composite index for room and time range queries
            builder.HasIndex(x => new { x.RoomId, x.StartTime, x.EndTime })
                .HasDatabaseName("ix_reservation_room_time_range");

            // Check constraint to ensure EndTime > StartTime
            builder.ToTable(t => t.HasCheckConstraint("ck_reservation_end_after_start", "end_time > start_time"));

            builder.HasIndex(x => x.RoomId)
                .HasDatabaseName("ix_reservation_room_id");

            builder.HasIndex(x => x.AccountId)
                .HasDatabaseName("ix_reservation_account_id");

            builder.HasIndex(x => new { x.RoomId, x.StartTime, x.EndTime })
                .HasDatabaseName("ix_reservation_room_time_range");

            // Constraint para garantir que EndTime > StartTime
            builder.ToTable(t => t.HasCheckConstraint(
                "ck_reservation_end_after_start",
                "end_time > start_time"));

        }
    }
}
