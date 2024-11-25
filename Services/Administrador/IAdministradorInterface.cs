using NewRepository.Dto;
using NewRepository.Models.NewRepository.Models;

namespace NewRepository.Services.Adminstrador
{
    public interface IAdministradorInterface
    {
        Task<AdministradorModel> Login(string email, string senha);
        Task<bool> ValidarAdministrador(int id);
        Task<AdministradorModel> Cadastrar(AdministradorCriacaoDto administradorDto);

    }
}
