namespace Domain.Models;

public class Manager:Account 
{
  public List<long> ManagedRooms {get; set;}  
} 
