using RR.Core.Entities.Base;

namespace RR.Core.Entities
{
    public class StudentAdvisor:BaseEntity
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int ServantId { get; set; }

        public virtual Student Student { get; set; }
        public virtual Servant Servant { get; set; }
    }
}
