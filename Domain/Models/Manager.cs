namespace Domain.Models;

public class Manager:BaseModel
{
    public Guid AccountId { get; set; }

    public Account Account { get; set; }

    public List<Guid>? ManagedRooms {get; set;}  
} 
