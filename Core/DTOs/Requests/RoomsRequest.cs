using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RR.Core.DTOs.Requests
{
    public class CreateRoomRequest
    {
        public string Name { get; set; }
        public int Capacity { get; set; }
        public int ManagerId { get; set; }
    }
}
