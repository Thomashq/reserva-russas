using RR.Core.Entities.Base;
using System.Collections.Generic;

namespace RR.Core.Entities
{
    //classe associada aos detalhes de uma sala, como regras especificas e etc
    public class RoomDetails : BaseEntity
    {
        public int Id { get; set; }

        public int RoomId { get; set; }

        public bool IsReserveable { get; set; }

        //a partir aqui , o tipo da sala sera representado por um int e pode aplicar uma regra especifica por reserva e assim por diante.
        public int RoomType { get; set; }

        public Rooms Room { get; set; } = null!;
        public ICollection<Equipment> EquipmentList { get; set; } = new List<Equipment>();
    }

    public class Equipment : BaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        public ICollection<RoomEquipment> RoomEquipments { get; set; } = new List<RoomEquipment>();
    }
}
