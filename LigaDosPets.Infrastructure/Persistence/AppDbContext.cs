using LigaDosPets.Domain.Entities;
using LigaDosPets.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace LigaDosPets.Infrastructure.Persistence
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<SolicitacaoFoto> SolicitacaoFotos { get; set; }
        public DbSet<Adocao> Adocoes { get; set; }
        public DbSet<Agendamento> Agendamentos { get; set; }
        public DbSet<Animal> Animais { get; set; }
        public DbSet<AnimalCaracteristica> AnimaisCaracteristicas { get; set; }
        public DbSet<Caracteristica> Caracteristicas { get; set; }
        public DbSet<Conversa> Conversas { get; set; }
        public DbSet<Especie> Especies { get; set; }
        public DbSet<Foto> Fotos { get; set; }
        public DbSet<Mensagem> Mensagens { get; set; }
        public DbSet<Notificacao> Notificacoes { get; set; }
        public DbSet<Porte> Portes { get; set; }
        public DbSet<Raca> Racas { get; set; }
        public DbSet<SolicitacaoEntrega> SolicitacoesEntregas { get; set; }
        public DbSet<StatusAgendamento> StatusAgendamentos { get; set; }
        public DbSet<StatusSolicitacao> StatusSolicitacoes { get; set; }
        public DbSet<TipoUsuario> TiposUsuarios { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // =======================================================
            // Configurações de Relacionamentos (Fluent API)
            // =======================================================

            // ---------- Adocao ----------
            builder.Entity<Adocao>()
                .HasOne(a => a.UsuarioAdotante)
                .WithMany(u => u.Adocoes)
                .HasForeignKey(a => a.UsuarioAdotanteId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Adocao>()
                .HasOne(a => a.AdminConfirmou)
                .WithMany(u => u.AdocoesConfirmadas)
                .HasForeignKey(a => a.AdminConfirmouId)
                .OnDelete(DeleteBehavior.Restrict);

            // ---------- Conversa ----------
            builder.Entity<Conversa>()
                .HasOne(c => c.UsuarioA)
                .WithMany(u => u.ConversasComoA)
                .HasForeignKey(c => c.UsuarioAId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Conversa>()
                .HasOne(c => c.UsuarioB)
                .WithMany(u => u.ConversasComoB)
                .HasForeignKey(c => c.UsuarioBId)
                .OnDelete(DeleteBehavior.Restrict);

            // ---------- Animal (relacionamentos básicos já são inferidos, mas podemos reforçar) ----------
            builder.Entity<Animal>()
                .HasOne(a => a.Porte)
                .WithMany(p => p.Animais)
                .HasForeignKey(a => a.PorteId);

            builder.Entity<Animal>()
                .HasOne(a => a.Especie)
                .WithMany(e => e.Animais)
                .HasForeignKey(a => a.EspecieId);

            builder.Entity<Animal>()
                .HasOne(a => a.Raca)
                .WithMany(r => r.Animais)
                .HasForeignKey(a => a.RacaId)
                .IsRequired(false);

            builder.Entity<Animal>()
                .HasOne(a => a.UsuarioCadastro)
                .WithMany(u => u.AnimaisCadastrados)
                .HasForeignKey(a => a.UsuarioCadastroId);

            // ---------- AnimalCaracteristica (chave composta) ----------
            builder.Entity<AnimalCaracteristica>()
                .HasKey(ac => new { ac.AnimalId, ac.CaracteristicaId });

            builder.Entity<AnimalCaracteristica>()
                .HasOne(ac => ac.Animal)
                .WithMany(a => a.AnimalCaracteristicas)
                .HasForeignKey(ac => ac.AnimalId);

            builder.Entity<AnimalCaracteristica>()
                .HasOne(ac => ac.Caracteristica)
                .WithMany(c => c.AnimalCaracteristicas)
                .HasForeignKey(ac => ac.CaracteristicaId);

            // ---------- Foto ----------
            builder.Entity<Foto>()
                .HasOne(f => f.Animal)
                .WithMany(a => a.Fotos)
                .HasForeignKey(f => f.AnimalId)
                .OnDelete(DeleteBehavior.Cascade);

            // ---------- Agendamento ----------
            // ---------- Agendamento ----------
            builder.Entity<Agendamento>()
                .HasOne(a => a.Animal)
                .WithMany(an => an.Agendamentos)
                .HasForeignKey(a => a.AnimalId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Agendamento>()
                .HasOne(a => a.Usuario)
                .WithMany(u => u.Agendamentos)
                .HasForeignKey(a => a.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);   

            builder.Entity<Agendamento>()
                .HasOne(a => a.StatusAgendamento)
                .WithMany(s => s.Agendamentos)
                .HasForeignKey(a => a.StatusAgendamentoId)
                .OnDelete(DeleteBehavior.Restrict);

            // ---------- Mensagem ----------
            builder.Entity<Mensagem>()
                .HasOne(m => m.Conversa)
                .WithMany(c => c.Mensagens)
                .HasForeignKey(m => m.ConversaId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Mensagem>()
                .HasOne(m => m.Remetente)
                .WithMany(u => u.MensagensEnviadas)
                .HasForeignKey(m => m.RemetenteId)
                .OnDelete(DeleteBehavior.Restrict);

            // ---------- Notificacao ----------
            builder.Entity<Notificacao>()
                .HasOne(n => n.Usuario)
                .WithMany(u => u.Notificacoes)
                .HasForeignKey(n => n.UsuarioId);

            // ---------- SolicitacaoEntrega ----------
            builder.Entity<SolicitacaoEntrega>()
                .HasOne(s => s.Usuario)
                .WithMany(u => u.Solicitacoes)
                .HasForeignKey(s => s.UsuarioId);

            builder.Entity<SolicitacaoEntrega>()
                .HasOne(s => s.Status)
                .WithMany(st => st.Solicitacoes)
                .HasForeignKey(s => s.StatusId);

            builder.Entity<SolicitacaoEntrega>()
                .HasOne(s => s.Animal)
                .WithMany(a => a.SolicitacoesEntrega)
                .HasForeignKey(s => s.AnimalId)
                .IsRequired(false);

            // ---------- Raca ----------
            builder.Entity<Raca>()
                .HasOne(r => r.Especie)
                .WithMany(e => e.Racas)
                .HasForeignKey(r => r.EspecieId);

            // ---------- Usuario (relacionamento com IdentityUser) ----------
            builder.Entity<Usuario>()
                .HasOne<ApplicationUser>()
                .WithOne(u => u.Usuario)
                .HasForeignKey<Usuario>(u => u.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Aplica quaisquer configurações adicionais via IEntityTypeConfiguration
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}