using LigaDosPets.Application.Abstractions;
using LigaDosPets.Application.Abstractions.Repositories;
using LigaDosPets.Infrastructure.Persistence;
using LigaDosPets.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LigaDosPets.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // DbContext
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

            // UnitOfWork
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Repositórios individuais (opcional, já que o UoW expõe todos)
            services.AddScoped<IAdocaoRepository, EfAdocaoRepository>();
            services.AddScoped<IAgendamentoRepository, EfAgendamentoRepository>();
            services.AddScoped<IAnimalRepository, EfAnimalRepository>();
            services.AddScoped<ICaracteristicaRepository, EfCaracteristicaRepository>();
            services.AddScoped<IConversaRepository, EfConversaRepository>();
            services.AddScoped<IEspecieRepository, EfEspecieRepository>();
            services.AddScoped<IFotoRepository, EfFotoRepository>();
            services.AddScoped<IMensagemRepository, EfMensagemRepository>();
            services.AddScoped<INotificacaoRepository, EfNotificacaoRepository>();
            services.AddScoped<IPorteRepository, EfPorteRepository>();
            services.AddScoped<IRacaRepository, EfRacaRepository>();
            services.AddScoped<ISolicitacaoEntregaRepository, EfSolicitacaoEntregaRepository>();
            services.AddScoped<IStatusAgendamentoRepository, EfStatusAgendamentoRepository>();
            services.AddScoped<IStatusSolicitacaoRepository, EfStatusSolicitacaoRepository>();
            services.AddScoped<ITipoUsuarioRepository, EfTipoUsuarioRepository>();
            services.AddScoped<IUsuarioRepository, EfUsuarioRepository>();
            services.AddScoped<ISolicitacaoFotoRepository, EfSolicitacaoFotoRepository>();

            return services;
        }
    }
}