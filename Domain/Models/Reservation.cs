namespace Domain.Models;

public class Reservation: BaseModel 
{
  public required long RoomId {get;set;}

  public required long AccountId {get;set;} 

  public required DateTime StartTime {get;set;}
  
  public required DateTime EndTime {get; set;}
}

