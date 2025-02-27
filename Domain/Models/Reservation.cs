namespace Domain.Models;

public class Reservation: BaseModel 
{
    public required Guid RoomId {get;set;}

    public required Guid AccountId {get;set;} 

    public required DateTime StartTime {get;set;}
  
    public required DateTime EndTime {get; set;}
}

