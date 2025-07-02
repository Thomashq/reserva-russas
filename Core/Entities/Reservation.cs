using RR.Core.Entities.Base;

namespace RR.Core.Entities;

public class Reservation: BaseEntity 
{
    public int Id { get; set; }

    public int RoomId {get;set;}

    public int AccountId {get;set;} 

    public DateTime StartTime {get;set;}
  
    public DateTime EndTime {get; set;}

    // Navigation Properties
    public virtual Rooms Room { get; set; }
    public virtual Account Account { get; set; }
}

