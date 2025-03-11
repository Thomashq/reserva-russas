using RR.Core.DTOs;

namespace RR.Core.Services
{
    public interface IServantService
    {
        Task<IEnumerable<ServantDTO>> GetAllAsync();
        Task<ServantDTO?> GetByIdAsync(Guid id);
        Task<ServantDTO> CreateAsync(ServantDTO dto);
        Task<ServantDTO?> UpdateAsync(Guid id, ServantDTO dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
