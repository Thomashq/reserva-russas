using RR.Core.Entities;
using RR.Core.Repositories;
using RR.Core.Services;
using RR.Core.Services.Base;

namespace RR.Service.Service
{
    public class StudentService : BaseService<Student, int>, IStudentService
    {
        private readonly IStudentRepository _studentRepository;
        public StudentService(IStudentRepository studentRepository) : base(studentRepository) { }
    }
}
