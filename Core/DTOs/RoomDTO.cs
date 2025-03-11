using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RR.Core.DTOs
{
    public class RoomDTO
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public int Capacity { get; set; }

        public ManagerDTO Manager { get; set; }

        public Guid ManagerId { get; set; }
    }
}
