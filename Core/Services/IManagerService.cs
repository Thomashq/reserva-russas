using RR.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RR.Core.Services
{
    public interface IManagerService
    {
        Task<IEnumerable<ManagerDTO>> GetAllAsync();
        Task<ManagerDTO?> GetByIdAsync(Guid id);
        Task<ManagerDTO> CreateAsync(ManagerDTO dto);
        Task<ManagerDTO?> UpdateAsync(Guid id, ManagerDTO dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
