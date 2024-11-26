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


        public async Task<AdministradorModel> Login(AdministradorCriacaoDto administradorDto)
        {
            try
            {
                var administrador = await _context.Administradores.FirstOrDefaultAsync(user => user.Email == administradorDto.Email);

                if (administrador == null)
                {
                    return new AdministradorModel();
                }

                if (!VerificarSenha(administradorDto.Senha, administrador.SenhaHash, administrador.SenhaSalt))
                {
                    return new AdministradorModel();
                }

                return administrador;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<AdministradorModel> Cadastrar(AdministradorCriacaoDto administradorDto)
        {
            try
            {
                CriarSenhaHash(administradorDto.Senha, out byte[] senhaHash, out byte[] senhaSalt);

                var administrador = new AdministradorModel()
                {
                    Email = administradorDto.Email,
                    SenhaHash = senhaHash,
                    SenhaSalt = senhaSalt
                };

                _context.Add(administrador);
                await _context.SaveChangesAsync();

                return administrador;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
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
        private bool VerificarSenha(string senha, byte[] senhaHash, byte[] senhaSalt)
        {
            using (var hmac = new HMACSHA512(senhaSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(senha));
                return computedHash.SequenceEqual(senhaHash);
            }
        }
    }
}