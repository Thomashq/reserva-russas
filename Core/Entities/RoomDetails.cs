using RR.Core.Entities.Base;

namespace RR.Core.Entities
{
    public class RoomDetails : BaseEntity
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public bool IsReserveable { get; set; }
        public int RoomType { get; set; }
        // Navigation properties
        public Rooms Room { get; set; } = null!;
        // Lista de equipamentos através da tabela de junção
        public ICollection<RoomEquipment> RoomEquipments { get; set; } = new List<RoomEquipment>();
    }
}
