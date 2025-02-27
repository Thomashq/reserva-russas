using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RR.Core.DTOs
{
    public class ReservationDTO
    {
        public Guid Id {get;set;}

        public AccountDTO Account {get;set;} 

        public required DateTime StartTime {get;set;}
  
        public required DateTime EndTime {get; set;}
    }
}
