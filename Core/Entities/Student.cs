using RR.Core.Entities.Base;

namespace RR.Core.Entities;

public class Student:BaseEntity
{
    public int Id { get; set; }

    public int AccountId { get; set; }

    //lazy loading novamente
    public virtual Account Account { get; set; }
    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    public virtual ICollection<StudentAdvisor> Advisors { get; set; } = new List<StudentAdvisor>();
    public virtual ICollection<StudentPermission> Permissions { get; set; } = new List<StudentPermission>();
}  
