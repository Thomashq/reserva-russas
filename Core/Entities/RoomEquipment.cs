using System;

namespace RR.Core.Entities
{
    public class RoomEquipment
    {
        // Composite key: { RoomDetailsId, EquipmentId }
        public int RoomDetailsId { get; set; }
        public int EquipmentId { get; set; }

        // Optional extra columns
        public int Quantity { get; set; } = 1;
        public string? Notes { get; set; }

        // Navigation properties
        public RoomDetails RoomDetails { get; set; } = null!;
        public Equipment Equipment { get; set; } = null!;
    }
}