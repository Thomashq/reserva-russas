using RR.Core.Entities.Base;

namespace RR.Core.Entities
{
    public class EquipmentReservation : BaseEntity
    {
        public int Id { get; set; }
        public int EquipmentId { get; set; }
        public int AccountId { get; set; }
        public int? RoomReservationId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int Status { get; set; } // 0=Criado, 1=Aprovado, 2=Cancelado

        public Equipment Equipment { get; set; } = null!;
        public Account Account { get; set; } = null!;
        public Reservation? RoomReservation { get; set; }
    }
}
