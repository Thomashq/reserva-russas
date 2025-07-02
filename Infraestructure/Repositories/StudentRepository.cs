using RR.Core.Entities;
using RR.Core.Repositories;
using RR.Infraestructure.DataContext;
using RR.Infraestructure.Repositories.Base;

namespace RR.Infraestructure.Repositories
{
    public class StudentRepository : BaseRepository<Student, int>, IStudentRepository
    {
        public StudentRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
