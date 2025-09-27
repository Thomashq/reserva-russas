using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RR.Core.DTOs.Requests
{
    // ---- Disponibilidade / Agenda ----
    public class IsRoomOccupiedRequest 
    {
        [Required] public int RoomId { get; set; }
        [Required] public DateTime StartTime { get; set; }
        [Required] public DateTime EndTime { get; set; }
    }

    public class FindRoomGapsRequest
    {
        [Required] public int RoomId { get; set; }
        [Required] public DateTime Day { get; set; }      // usar só a parte da data
        [Required] public TimeSpan OpenTime { get; set; } // ex.: 08:00
        [Required] public TimeSpan CloseTime { get; set; } // ex.: 22:00
    }

    public class GetRoomScheduleRequest
    {
        [Required] public int RoomId { get; set; }
        [Required] public DateTime Day { get; set; } // usar só a parte da data
        public bool IncludeCancelled { get; set; } = false;
    }

    // (Opcional) Prévia por período
    public class PreviewConflictsRequest
    {
        [Required] public int RoomId { get; set; }
        [Required] public DateTime From { get; set; }
        [Required] public DateTime To { get; set; }
        public int? AccountId { get; set; }
        public string ClassCode { get; set; }
    }
}
