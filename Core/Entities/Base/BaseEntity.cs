namespace RR.Core.Entities.Base;

public interface IBaseEntity
{
    DateTime CreatedAt { get; set; }
    DateTime UpdatedAt { get; set; }
    bool IsActive { get; set; }
}
public abstract class BaseEntity:IBaseEntity
{
  public DateTime CreatedAt {get; set;}
  public DateTime UpdatedAt {get; set;}
  public bool IsActive {get; set;}
}
