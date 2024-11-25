using Microsoft.EntityFrameworkCore;
using NewRepository.Models;
using NewRepository.Services;
using NewRepository.Services.Adminstrador;
using NewRepository.Services.EmailService;
using NewRepository.Services.Livro;
using NewRepository.Services.SessaoService;
using NewRepository.Services.UsuarioService;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<ILivroInterface, LivroService>();
builder.Services.AddScoped<IUsuarioInterface, UsuarioService>();
builder.Services.AddScoped<ISessaoInterface, SessaoService>();
builder.Services.AddScoped<IExcelInterface, ImportarPlanilhaExcelProjeto.Services.ExcelService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IUsuarioInterface, UsuarioService>();
builder.Services.AddScoped<IAdministradorInterface, AdministradorService>();    


builder.Services.AddDbContext<Contexto>(opcoes =>
    opcoes.UseSqlite(builder.Configuration.GetConnectionString("ConexaoSQlite")));


builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error"); // Redireciona para a página de erro em modo de produção
    app.UseHsts();
}

app.UseHttpsRedirection(); // Redireciona todas as requisições para HTTPS
app.UseStaticFiles(); // Permite arquivos estáticos (CSS, JS, imagens)

app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();


app.UseEndpoints(endpoints =>
{
    _ = endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});




app.Run();

