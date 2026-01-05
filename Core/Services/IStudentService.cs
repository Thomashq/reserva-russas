using RR.Core.Entities;

namespace RR.Core.Services
{
    public interface IStudentService
    {
        Task<IEnumerable<Student>> GetAllAsync();
        Task<bool> AddAsync(Student student);
        Task<Student> GetStudentById(int id);
        Task<Student> UpdateAsync(Student servant);
        Task<bool> DeleteAsync(int id);
        Task<Student> GetStudentByAccountId(int id);
    }
}
