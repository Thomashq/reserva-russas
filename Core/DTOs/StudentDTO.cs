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

        public AccountDTO? Account { get; set; }

        public List<ReservationDTO>? Reservation { get; set; }

        public List<ServantDTO>? Advisor { get; set; }

        public List<RoomDTO>? Permissions { get; set; }
    }
}
