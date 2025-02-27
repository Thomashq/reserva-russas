namespace Domain.Models;

public class Rooms:BaseModel 
{
    public required string Name { get; set; }

    public required int Capacity { get; set; }

    public long ManagerId { get; set; }
}
