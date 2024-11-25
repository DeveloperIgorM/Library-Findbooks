namespace NewRepository.Models
{
    using System.ComponentModel.DataAnnotations;

    namespace NewRepository.Models
    {
        public class AdministradorModel
        {
            [Key]
            public int Id { get; set; }

            [Required(ErrorMessage = "Insira um Email!")]
            [EmailAddress(ErrorMessage = "Insira um Email válido!")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Insira uma senha!")]
            public byte[] SenhaHash { get; set; }
            public byte[] SenhaSalt { get; set; }
            public int Status { get; set; } = 1;
        }
    }
}