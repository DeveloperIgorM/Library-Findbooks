using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NewRepository.Models
{
    public class UsuarioModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Insira um Email válido!")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Insira a razão social!")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "A razão social deve ter entre 5 e 100 caracteres.")]
        public string RazaoSocial { get; set; } = string.Empty;

        [Required(ErrorMessage = "Insira o nome fantasia!")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "O nome fantasia deve ter entre 3 e 100 caracteres.")]
        public string NomeFantasia { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"\d{14}", ErrorMessage = "CNPJ inválido.")]
        public string Cnpj { get; set; } = string.Empty;

        [Phone]
        public string Telefone { get; set; } = string.Empty;
        public string Endereco { get; set; } = string.Empty;

        [Required]
        public byte[] SenhaHash { get; set; }
        [Required]
        public byte[] SenhaSalt { get; set; }

        public DateTime DataCriacao { get; set; } = DateTime.Now;
        public int Status { get; set; } = 0;    

        public ICollection<LivroModel> Livros { get; set; } = new List<LivroModel>();
        public ICollection<InstituicaoLivroModel> InstituicaoLivros { get; set; } = new List<InstituicaoLivroModel>();
    }
}
