namespace LigaDosPets.Domain.Entities
{
    public class SolicitacaoEntrega
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public int StatusId { get; set; }
        public DateTime DataSolicitacao { get; set; } = DateTime.UtcNow;

        // Dados do animal informados pelo solicitante
        public string NomeAnimal { get; set; } = string.Empty;
        public string? DescricaoAnimal { get; set; }
        public string? MotivoEntrega { get; set; }
        public string? EspecieDescricao { get; set; }
        public string? RacaDescricao { get; set; }
        public string? IdadeEstimada { get; set; }
        public string? PorteDescricao { get; set; }
        public string? CaracteristicasDescricao { get; set; }

        /// <summary>Observação do Admin ao aprovar ou rejeitar.</summary>
        public string? ObservacaoAdmin { get; set; }

        /// <summary>
        /// Preenchido pelo Admin após aprovar e cadastrar o animal.
        /// Null enquanto pendente.
        /// </summary>
        public int? AnimalId { get; set; }

        // Navegação
        public Usuario Usuario { get; set; } = null!;
        public StatusSolicitacao Status { get; set; } = null!;
        public Animal? Animal { get; set; }
        public ICollection<SolicitacaoFoto> SolicitacaoFotos { get; set; } = new List<SolicitacaoFoto>();
    }
}