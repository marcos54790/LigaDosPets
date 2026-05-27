using System.ComponentModel.DataAnnotations;

namespace LigaDosPets.Application.DTOs.Agendamento
{
    public class CreateAgendamentoDto
    {
        [Required(ErrorMessage = "O animal é obrigatório.")]
        public int AnimalId { get; set; }

        [Required(ErrorMessage = "A data da visita é obrigatória.")]
        [DataType(DataType.DateTime)]
        [FutureDate(ErrorMessage = "A data da visita deve ser futura.")]
        public DateTime DataVisita { get; set; }

        [StringLength(300)]
        public string? Observacao { get; set; }
    }

    // Validação customizada (coloque em um arquivo separado ou na mesma classe)
    public class FutureDateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is DateTime dateTime)
            {
                if (dateTime > DateTime.Now)
                    return ValidationResult.Success;
                return new ValidationResult(ErrorMessage ?? "A data deve ser futura.");
            }
            return new ValidationResult("Data inválida.");
        }
    }
}