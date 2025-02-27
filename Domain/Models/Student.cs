namespace Domain.Models;

public class Student:BaseModel
{
    public Guid AccountId { get; set; }

    public Account Account { get; set; }
    
    public List<Guid> Reservations {get; set;}  

    public List<Guid> Advisor {get; set;}  

    public List<Guid>? Permissions {get; set;}  
}  
