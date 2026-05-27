namespace LigaDosPets.Application.DTOs.SolicitacaoEntrega
{
    public class SolicitacaoDto
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string NomeUsuario { get; set; } = string.Empty;
        public int StatusId { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime DataSolicitacao { get; set; }
        public string NomeAnimal { get; set; } = string.Empty;
        public string? DescricaoAnimal { get; set; }
        public string? MotivoEntrega { get; set; }
        public string? EspecieDescricao { get; set; }
        public string? RacaDescricao { get; set; }
        public string? IdadeEstimada { get; set; }
        public string? ObservacaoAdmin { get; set; }
        public int? AnimalId { get; set; }

        // Novas propriedades
        public string? PorteDescricao { get; set; }
        public string? CaracteristicasDescricao { get; set; }
        public string? EmailUsuario { get; set; }
        public string? TelefoneUsuario { get; set; }
        public string? CepUsuario { get; set; }

        public List<string> Fotos { get; set; } = new();
    }
}