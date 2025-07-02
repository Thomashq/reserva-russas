using RR.Core.Entities;
using RR.Core.Repositories;
using RR.Infraestructure.DataContext;
using RR.Infraestructure.Repositories.Base;

namespace RR.Infraestructure.Repositories
{
    public class ServantRepository : BaseRepository<Servant, int>, IServantRepository
    {
        public ServantRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
