using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RR.Core.DTOs
{
    public class AccountDTO
    {
        public Guid Id { get; set; } // Presume-se que haja um ID na classe BaseModel

        public string? UserName { get; set; }

        public string? Password { get; set; }

        public string? Email { get; set; }

        public string? Phone { get; set; }

        public int AccountPermission { get; set; }
    }
}
