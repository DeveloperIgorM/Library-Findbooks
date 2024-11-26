using NewRepository.Models;
using NewRepository.Models.NewRepository.Models;

namespace NewRepository.Services.SessaoService
{
    public interface ISessaoInterface
    {
        UsuarioModel BuscarSessao();
        AdministradorModel BuscarSessaoAdm();
        
            void CriarSessao(UsuarioModel usuario);
            void CriarSessaoAdm(AdministradorModel administrador);

            void RemoverSessao();
    }
    
}
