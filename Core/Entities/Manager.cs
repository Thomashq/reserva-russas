using RR.Core.Entities.Base;

namespace RR.Core.Entities;

public class Manager:BaseEntity
{
    public int Id { get; set; }

    public int AccountId { get; set; }

    //lazy loading das coisas
    public virtual Account Account { get; set; }
    public virtual ICollection<Rooms> ManagedRooms { get; set; } = new List<Rooms>();
} 
