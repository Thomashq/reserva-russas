using RR.Core.Entities.Base;

namespace RR.Core.Entities;

public class Account : BaseEntity 
{
    public int Id { get; set; }

    public int UserId { get; set; } // conexão com o appuser para aspnet identity

    public string UserName {get; set;}

    public string Mail {get; set;}

    public string? Phone {get; set;}

    public int AccountPermission { get; set; }

    //propriedade de navegação
    public virtual Manager? Manager { get; set; }
    public virtual Servant? Servant { get; set; }
    public virtual Student? Student { get; set; }
    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
