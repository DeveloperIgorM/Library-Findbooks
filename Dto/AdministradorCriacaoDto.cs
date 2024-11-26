using System.ComponentModel.DataAnnotations;

namespace NewRepository.Dto
{
    public class AdministradorCriacaoDto
    {
        public string Nome { get; set; }

        [Required(ErrorMessage = "Insira um Email!")]
        [EmailAddress(ErrorMessage = "Insira um Email válido!")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Insira uma senha!")]
        public string Senha { get; set; }
    }
}
