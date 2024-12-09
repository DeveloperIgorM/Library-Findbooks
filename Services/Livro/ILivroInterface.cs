using NewRepository.Dto;
using NewRepository.Models;

namespace NewRepository.Services.Livro
{
    public interface ILivroInterface
    {
        Task<LivroModel> CriarLivro(LivroCriacaoDto livroCriacaoDto, IFormFile foto, int usuarioId);
        Task<List<LivroModel>> GetLivros();
        Task<LivroModel> GetLivroPorId(int id);
        Task<LivroModel> EditarLivro(LivroModel livro, IFormFile? foto);
        Task<LivroModel> RemoverLivro(int id);
        Task<List<LivroModel>> GetLivrosFiltro(string? pesquisar);
        Task<List<LivroModel>> GetLivrosPorUsuario(int usuarioId);
        Task<LivroModel?> GetLivroPorIsbnEUsuario(string isbn, int usuarioId);
        Task CadastrarLivrosEmLote(List<LivroCriacaoDto> livrosCriacaoDto, int usuarioId); // Ajustado
     //   Task AtualizarQuantidadeLivro(int instituicaoId, string isbn, int quantidade);
        Task<List<InstituicaoLivroModel>> GetInstituicaoLivroPorLivro(string isbn);
    }
}
