namespace Domain.Models;
public abstract class BaseModel
{
  public Guid Id {get; set;}
  public DateTime? CreationDate {get; set;}
  public DateTime? UpdateDate {get; set;}
  public DateTime? DeleteDate {get; set;}
}
