using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewRepository.Dto;
using NewRepository.Services.Adminstrador;
using NewRepository.Services.UsuarioService;
using System.Security.Claims;

public class AdministradorController : Controller
{
    private readonly IAdministradorInterface _administradorService;
    private readonly IUsuarioInterface _usuarioInterface;


    public AdministradorController(IAdministradorInterface administradorService, IUsuarioInterface usuarioInterface)
    {
        _administradorService = administradorService;
        _usuarioInterface = usuarioInterface;

    }

    public IActionResult Login()
    {
        return View();
    }

    public IActionResult Cadastrar()
    {
        return View();
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
            var novoAdministrador = await _administradorService.Cadastrar(administradorDto);
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

    public async Task<IActionResult> Login(string email, string senha)
    {
        var admin = await _administradorService.Login(email, senha);

        if (admin == null)
        {
            TempData["MensagemErro"] = "Credenciais inválidas.";
            return View();
        }

        // Autenticar o administrador (exemplo usando Claims)
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, admin.Email),
            new Claim("Id", admin.Id.ToString())
        };

        var identity = new ClaimsIdentity(claims, "Login");
        var principal = new ClaimsPrincipal(identity);
        await HttpContext.SignInAsync(principal);

        return RedirectToAction("InstituicoesPendentes", "Usuario");
    }

    [Authorize]
    public async Task<IActionResult> InstituicoesPendentes()
    {
        // Verifica se o usuário atual é um administrador
        var adminId = Convert.ToInt32(HttpContext.User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value);

        if (!await _administradorService.ValidarAdministrador(adminId))
        {
            return Unauthorized(); // Retorna 401 se não for um administrador
        }

        var instituicoesPendentes = await _usuarioInterface.ObterInstituicoesPendentes();
        return View(instituicoesPendentes);
    }
}
