using NewRepository.Models;
using NewRepository.Models.NewRepository.Models;
using Newtonsoft.Json;

namespace NewRepository.Services.SessaoService
{
    public class SessaoService : ISessaoInterface
    {
        private readonly IHttpContextAccessor _httpAcessor;

        public SessaoService(IHttpContextAccessor httpAccessor)
        {
            _httpAcessor = httpAccessor;
        }

        public UsuarioModel BuscarSessao()
        {
            string sessaoUsuario = _httpAcessor.HttpContext.Session.GetString("UsuarioAtivo");
            if (sessaoUsuario == null)
            {
                return null;
            }

            return JsonConvert.DeserializeObject<UsuarioModel>(sessaoUsuario);
        }

        public AdministradorModel BuscarSessaoAdm()
        {
            string sessaoAdministrador = _httpAcessor.HttpContext.Session.GetString("AdministradorAtivo");
            if (sessaoAdministrador == null)
            {
                return null;
            }

            return JsonConvert.DeserializeObject<AdministradorModel>(sessaoAdministrador);
        }

        public void CriarSessao(UsuarioModel usuario)
        {
            string usuarioJson = JsonConvert.SerializeObject(usuario);
            _httpAcessor.HttpContext.Session.SetString("UsuarioAtivo", usuarioJson);
        }

        public void CriarSessaoAdm(AdministradorModel administrador)
        {
            string administradorJson = JsonConvert.SerializeObject(administrador);
            _httpAcessor.HttpContext.Session.SetString("AdministradorAtivo", administradorJson);
        }

        public void RemoverSessao()
        {
            _httpAcessor.HttpContext.Session.Remove("UsuarioAtivo");
        }
    }
}
