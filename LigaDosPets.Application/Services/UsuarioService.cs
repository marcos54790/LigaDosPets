using LigaDosPets.Application.Abstractions;
using LigaDosPets.Application.Abstractions.Identity;
using LigaDosPets.Application.Abstractions.Repositories;
using LigaDosPets.Application.Abstractions.Services;
using LigaDosPets.Application.DTOs.Usuario;
using LigaDosPets.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LigaDosPets.Application.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ITipoUsuarioRepository _tipoUsuarioRepository;
        private readonly IIdentityService _identityService;
        private readonly IUnitOfWork _unitOfWork;

        // Mapeamento fixo entre nome do TipoUsuario (domínio) e role do Identity
        private static readonly Dictionary<string, string> _roleMap = new()
        {
            { "Usuário",        "Usuário"       },
            { "Gerente",        "Gerente"       },
            { "Administrador",  "Administrador" }
        };

        public UsuarioService(
            IUsuarioRepository usuarioRepository,
            ITipoUsuarioRepository tipoUsuarioRepository,
            IIdentityService identityService,
            IUnitOfWork unitOfWork)
        {
            _usuarioRepository = usuarioRepository;
            _tipoUsuarioRepository = tipoUsuarioRepository;
            _identityService = identityService;
            _unitOfWork = unitOfWork;
        }

        // ── Mappers ────────────────────────────────────────────────────────────
        private static UsuarioPerfilDto MapToPerfilDto(Usuario u) => new()
        {
            Id = u.Id,
            Nome = u.Nome,
            Email = u.Email,
            Telefone = u.Telefone,
            Cep = u.Cep,
            PerfilDoadorCompleto = u.PerfilDoadorCompleto,
            TipoUsuario = u.TipoUsuario?.Nome ?? string.Empty,
            DataCadastro = u.DataCadastro,
            Ativo = u.Ativo
        };

        private static UsuarioListDto MapToListDto(Usuario u) => new()
        {
            Id = u.Id,
            Nome = u.Nome,
            Email = u.Email,
            TipoUsuario = u.TipoUsuario?.Nome ?? string.Empty,
            Ativo = u.Ativo
        };

        // ── Consultas ──────────────────────────────────────────────────────────
        public async Task<UsuarioPerfilDto?> GetPerfilAsync(int id)
        {
            var u = await _usuarioRepository.Query()
                .Include(u => u.TipoUsuario)
                .FirstOrDefaultAsync(u => u.Id == id);
            return u == null ? null : MapToPerfilDto(u);
        }

        public async Task<UsuarioPerfilDto?> GetPerfilPorApplicationUserIdAsync(string applicationUserId)
        {
            var u = await _usuarioRepository.ObterPorApplicationUserIdAsync(applicationUserId);
            return u == null ? null : MapToPerfilDto(u);
        }

        public async Task<IEnumerable<UsuarioListDto>> GetAllAsync()
        {
            var list = await _usuarioRepository.Query()
                .Include(u => u.TipoUsuario)
                .OrderBy(u => u.Nome)
                .ToListAsync();
            return list.Select(MapToListDto);
        }

        public async Task<bool> EmailExisteAsync(string email)
            => await _usuarioRepository.EmailExisteAsync(email);

        // ── Criação de usuário ─────────────────────────────────────────────────
        /// <summary>Registro público — sempre cria como "Usuário".</summary>
        public async Task<UsuarioPerfilDto> RegistrarUsuarioExternoAsync(RegistroUsuarioDto dto)
        {
            const string role = "Usuário";
            var appUserId = await _identityService.CreateUserAsync(dto.Email, dto.Senha, role);

            var tipo = await _tipoUsuarioRepository.ObterPorNomeAsync(role);
            if (tipo == null) throw new InvalidOperationException($"TipoUsuario '{role}' não encontrado.");

            var usuario = new Usuario
            {
                Nome = dto.Nome,
                Email = dto.Email,
                Telefone = dto.Telefone,
                ApplicationUserId = appUserId,
                TipoUsuarioId = tipo.Id,
                DataCadastro = DateTime.UtcNow,
                Ativo = true
            };
            await _usuarioRepository.AddAsync(usuario);
            await _unitOfWork.SaveChangesAsync();
            return MapToPerfilDto(usuario);
        }

        /// <summary>Criação pelo admin — respeita o tipoUsuarioId escolhido.</summary>
        public async Task<UsuarioPerfilDto> CriarUsuarioAsync(RegistroUsuarioDto dto, int tipoUsuarioId)
        {
            if (await _usuarioRepository.EmailExisteAsync(dto.Email))
                throw new InvalidOperationException("Este e-mail já está cadastrado.");

            var tipo = await _tipoUsuarioRepository.GetByIdAsync(tipoUsuarioId);
            if (tipo == null) throw new InvalidOperationException("Tipo de usuário inválido.");

            // Usa o nome do TipoUsuario como role no Identity
            var role = _roleMap.GetValueOrDefault(tipo.Nome, "Usuário");
            var appUserId = await _identityService.CreateUserAsync(dto.Email, dto.Senha, role);

            var usuario = new Usuario
            {
                Nome = dto.Nome,
                Email = dto.Email,
                Telefone = dto.Telefone,
                ApplicationUserId = appUserId,
                TipoUsuarioId = tipoUsuarioId,
                DataCadastro = DateTime.UtcNow,
                Ativo = true
            };
            await _usuarioRepository.AddAsync(usuario);
            await _unitOfWork.SaveChangesAsync();
            return MapToPerfilDto(usuario);
        }

        // ── Promoção de role ───────────────────────────────────────────────────
        public async Task PromoverUsuarioAsync(int id, string novaRole)
        {
            if (!_roleMap.ContainsKey(novaRole))
                throw new InvalidOperationException($"Role '{novaRole}' inválida.");

            var usuario = await _usuarioRepository.Query()
                .Include(u => u.TipoUsuario)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (usuario == null)
                throw new InvalidOperationException("Usuário não encontrado.");

            // Não permite rebaixar o próprio Administrador para evitar lock-out
            if (usuario.TipoUsuario?.Nome == "Administrador" && novaRole != "Administrador")
                throw new InvalidOperationException(
                    "Não é possível rebaixar um Administrador por aqui. " +
                    "Faça isso diretamente pelo painel de identidade.");

            // Atualiza role no Identity
            await _identityService.AlterarRoleAsync(usuario.ApplicationUserId, novaRole);

            // Atualiza TipoUsuario no domínio
            var novoTipo = await _tipoUsuarioRepository.ObterPorNomeAsync(novaRole);
            if (novoTipo == null)
                throw new InvalidOperationException($"TipoUsuario '{novaRole}' não encontrado no banco.");

            usuario.TipoUsuarioId = novoTipo.Id;
            await _usuarioRepository.UpdateAsync(usuario);
            await _unitOfWork.SaveChangesAsync();
        }

        // ── Ativar / Desativar / Excluir ───────────────────────────────────────
        public async Task AtualizarPerfilAsync(int id, UpdateUsuarioDto dto)
        {
            var u = await _usuarioRepository.GetByIdAsync(id);
            if (u == null) throw new InvalidOperationException("Usuário não encontrado.");
            u.Nome = dto.Nome;
            u.Telefone = dto.Telefone;
            await _usuarioRepository.UpdateAsync(u);
            await _unitOfWork.SaveChangesAsync();
        }

        // Mantém assinatura original mas retorna void internamente
        async Task<UsuarioPerfilDto> IUsuarioService.AtualizarPerfilAsync(int id, UpdateUsuarioDto dto)
        {
            var u = await _usuarioRepository.GetByIdAsync(id);
            if (u == null) throw new InvalidOperationException("Usuário não encontrado.");
            u.Nome = dto.Nome;
            u.Telefone = dto.Telefone;
            await _usuarioRepository.UpdateAsync(u);
            await _unitOfWork.SaveChangesAsync();
            return MapToPerfilDto(u);
        }

        public async Task AtualizarDadosDoadorAsync(string applicationUserId, CompletarPerfilDto dto)
        {
            var u = await _usuarioRepository.ObterPorApplicationUserIdAsync(applicationUserId);
            if (u == null) throw new InvalidOperationException("Usuário não encontrado.");
            u.Cep = dto.Cep;
            u.Logradouro = dto.Logradouro;
            u.Numero = dto.Numero;
            u.Bairro = dto.Bairro;
            u.Cidade = dto.Cidade;
            u.Estado = dto.Estado;
            u.Telefone = dto.Telefone;
            u.PerfilDoadorCompleto = true;
            await _usuarioRepository.UpdateAsync(u);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DesativarUsuarioAsync(int id)
        {
            var u = await _usuarioRepository.GetByIdAsync(id);
            if (u == null) throw new InvalidOperationException("Usuário não encontrado.");
            u.Ativo = false;
            await _usuarioRepository.UpdateAsync(u);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task AtivarUsuarioAsync(int id)
        {
            var u = await _usuarioRepository.GetByIdAsync(id);
            if (u == null) throw new InvalidOperationException("Usuário não encontrado.");
            u.Ativo = true;
            await _usuarioRepository.UpdateAsync(u);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task ExcluirUsuarioAsync(int id)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(id);
            if (usuario == null) throw new InvalidOperationException("Usuário não encontrado.");
            await _usuarioRepository.DeleteAsync(usuario);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<UsuarioPerfilDto?> GetAdminPrincipalAsync()
        {
            var admins = await _usuarioRepository.ObterAdministradoresAsync();
            var admin = admins.FirstOrDefault(u => u.Ativo);
            if (admin == null) return null;
            return MapToPerfilDto(admin);
        }
    }
}