using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RR.Core.DTOs
{
    public class OcurrencePreviewDto
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int? RoomId { get; set; }
    }
}
