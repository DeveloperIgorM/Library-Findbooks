using NewRepository.Dto;
using NewRepository.Models.NewRepository.Models;

namespace NewRepository.Services.Adminstrador
{
    public interface IAdministradorInterface
    {
        Task<AdministradorModel> Login(AdministradorCriacaoDto administradorDto);
        Task<AdministradorModel> Cadastrar(AdministradorCriacaoDto administradorDto);

    }
}
