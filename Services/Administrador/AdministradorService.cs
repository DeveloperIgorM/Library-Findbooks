using Microsoft.EntityFrameworkCore;
using NewRepository.Dto;
using NewRepository.Models;
using NewRepository.Models.NewRepository.Models;
using Org.BouncyCastle.Crypto.Generators;
using System.Security.Cryptography;
using System.Text;

namespace NewRepository.Services.Adminstrador
{
    public class AdministradorService : IAdministradorInterface
    {
        private readonly Contexto _context;
        public AdministradorService(Contexto context)
        {
            _context = context;
        }

        private bool VerificarSenha(string senha, byte[] senhaHash, byte[] senhaSalt)
        {
            using (var hmac = new HMACSHA512(senhaSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(senha));
                return computedHash.SequenceEqual(senhaHash);
            }
        }

        public async Task<bool> ValidarAdministrador(int id)
        {
            return await _context.Administradores.AnyAsync(a => a.Id == id);
        }

        async Task<AdministradorModel> IAdministradorInterface.Login(string email, string senha)
        {
            var admin = await _context.Administradores.FirstOrDefaultAsync(a => a.Email == email);

            if (admin == null || !VerificarSenha(senha, admin.SenhaHash, admin.SenhaSalt))
                return null;

            return admin;
        }

        public async Task<AdministradorModel> Cadastrar(AdministradorCriacaoDto administradorDto)
        {
            try
            {
                // Gerar o hash e salt da senha
                CriarSenhaHash(administradorDto.Senha, out byte[] senhaHash, out byte[] senhaSalt);

                // Criar a instância do administrador com os dados fornecidos
                var administrador = new AdministradorModel()
                {

                    Email = administradorDto.Email,
                    SenhaHash = senhaHash,
                    SenhaSalt = senhaSalt,

                };

                // Adicionar ao banco de dados
                _context.Administradores.Add(administrador);
                await _context.SaveChangesAsync();

                // Retornar o administrador criado
                return administrador;
            }
            catch (Exception ex)
            {
                // Lançar uma exceção com a mensagem de erro
                throw new Exception($"Erro ao cadastrar administrador: {ex.Message}");
            }
        }
        private void CriarSenhaHash(string senha, out byte[] senhaHash, out byte[] senhaSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                senhaSalt = hmac.Key;
                senhaHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(senha));
            }
        }
    }
}