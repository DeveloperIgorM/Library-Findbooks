using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewRepository.Dto;
using NewRepository.Filtros;
using NewRepository.Models;
using NewRepository.Models.NewRepository.Models;
using NewRepository.Services.Adminstrador;
using NewRepository.Services.Livro;
using NewRepository.Services.SessaoService;
using NewRepository.Services.UsuarioService;
using SQLitePCL;
using System.Security.Claims;

public class AdministradorController : Controller
{
    private readonly IAdministradorInterface _administradorInterface;
    private readonly ILivroInterface _livroInterface;
    private readonly ISessaoInterface _sessaoInterface;
    private readonly Contexto _context;


    public AdministradorController(IAdministradorInterface administradorInterface, ISessaoInterface sessaoInterface, ILivroInterface livroInterface, Contexto context)
    {
        _administradorInterface = administradorInterface;
        _livroInterface = livroInterface;
        _sessaoInterface = sessaoInterface;
        _context = context;
    }

   

    public IActionResult Login()
    {
        return View();
    }

    public IActionResult Cadastrar()
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
        var AdministradorLogado = _sessaoInterface.BuscarSessaoAdm();
        ViewBag.AdministradorLogado = AdministradorLogado != null ? true : false;


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

    [HttpPost]
    public async Task<IActionResult> Cadastrar(AdministradorCriacaoDto administradorDto)
    {
        if (!ModelState.IsValid)
        {
            return View(administradorDto);
        }

        try
        {
            var novoAdministrador = await _administradorInterface.Cadastrar(administradorDto);
            TempData["MensagemSucesso"] = "Administrador criado com sucesso!";
            return RedirectToAction("Login");
        }
        catch (Exception ex)
        {
            TempData["MensagemErro"] = $"Erro ao criar administrador: {ex.Message}";
            return View(administradorDto);
        }
    }


    [HttpPost]

    public async Task<ActionResult<AdministradorModel>> Login(AdministradorCriacaoDto administradorDto)
    {
        if (ModelState.IsValid)
        {
            var administrador = await _administradorInterface.Login(administradorDto);

            if (administrador.Id == 0)
            {
                TempData["MensagemErro"] = "Credenciais inválidas!";
                return View(administradorDto);
            }
            else
            {
                TempData["MensagemSucesso"] = "Administrador logado com sucesso!";
                _sessaoInterface.CriarSessaoAdm(administrador); // Cria a sessão com o usuário logado

                
                HttpContext.Session.SetString("Nome", administrador.Nome);

                return RedirectToAction("Index", "Administrador");
            }
        }
        else
        {
            return View(administradorDto);
        }
    }
    [HttpGet]
    public async Task<IActionResult>Pendente()
    {
        var AdministradorLogado = _sessaoInterface.BuscarSessaoAdm();
        ViewBag.AdministradorLogado = AdministradorLogado != null ? true : false;


        if (AdministradorLogado != null)
        {
            ViewBag.Nome = AdministradorLogado.Nome;
        

        var Pendente = await _administradorInterface.Pendente();
            return View(Pendente); // Retorna para a View
        }else
        {
            return View(null);
        }
    }
    [AdministradorLogado]
    [HttpPost]
    public async Task<IActionResult> AtualizarStatusInstituicao(int id, int status)
    {
        try
        {
 
                // Verifica se o status é válido ("0" ou "1")
                if (status != 0 && status != 1)
            {
                TempData["MensagemErro"] = "Status inválido!";
                return RedirectToAction("Pendente");
            }
            

                var instituicao = await _context.Instituicoes.FindAsync(id);
            if (instituicao != null)
            {
                instituicao.Status = status; // Atualiza o status para "1" (ativo) ou "0" (inativo)
                _context.Instituicoes.Update(instituicao);
                await _context.SaveChangesAsync();
                TempData["MensagemSucesso"] = "Status atualizado com sucesso!";
            }
            else
            {
                TempData["MensagemErro"] = "Instituição não encontrada!";
            }

            return RedirectToAction("Pendente");
        }
        catch (Exception ex)
        {
            TempData["MensagemErro"] = $"Erro ao atualizar status: {ex.Message}";
            return RedirectToAction("Pendente");
        }
    }
}

