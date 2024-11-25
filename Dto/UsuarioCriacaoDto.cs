using System.ComponentModel.DataAnnotations;

namespace NewRepository.Dto
{
    public class UsuarioCriacaoDto
    {
        [Required(ErrorMessage = "Insira um Email!")]
        [EmailAddress(ErrorMessage = "Insira um Email válido!")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Insira a razão social!")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "A razão social deve ter entre 5 e 100 caracteres.")]
        public string RazaoSocial { get; set; } = string.Empty;

        [Required(ErrorMessage = "Insira o nome fantasia!")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "O nome fantasia deve ter entre 3 e 100 caracteres.")]
        public string NomeFantasia { get; set; } = string.Empty;


        [Required(ErrorMessage = "Insira o CNPJ!")]
        [RegularExpression(@"\d{14}", ErrorMessage = "CNPJ deve conter 14 dígitos!")]
        public string Cnpj { get; set; } = string.Empty;


        [Required(ErrorMessage = "Insira o telefone!")]
        public string Telefone { get; set; } = string.Empty;


        [Required(ErrorMessage = "Insira o endereço!")]
        public string Endereco { get; set; } = string.Empty;
   


        [Required(ErrorMessage = "Insira uma senha!")]
        public string Senha { get; set; }

        [Required(ErrorMessage = "Confirme sua senha!")]
        [Compare("Senha", ErrorMessage = "Senhas estão diferentes!")]
        public string ConfirmaSenha { get; set; }
    }
}
