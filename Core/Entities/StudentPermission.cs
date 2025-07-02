using RR.Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RR.Core.Entities
{
    public class StudentPermission:BaseEntity
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int PermissionId { get; set; }

        // Navigation Properties
        public virtual Student Student { get; set; }
    }
}
