using RR.Core.Entities;
using RR.Core.Entities.Base;
using RR.Core.Services.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RR.Core.Services
{
    public interface IStudentService
    {
        Task<bool> AddAsync(Student student);
        Task<Student> GetServantById(int id);
        Task<Student> UpdateAsync(Student servant);
        Task<bool> DeleteAsync(int id);
        Task<Student> GetStudentByAccountId(int id);
    }
}