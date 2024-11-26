using Microsoft.AspNetCore.Mvc;
using NewRepository.Dto;
using NewRepository.Models;
using NewRepository.Services.Livro;
using NewRepository.Services.SessaoService;
using NewRepository.Services.UsuarioService;
using System.Threading.Tasks;

namespace NewRepository.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILivroInterface _livroInterface;
        private readonly IUsuarioInterface _usuarioInterface;
        private readonly ISessaoInterface _sessaoInterface;

        public HomeController(ILivroInterface livroInterface, IUsuarioInterface usuarioInterface, ISessaoInterface sessaoInterface)
        {
            _livroInterface = livroInterface;
            _usuarioInterface = usuarioInterface;
            _sessaoInterface = sessaoInterface;
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Sair()
        {
            _sessaoInterface.RemoverSessao();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Index(string? pesquisar)
        {
            var usuarioLogado = _sessaoInterface.BuscarSessao();
            var AdministradorLogado = _sessaoInterface.BuscarSessaoAdm();

            ViewBag.UsuarioLogado = usuarioLogado != null;
            ViewBag.AdministradorLogado = AdministradorLogado != null;

            if (usuarioLogado != null)
            {
                ViewBag.NomeFantasia = usuarioLogado.NomeFantasia;
            }

            if (AdministradorLogado != null)
            {
                ViewBag.Nome = AdministradorLogado.Nome;
            }
            // Busca todos os livros ou com filtro
            var livros = pesquisar == null
                ? await _livroInterface.GetLivros()
                : await _livroInterface.GetLivrosFiltro(pesquisar);

            // Busca informações de instituições e quantidades para cada livro
            foreach (var livro in livros)
            {
                livro.InstituicaoLivros = await _livroInterface.GetInstituicaoLivroPorLivro(livro.Isbn);
            }

            return View(livros);
        }


        public IActionResult Home()
        {
            return RedirectToAction("Index", "Home");
        }

        public  IActionResult About()
        {
            return View();
        }

        public IActionResult Equipe()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult<UsuarioModel>> Login(LoginDto loginDto)
        {
            if (ModelState.IsValid)
            {
                var usuario = await _usuarioInterface.Login(loginDto);

                if (usuario.Id == 0)
                {
                    TempData["MensagemErro"] = "Credenciais inválidas!";
                    return View(loginDto);
                }
                else
                {
                    TempData["MensagemSucesso"] = "Usuário logado com sucesso!";
                    _sessaoInterface.CriarSessao(usuario); // Cria a sessão com o usuário logado

                    // Salva o NomeFantasia na sessão para ser exibido na barra de navegação
                    HttpContext.Session.SetString("NomeFantasia", usuario.NomeFantasia);

                    return RedirectToAction("Index", "Livros");
                }
            }
            else
            {
                return View(loginDto);
            }
        }
    }
}
