
using RR.Core.Entities;

namespace RR.Core.Repositories
{
    public interface IStudentRepository
    {
        Task<IEnumerable<Student>> GetAllAsync();
        Task<bool> AddAsync(Student student);
        Task<Student> GetStudentById(int id);
        Task<Student> UpdateAsync(Student servant);
        Task<bool> DeleteAsync(int id);
        Task<Student> GetStudentByAccountId(int id);
    }
}
