using Microsoft.EntityFrameworkCore;
using NewRepository.Dto;
using NewRepository.Models;
using NewRepository.Services.EmailService;
using System.Security.Cryptography;
using System.Text;

namespace NewRepository.Services.UsuarioService
{
    public class UsuarioService : IUsuarioInterface
    {
        private readonly Contexto _context;
        private readonly IEmailService _emailService;
        

        public UsuarioService(IEmailService emailService,Contexto contexto)
        {
            _context = contexto;
            _emailService = emailService;
        }

        public async Task<UsuarioModel> Cadastrar(UsuarioCriacaoDto usuarioCriacaoDto)
        {
            try
            {
                CriarSenhaHash(usuarioCriacaoDto.Senha, out byte[] senhaHash, out byte[] senhaSalt);

                var usuario = new UsuarioModel()
                {
                    Email = usuarioCriacaoDto.Email,
                    RazaoSocial = usuarioCriacaoDto.RazaoSocial,
                    NomeFantasia = usuarioCriacaoDto.NomeFantasia,
                    Cnpj = usuarioCriacaoDto.Cnpj,
                    Telefone = usuarioCriacaoDto.Telefone,
                    Endereco = usuarioCriacaoDto.Endereco,
                    SenhaHash = senhaHash,
                    SenhaSalt = senhaSalt
                };

                _context.Add(usuario);
                await _context.SaveChangesAsync();

                return usuario;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        //criptografando senha
        public void CriarSenhaHash(string senha, out byte[] senhaHash, out byte[] senhaSalt)
        {
            using (HMACSHA512 hmac = new HMACSHA512())
            {
                senhaSalt = hmac.Key;
                senhaHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(senha));
            }
        }

        public async Task<UsuarioModel> Login(LoginDto loginDto)
        {
            try
            {
                var usuario = await _context.Instituicoes.FirstOrDefaultAsync(user => user.Email == loginDto.Email);

                if (usuario == null)
                {
                    return new UsuarioModel();
                }

                if (!VerificarSenha(loginDto.Senha, usuario.SenhaHash, usuario.SenhaSalt))
                {
                    return new UsuarioModel();
                }

                return usuario;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private bool VerificarSenha(string senha, byte[] senhaHash, byte[] senhaSalt)
        {
            using (HMACSHA512 hmac = new HMACSHA512(senhaSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(senha));
                return computedHash.SequenceEqual(senhaHash);
            }
        }

        public async Task<string> GerarTokenRedefinicaoSenha(UsuarioModel usuario)
        {
            string token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            DateTime expiration = DateTime.UtcNow.AddHours(1); // Expira em 1 hora

            // Salve o token e sua expiração no banco de dados
            await _context.Tokens.AddAsync(new TokenModel
            {
                UsuarioId = usuario.Id,
                Token = token,
                ExpirationDate = expiration
            });
            await _context.SaveChangesAsync();

            return token;
        }


        public async Task EnviarEmailRedefinicaoSenha(string email, string callbackUrl)
        {
            var mensagem = $"Redefina sua senha usando o link: {callbackUrl}";
            await _emailService.EnviarEmailAsync(email, "Redefinição de Senha", mensagem);
        }

        public async Task<bool> RedefinirSenha(string email, string token, string novaSenha)
        {
            var usuario = await _context.Instituicoes.FirstOrDefaultAsync(u => u.Email == email);
            if (usuario == null) return false;

            var tokenEntry = await _context.Tokens
                .FirstOrDefaultAsync(t => t.UsuarioId == usuario.Id && t.Token == token);

            if (tokenEntry == null || tokenEntry.ExpirationDate < DateTime.UtcNow)
            {
                return false; // Token inválido ou expirado
            }

            usuario.SenhaHash = CriptografarSenha(novaSenha);
            _context.Tokens.Remove(tokenEntry); // Remova o token após o uso
            await _context.SaveChangesAsync();

            return true;
        }


        public async Task<UsuarioModel> ObterPorEmailAsync(string email)
        {
            return await _context.Instituicoes.FirstOrDefaultAsync(u => u.Email == email);
        }

        private byte[] CriptografarSenha(string senha)
        {
            // Implementação da lógica de criptografia
            using (var sha256 = SHA256.Create())
            {
                return sha256.ComputeHash(Encoding.UTF8.GetBytes(senha));
            }
        }

        public async Task<IEnumerable<UsuarioModel>> ObterInstituicoesPendentes()
        {
            // Busque as instituições pendentes no banco de dados (exemplo: status 0)
            return await Task.Run(() =>
                _context.Instituicoes.Where(i => i.Status == 0).ToList()
            );
        }
    }
}