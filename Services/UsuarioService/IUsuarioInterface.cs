using NewRepository.Dto;
using NewRepository.Filtros;
using NewRepository.Models;

namespace NewRepository.Services.UsuarioService
{
    public interface IUsuarioInterface
    {
        Task<UsuarioModel> Cadastrar(UsuarioCriacaoDto usuarioCriacaoDto);
        Task<UsuarioModel> Login(LoginDto loginDto);
        Task<string> GerarTokenRedefinicaoSenha(UsuarioModel usuario);
        Task EnviarEmailRedefinicaoSenha(string email, string callbackUrl);
        Task<bool> RedefinirSenha(string email, string token, string novaSenha);
        Task<UsuarioModel> ObterPorEmailAsync(string email);
        Task<IEnumerable<UsuarioModel>> ObterInstituicoesPendentes();
    }
}
