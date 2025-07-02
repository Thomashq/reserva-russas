using RR.Core.Entities;
using RR.Core.Repositories;
using RR.Core.Services.Base;

namespace RR.Service.Service
{
    public class ServantService : BaseService<Servant, int>, IServantService
    {
        private readonly IServantRepository _servantRepository;

        public ServantService(IServantRepository servantRepository) :base(servantRepository)
        {
        }
    }
}
