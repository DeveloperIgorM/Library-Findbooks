using Microsoft.AspNetCore.Mvc;
using NewRepository.Dto;
using NewRepository.Models;
using NewRepository.Services.Adminstrador;
using NewRepository.Services.UsuarioService;

namespace NewRepository.Controllers
{
    public class UsuarioController : Controller
    {

        private readonly IUsuarioInterface _usuarioInterface;
   
        public UsuarioController(IUsuarioInterface usuarioInterface)
        {
            _usuarioInterface = usuarioInterface;
           
        }

        public IActionResult Cadastrar()
        {
            return View();
        }

  


        [HttpPost]
        public async Task<ActionResult<UsuarioModel>> Cadastrar(UsuarioCriacaoDto usuarioCriacaoDto)
        {
            if (ModelState.IsValid)
            {
                //cadastrar usuário no banco e atribuir a variável "usuario"
                var usuario = await _usuarioInterface.Cadastrar(usuarioCriacaoDto);

                if(usuario != null)
                {
                    TempData["MensagemSucesso"] = "Usuário cadastrado com sucesso!";
                    return RedirectToAction("Index", "Livros");
                }
                else
                {
                    TempData["MensagemErro"] = "Ocorreu um erro no momento do cadastro!";
                    return View(usuarioCriacaoDto);
                }

            }
            else
            {
                return View(usuarioCriacaoDto);
            }
        }

        
        public async Task<IActionResult> SolicitarRedefinicaoSenha()
        {
            
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> SolicitarRedefinicaoSenha(string email)
        {
            var usuario = await _usuarioInterface.ObterPorEmailAsync(email);
            if (usuario == null)
            {
                TempData["MensagemErro"] = "Usuário não encontrado.";
                return RedirectToAction("SolicitarRedefinicaoSenha");
            }

            var token = await _usuarioInterface.GerarTokenRedefinicaoSenha(usuario);
            var callbackUrl = Url.Action("RedefinirSenha", "Usuario", new { token }, Request.Scheme);
            await _usuarioInterface.EnviarEmailRedefinicaoSenha(usuario.Email, callbackUrl);

            TempData["MensagemSucesso"] = "Email de redefinição de senha enviado.";
            return RedirectToAction("SolicitarRedefinicaoSenha");
        }



        [HttpPost]
        public IActionResult RedefinirSenha(string token)
        {
            return View(new RedefinirSenhaViewModel { Token = token });
        }

        [HttpPost]
        public async Task<IActionResult> RedefinirSenha(RedefinirSenhaViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var resultado = await _usuarioInterface.RedefinirSenha(model.Email, model.Token, model.NovaSenha);
            if (resultado) return RedirectToAction("Login");

            return BadRequest("Erro ao redefinir senha.");
        }
    }
}