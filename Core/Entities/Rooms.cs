using RR.Core.Entities.Base;

namespace RR.Core.Entities;

public class Rooms:BaseEntity 
{
    public int Id { get; set; }

    public string Name { get; set; }

    public int Capacity { get; set; }

    public int ManagerId { get; set; }

    //lazy loading
    public virtual Manager Manager { get; set; }
    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
