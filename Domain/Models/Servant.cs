namespace Domain.Models;

public class Servant:Account 
{
  public List<long> Advisee {get; set;}  

  public List<long> Reservation {get; set;} = new();  
} 
