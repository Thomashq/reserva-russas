using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RR.Core.Entities;

namespace RR.Infraestructure.Configuration
{
    internal sealed class RoomDetailsConfiguration : IEntityTypeConfiguration<RoomDetails>
    {
        public void Configure(EntityTypeBuilder<RoomDetails> builder)
        {
            builder.ToTable("RoomDetails");

            // Primary key
            builder.HasKey(rd => rd.Id);
            builder.Property(rd => rd.Id)
                   .ValueGeneratedOnAdd();

            // Properties
            builder.Property(rd => rd.IsReserveable)
                   .IsRequired();

            builder.Property(rd => rd.RoomType)
                   .IsRequired();

            builder.Property(rd => rd.RoomId)
                   .IsRequired();

            // Foreign key to Room (assumes RoomId property and Room navigation exist)
            builder.HasOne(rd => rd.Room)
                   .WithOne(r => r.RoomDetails)
                   .HasForeignKey<RoomDetails>(rd => rd.RoomId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Example index for quick lookup by RoomId
            builder.HasIndex(rd => rd.RoomId)
                   .HasDatabaseName("IX_RoomDetails_RoomId");
        }
    }

    internal sealed class EquipmentConfiguration : IEntityTypeConfiguration<Equipment>
    {
        public void Configure(EntityTypeBuilder<Equipment> builder)
        {
            builder.ToTable("Equipment");

            // Primary key
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id)
                   .ValueGeneratedOnAdd();

            // Properties
            builder.Property(e => e.Name)
                   .HasMaxLength(250)
                   .IsRequired();

            builder.Property(e => e.Description)
                   .HasMaxLength(1000)
                   .IsRequired(false);

            // Indexes
            builder.HasIndex(e => e.Name)
                   .HasDatabaseName("IX_Equipment_Name");
        }
    }

    internal sealed class RoomEquipmentConfiguration : IEntityTypeConfiguration<RoomEquipment>
    {
        public void Configure(EntityTypeBuilder<RoomEquipment> builder)
        {
            builder.ToTable("RoomEquipments");

            builder.HasKey(re => new { re.RoomDetailsId, re.EquipmentId });

            builder.Property(re => re.Quantity)
                   .HasDefaultValue(1)
                   .IsRequired();

            builder.HasOne(re => re.RoomDetails)
                   .WithMany() 
                   .HasForeignKey(re => re.RoomDetailsId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(re => re.Equipment)
                   .WithMany(e => e.RoomEquipments)
                   .HasForeignKey(re => re.EquipmentId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Indexes for common queries
            builder.HasIndex(re => re.EquipmentId)
                   .HasDatabaseName("IX_RoomEquipments_EquipmentId");
        }
    }
}
