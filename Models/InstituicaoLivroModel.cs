using System.ComponentModel.DataAnnotations;


namespace NewRepository.Models
{
    public class InstituicaoLivroModel
    {
        [Required]
        public int Id { get; set; } // Identificador único do relacionamento

        [Required]
        public int UsuarioId { get; set; } // Identificador da Instituição

        [Required]
        public int LivroId { get; set; } // Identificador do Livro

        [Required]
        public int Quantidade { get; set; } // Quantidade de exemplares disponíveis na Instituição

        // Relacionamentos
        public UsuarioModel Usuario { get; set; } = null!; // Relacionamento com a Instituição (Usuário)
        public LivroModel Livro { get; set; } = null!;     // Relacionamento com o Livro
    }

}
