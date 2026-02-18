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
            
            builder.HasKey(rd => rd.Id);
            builder.Property(rd => rd.Id)
                   .ValueGeneratedOnAdd();
            
            builder.Property(rd => rd.IsReserveable)
                   .IsRequired();
            
            builder.Property(rd => rd.RoomType)
                   .IsRequired();
            
            builder.Property(rd => rd.RoomId)
                   .IsRequired();
            
            builder.HasOne(rd => rd.Room)
                   .WithOne(r => r.RoomDetails)
                   .HasForeignKey<RoomDetails>(rd => rd.RoomId)
                   .OnDelete(DeleteBehavior.Cascade);
            
            // Indexes
            builder.HasIndex(rd => rd.RoomId)
                   .HasDatabaseName("IX_RoomDetails_RoomId");
        }
    }
    
    internal sealed class EquipmentConfiguration : IEntityTypeConfiguration<Equipment>
    {
        public void Configure(EntityTypeBuilder<Equipment> builder)
        {
            builder.ToTable("Equipment");
            
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
            
            // Composite primary key
            builder.HasKey(re => new { re.RoomDetailsId, re.EquipmentId });
            
            builder.Property(re => re.Quantity)
                   .HasDefaultValue(1)
                   .IsRequired();
            
            builder.HasOne(re => re.RoomDetails)
                   .WithMany(rd => rd.RoomEquipments)
                   .HasForeignKey(re => re.RoomDetailsId)
                   .OnDelete(DeleteBehavior.Cascade);
            
            // Foreign key to Equipment
            builder.HasOne(re => re.Equipment)
                   .WithMany(e => e.RoomEquipments)
                   .HasForeignKey(re => re.EquipmentId)
                   .OnDelete(DeleteBehavior.Restrict);
            
            builder.HasIndex(re => re.EquipmentId)
                   .HasDatabaseName("IX_RoomEquipments_EquipmentId");
            
            builder.HasIndex(re => re.RoomDetailsId)
                   .HasDatabaseName("IX_RoomEquipments_RoomDetailsId");
        }
    }
}
