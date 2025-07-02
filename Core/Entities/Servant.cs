using RR.Core.Entities.Base;

namespace RR.Core.Entities;

public class Servant:BaseEntity
{
    public int Id { get; set; }

    public int AccountId { get; set; }

    //navegaçao via lazy loading
    public virtual Account Account { get; set; }
    public virtual ICollection<StudentAdvisor> AdvisedStudents { get; set; } = new List<StudentAdvisor>();
    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
} 
