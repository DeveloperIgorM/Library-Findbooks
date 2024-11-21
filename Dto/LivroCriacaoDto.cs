
    namespace NewRepository.Dto
    {
        public class LivroCriacaoDto
        {
            public string Capa { get; set; } = string.Empty;
            public string Isbn { get; set; } = string.Empty;  // Código ISBN do livro
            public string Titulo { get; set; } = string.Empty;
            public string Sinopse {  get; set; } = string.Empty;
            public string Autor { get; set; } = string.Empty;
            public string AnoPublicacao { get; set; } = string.Empty;
            public string NomeEditatora { get; set; } = string.Empty;
            public string Genero { get; set; } = string.Empty;  // Gênero do livro
            public int UsuarioId { get; set; } // Referência à biblioteca
         
        // Adiciona o campo Quantidade
        public int Quantidade { get; set; }
        }
    }


