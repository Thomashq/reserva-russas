using RR.Core.DTOs;

namespace RR.Core.Services
{
    public interface IStudentService
    {
        Task<IEnumerable<StudentDTO>> GetAllAsync();
        Task<StudentDTO?> GetByIdAsync(Guid id);
        Task<StudentDTO> CreateAsync(StudentDTO to);
        Task<StudentDTO?> UpdateAsync(Guid id, StudentDTO dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
