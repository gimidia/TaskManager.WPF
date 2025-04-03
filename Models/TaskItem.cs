using System;
using System.ComponentModel.DataAnnotations;

namespace TaskManager.WPF.Models
{
    public class TaskItem
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O título é obrigatório")]
        [StringLength(100, ErrorMessage = "O título não pode ter mais que 100 caracteres")]
        public string? Titulo { get; set; }

        [StringLength(500, ErrorMessage = "A descrição não pode ter mais que 500 caracteres")]
        public string? Descricao { get; set; }

        [Range(0, 2, ErrorMessage = "Status inválido")]
        public int Status { get; set; }

        public string StatusNome => Status switch
        {
            0 => "Pendente",
            1 => "Em Progresso",
            2 => "Concluída",
            _ => "Desconhecido"
        };

        public DateTime DataCriacao { get; set; }
        public string DataCriacaoFormatada => DataCriacao.ToString("dd/MM/yyyy HH:mm");

        public DateTime? DataConclusao { get; set; }
        public string DataConclusaoFormatada => DataConclusao?.ToString("dd/MM/yyyy") ?? "Não concluída";
    }
}
