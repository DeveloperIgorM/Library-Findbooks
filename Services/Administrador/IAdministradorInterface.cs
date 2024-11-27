using NewRepository.Dto;
using NewRepository.Models;
using NewRepository.Models.NewRepository.Models;

namespace NewRepository.Services.Adminstrador
{
    public interface IAdministradorInterface
    {
        Task<AdministradorModel> Login(AdministradorCriacaoDto administradorDto);
        Task<AdministradorModel> Cadastrar(AdministradorCriacaoDto administradorDto);
        Task<List<UsuarioModel>> Pendente();

    }
}
