using RR.Core.Entities.Base;

namespace RR.Core.Entities
{
    public class RoomEquipment : BaseEntity
    {
        public int RoomDetailsId { get; set; }
        public int EquipmentId { get; set; }
        public int Quantity { get; set; }

        public RoomDetails RoomDetails { get; set; } = null!;
        public Equipment Equipment { get; set; } = null!;
    }
}
