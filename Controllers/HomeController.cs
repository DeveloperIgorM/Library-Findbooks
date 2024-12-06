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

        public async Task<IActionResult> Index(string? pesquisar, int page = 1, int pageSize = 10)
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

            // Busca todos os livros ou com filtro, com paginação
            var livros = pesquisar == null
                ? await _livroInterface.GetLivrosFiltro(null, page, pageSize)
                : await _livroInterface.GetLivrosFiltro(pesquisar, page, pageSize);

            // Verifica se encontrou livros
            if (livros == null || !livros.Any())
            {
                TempData["MensagemErro"] = "Nenhum Livro foi encontrado com esse nome ou Isbn.";
                return View();
               
            }

            // Busca informações de instituições e quantidades para cada livro
            foreach (var livro in livros)
            {
                livro.InstituicaoLivros = await _livroInterface.GetInstituicaoLivroPorLivro(livro.Isbn);
            }

            // Envia a lista de livros e as informações de paginação para a View
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;

            return View(livros);
        }


        public IActionResult Home()
        {
            return RedirectToAction("Index", "Home");
        }

        public  IActionResult About()
        {
            ViewBag.ExibirRodape = false;
            return View();
        }

        public IActionResult Equipe()
        {
            ViewBag.ExibirRodape = false;
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

                // Verifica se o Id do usuário é válido e se o Status é "1"
                if (usuario.Id == 0 || usuario.Status != 1)
                {
                    TempData["MensagemErro"] = usuario.Status != 1
                        ? "Acesso negado! Instituição inativa ou pendente de aprovação."
                        : "Credenciais inválidas!";
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
