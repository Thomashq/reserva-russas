using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RR.Core.DTOs.Requests
{
    public class ApplyExceptionSkipRequest
    {
        [Required] public int SeriesId { get; set; }
        /// <summary>Data local da ocorrência original (ignora hora; ela virá da série).</summary>
        [Required] public DateTime OriginalDate { get; set; }
        [MaxLength(1000)] public string Reason { get; set; }
    }

    public class ApplyExceptionMoveRequest
    {
        [Required] public int SeriesId { get; set; }
        [Required] public DateTime OriginalDate { get; set; }

        [Required] public DateTime NewStartTime { get; set; }
        [Required] public DateTime NewEndTime { get; set; }
        public int? NewRoomId { get; set; }
        [MaxLength(1000)] public string Reason { get; set; }
    }
}
