namespace Domain.Models;

public class Student:Account 
{
  public List<long> Reservations {get; set;}  

  public List<long> Advisor {get; set;}  

  public List<long>? Permissions {get; set;}  
}  
