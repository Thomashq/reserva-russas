namespace Domain.Models;

public class Servant:BaseModel
{
    public Guid AccountId { get; set; }

    public Account Account { get; set; }

    public List<Guid> Advisee {get; set;}  

    public List<Guid> Reservation {get; set;} = new();  
} 
