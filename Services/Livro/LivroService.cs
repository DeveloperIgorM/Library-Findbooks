using DocumentFormat.OpenXml.InkML;
using Microsoft.EntityFrameworkCore;
using NewRepository.Dto;
using NewRepository.Filtros;
using NewRepository.Models;
using System.Linq.Expressions;

namespace NewRepository.Services.Livro
{
    // Implementação do ILivroInterface
    public class LivroService : ILivroInterface
    {
        private readonly Contexto _contexto;
        private readonly string _sistema;

        public LivroService(Contexto contexto, IWebHostEnvironment sistema)
        {
            _contexto = contexto;
            _sistema = sistema.WebRootPath;
        }

        // Método para gerar caminho único para a imagem do livro
        public string GeraCaminhoArquivo(IFormFile foto)
        {
            var codigoUnico = Guid.NewGuid().ToString();
            var nomeCaminhoImage = foto.FileName.Replace(" ", "").ToLower() + codigoUnico + ".png";
            var caminhoParaSalvarImagens = _sistema + "\\imagem\\";

            if (!Directory.Exists(caminhoParaSalvarImagens))
            {
                Directory.CreateDirectory(caminhoParaSalvarImagens);
            }

            using (var stream = File.Create(caminhoParaSalvarImagens + nomeCaminhoImage))
            {
                foto.CopyToAsync(stream).Wait();
            }

            return nomeCaminhoImage;
        }

        // Método para criar um livro
        public async Task<LivroModel> CriarLivro(LivroCriacaoDto livroCriacaoDto, IFormFile foto, int usuarioId)
        {
            try
            {
                // Gera o caminho da imagem, se a foto foi enviada
                var nomeCaminhoImagem = foto != null ? GeraCaminhoArquivo(foto) : null;

                // Busca o usuário (instituição) pelo ID fornecido
                var usuario = await _contexto.Instituicoes.FirstOrDefaultAsync(u => u.Id == usuarioId);
                if (usuario == null)
                {
                    throw new Exception("Usuário não encontrado.");
                }

                // Cria uma nova instância de LivroModel
                var livro = new LivroModel
                {
                    Capa = nomeCaminhoImagem,
                    Isbn = livroCriacaoDto.Isbn,
                    Titulo = livroCriacaoDto.Titulo,
                    Sinopse = livroCriacaoDto.Sinopse,
                    Autor = livroCriacaoDto.Autor,
                    AnoPublicacao = livroCriacaoDto.AnoPublicacao,
                    NomeEditatora = livroCriacaoDto.NomeEditatora,
                    Genero = livroCriacaoDto.Genero,
                    UsuarioId = usuarioId,
                    Usuario = usuario,
                    FonteCadastro = "manual"
                };

                // Cria uma nova instância de InstituicaoLivroModel para associar o livro à instituição com a quantidade especificada
                var instituicaoLivro = new InstituicaoLivroModel
                {
                    Livro = livro,
                    Usuario = usuario,
                    Quantidade = livroCriacaoDto.Quantidade
                };

                // Adiciona o livro e a relação InstituicaoLivro ao contexto
                _contexto.Add(livro);
                _contexto.Add(instituicaoLivro);

                await _contexto.SaveChangesAsync();

                return livro;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao criar o livro: {ex.Message}");
            }
        }

        // Método para obter todos os livros
        public async Task<List<LivroModel>> GetLivros()
        {
            try
            {
                return await _contexto.Livros
                    .Include(l => l.InstituicaoLivros)  // Inclui as quantidades associadas à entidade InstituicaoLivros
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        // Método para obter um livro por ID
        public async Task<LivroModel> GetLivroPorId(int id)
        {
            try
            {
                return await _contexto.Livros.FirstOrDefaultAsync(livro => livro.Id == id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // Método para editar um livro
     
        public async Task<LivroModel> EditarLivro(LivroModel livro, IFormFile? foto)
        {
            try
            {
                // Carrega o livro existente no banco de dados para atualização.
                var livroBanco = await _contexto.Livros.FirstOrDefaultAsync(livroBD => livroBD.Id == livro.Id);
                if (livroBanco == null)
                {
                    throw new Exception("Livro não encontrado.");
                }

                // Variável para armazenar o caminho da imagem, se houver.
                string nomeCaminhoImagem = livroBanco.Capa;

                if (foto != null)
                {
                    // Caminho da capa existente
                    string caminhoCapaExistente = Path.Combine(_sistema, "imagem", livroBanco.Capa);

                    // Se a imagem já existir, ela é deletada
                    if (File.Exists(caminhoCapaExistente))
                    {
                        File.Delete(caminhoCapaExistente);
                    }

                    // Gera o novo caminho da imagem
                    nomeCaminhoImagem = GeraCaminhoArquivo(foto);
                }

                // Atualiza as propriedades do livro com os dados recebidos
                livroBanco.Isbn = livro.Isbn;
                livroBanco.Titulo = livro.Titulo;
                livroBanco.Sinopse = livro.Sinopse;
                livroBanco.Autor = livro.Autor;
                livroBanco.AnoPublicacao = livro.AnoPublicacao;
                livroBanco.NomeEditatora = livro.NomeEditatora;
                livroBanco.Genero = livro.Genero;
              //  livroBanco.Quantidade = livro.Quantidade;

                // Atualiza a quantidade de exemplares, se fornecida

                // Se uma nova capa for fornecida, substitui a antiga
                if (!string.IsNullOrEmpty(nomeCaminhoImagem))
                {
                    livroBanco.Capa = nomeCaminhoImagem;
                }

                // Atualiza o contexto e salva as mudanças
                _contexto.Livros.Update(livroBanco);
                await _contexto.SaveChangesAsync();

                return livroBanco;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao editar o livro: " + ex.Message);
            }
        }


        // Método para remover um livro
        public async Task<LivroModel> RemoverLivro(int id)
        {
            try
            {
                var livro = await _contexto.Livros.FirstOrDefaultAsync(livroBanco => livroBanco.Id == id);
                _contexto.Remove(livro);
                await _contexto.SaveChangesAsync();
                return livro;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // Método para buscar livros com filtro
        public async Task<List<LivroModel>> GetLivrosFiltro(string? pesquisar, int page = 1, int pageSize = 10)
        {
            IQueryable<LivroModel> query = _contexto.Livros;

            if (!string.IsNullOrWhiteSpace(pesquisar))
            {
                pesquisar = pesquisar.Trim().ToLower();
                query = query.Where(livroBanco =>
                    livroBanco.Titulo.ToLower().Contains(pesquisar) ||
                    livroBanco.Isbn.ToLower().Contains(pesquisar) ||
                    livroBanco.Autor.ToLower().Contains(pesquisar));
            }

            return await query
                .Skip((page - 1) * pageSize) // Pula os registros das páginas anteriores
                .Take(pageSize) // Traz apenas os registros da página atual
                .ToListAsync();
        }

        public async Task<List<LivroModel>> GetLivrosPorUsuario(int usuarioId)
{
    try
    {
        return await _contexto.Livros
            .Include(l => l.InstituicaoLivros)  // Inclui as quantidades associadas à entidade InstituicaoLivros
            .Where(livro => livro.UsuarioId == usuarioId)  // Filtra os livros do usuário logado
            .ToListAsync();
    }
    catch (Exception ex)
    {
        throw new Exception(ex.Message);
    }
}


        public async Task<LivroModel?> GetLivroPorIsbnEUsuario(string isbn, int usuarioId)
        {
            return await _contexto.Livros
                .FirstOrDefaultAsync(l => l.Isbn == isbn && l.UsuarioId == usuarioId); // Verifica ISBN e ID do usuário
        }

        public async Task CadastrarLivrosEmLote(List<LivroCriacaoDto> livrosCriacaoDto, int usuarioId)
        {
            foreach (var livroDto in livrosCriacaoDto)
            {
                var livro = new LivroModel
                {
                    Titulo = livroDto.Titulo,
                    Autor = livroDto.Autor,
                    Isbn = livroDto.Isbn,
                    AnoPublicacao = livroDto.AnoPublicacao,
                    NomeEditatora = livroDto.NomeEditatora,
                    Genero = livroDto.Genero,
                   
                    FonteCadastro = "lote", // Define a fonte de cadastro como "lote"
                    UsuarioId = usuarioId
                };

                _contexto.Livros.Add(livro);
            }

            await _contexto.SaveChangesAsync();
        }


        public async Task<List<InstituicaoLivroModel>> GetInstituicaoLivroPorLivro(string isbn)
        {
            // Busca o LivroId usando o ISBN
            var livro = await _contexto.Livros.FirstOrDefaultAsync(l => l.Isbn == isbn);
            if (livro == null)
            {
                throw new Exception("Livro não encontrado com o ISBN especificado.");
            }

            var livroId = livro.Id;

            // Retorna a lista de InstituicaoLivroModel filtrando pelo LivroId
            return await _contexto.InstituicaoLivros
                .Where(il => il.LivroId == livroId)
                .Include(il => il.Usuario) // Inclui dados da instituição
                .ToListAsync();
        }

    }
}
