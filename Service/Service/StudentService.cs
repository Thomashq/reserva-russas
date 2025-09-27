using RR.Core.Entities;
using RR.Core.Repositories;
using RR.Core.Services;
using RR.Core.Services.Base;

namespace RR.Service.Service
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _studentRepository;

        public StudentService(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }

        public async Task<bool> AddAsync(Student student)
        {
            return await _studentRepository.AddAsync(student);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _studentRepository.DeleteAsync(id);
        }

        public async Task<Student> GetServantById(int id)
        {
            return await _studentRepository.GetStudentById(id);
        }

        public async Task<Student> GetStudentByAccountId(int id)
        {
            return await _studentRepository.GetStudentByAccountId(id);
        }

        public async Task<Student> UpdateAsync(Student student)
        {
            return await _studentRepository.UpdateAsync(student);
        }
    }
}