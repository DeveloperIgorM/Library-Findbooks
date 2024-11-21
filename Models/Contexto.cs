using Microsoft.EntityFrameworkCore;

namespace NewRepository.Models
{
    public class Contexto : DbContext
    {
        public DbSet<Biblioteca> Bibliotecas { get; set; }
        public DbSet<LivroModel> Livros { get; set; }
        public DbSet<UsuarioModel> Instituicoes { get; set; }
        public DbSet<InstituicaoLivroModel> InstituicaoLivros { get; set; }

        public DbSet<TokenModel> Tokens { get; set; }

        public Contexto(DbContextOptions<Contexto> opcoes) : base(opcoes) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configurar ISBN como índice único em LivroModel
            modelBuilder.Entity<LivroModel>()
                .HasIndex(l => l.Id)
                .IsUnique();

            // Configuração do relacionamento entre LivroModel e UsuarioModel
            modelBuilder.Entity<LivroModel>()
                .HasOne(l => l.Usuario)
                .WithMany(u => u.Livros)
                .HasForeignKey(l => l.UsuarioId);

            // Configuração do relacionamento entre InstituicaoLivroModel e UsuarioModel
            modelBuilder.Entity<InstituicaoLivroModel>()
                .HasOne(il => il.Usuario)
                .WithMany(i => i.InstituicaoLivros)
                .HasForeignKey(il => il.UsuarioId);

            // Configuração do relacionamento entre InstituicaoLivroModel e LivroModel
            modelBuilder.Entity<InstituicaoLivroModel>()
                .HasOne(il => il.Livro)
                .WithMany(l => l.InstituicaoLivros)
                .HasForeignKey(il => il.LivroId);

            // Chave composta para garantir unicidade de (UsuarioId, LivroId) em InstituicaoLivroModel
            modelBuilder.Entity<InstituicaoLivroModel>()
                .HasIndex(il => new { il.UsuarioId, il.LivroId })
                .IsUnique();

            base.OnModelCreating(modelBuilder);
        }
    }
}
