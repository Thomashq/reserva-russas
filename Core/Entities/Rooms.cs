using RR.Core.Entities.Base;

namespace RR.Core.Entities;

public class Rooms:BaseEntity 
{
    public int Id { get; set; }

    public string Name { get; set; }

    public int Capacity { get; set; }

    public int ManagerId { get; set; }

    //lazy loading
    public Manager Manager { get; set; }

    public RoomDetails RoomDetails { get; set; }

    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
