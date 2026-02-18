using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RR.Core.Entities;

namespace RR.Infraestructure.Configuration
{
    internal sealed class EquipmentReservationConfiguration : IEntityTypeConfiguration<EquipmentReservation>
    {
        public void Configure(EntityTypeBuilder<EquipmentReservation> builder)
        {
            builder.ToTable("EquipmentReservations");
            
            // Primary key
            builder.HasKey(er => er.Id);
            builder.Property(er => er.Id)
                   .ValueGeneratedOnAdd();
            
            // Properties
            builder.Property(er => er.EquipmentId)
                   .IsRequired();
            
            builder.Property(er => er.AccountId)
                   .IsRequired();
            
            builder.Property(er => er.RoomReservationId)
                   .IsRequired(false);
            
            builder.Property(er => er.Title)
                   .HasMaxLength(250)
                   .IsRequired();
            
            builder.Property(er => er.Description)
                   .HasMaxLength(1000)
                   .IsRequired(false);
            
            builder.Property(er => er.StartTime)
                   .IsRequired();
            
            builder.Property(er => er.EndTime)
                   .IsRequired();
            
            builder.Property(er => er.Status)
                   .HasDefaultValue(0)
                   .IsRequired()
                   .HasComment("0=Criado, 1=Aprovado, 2=Cancelado");
            
            // Foreign key to Equipment
            builder.HasOne(er => er.Equipment)
                   .WithMany()
                   .HasForeignKey(er => er.EquipmentId)
                   .OnDelete(DeleteBehavior.Restrict);
            
            // Foreign key to Account
            builder.HasOne(er => er.Account)
                   .WithMany()
                   .HasForeignKey(er => er.AccountId)
                   .OnDelete(DeleteBehavior.Restrict);
            
            // Foreign key to Reservation (optional)
            builder.HasOne(er => er.RoomReservation)
                   .WithMany()
                   .HasForeignKey(er => er.RoomReservationId)
                   .OnDelete(DeleteBehavior.SetNull)
                   .IsRequired(false);
            
            // Indexes for performance
            builder.HasIndex(er => er.EquipmentId)
                   .HasDatabaseName("IX_EquipmentReservations_EquipmentId");
            
            builder.HasIndex(er => er.AccountId)
                   .HasDatabaseName("IX_EquipmentReservations_AccountId");
            
            builder.HasIndex(er => er.RoomReservationId)
                   .HasDatabaseName("IX_EquipmentReservations_RoomReservationId");
            
            builder.HasIndex(er => er.Status)
                   .HasDatabaseName("IX_EquipmentReservations_Status");
            
            builder.HasIndex(er => new { er.StartTime, er.EndTime })
                   .HasDatabaseName("IX_EquipmentReservations_Period");
            
            // Compound index for availability checks
            builder.HasIndex(er => new { er.EquipmentId, er.StartTime, er.EndTime, er.Status })
                   .HasDatabaseName("IX_EquipmentReservations_Availability");
        }
    }
}
