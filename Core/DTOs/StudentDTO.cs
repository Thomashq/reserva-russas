using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RR.Core.DTOs
{
    public class StudentDTO:AccountDTO
    {
        public Guid Id { get; set; }

        public Guid AccountId { get; set; }

        public AccountDTO? Account { get; set; }

        public List<Guid> Reservations { get; set; }

        public List<ReservationDTO>? ReservationList { get; set; }

        public List<Guid> Advisor { get; set; }

        public List<ServantDTO>? AdvisorList { get; set; }

        public List<Guid> Permissions { get; set; }

        public List<RoomDTO>? PermissionsList { get; set; }
    }
}
