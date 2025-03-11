namespace Domain.Models;

public class Account : BaseModel 
{
    public string UserName {get; set;}

    public string PasswordHash {get; set;}

    public string Mail {get; set;}

    public string? Phone {get; set;}

    public int AccountPermission { get; set; }
}
