using LigaDosPets.Application.Abstractions.Repositories;
using LigaDosPets.Domain.Entities;
using LigaDosPets.Infrastructure.Persistence;

namespace LigaDosPets.Infrastructure.Repositories
{
    public class EfSolicitacaoFotoRepository : Repository<SolicitacaoFoto>, ISolicitacaoFotoRepository
    {
        public EfSolicitacaoFotoRepository(AppDbContext context) : base(context)
        {
        }
    }
}