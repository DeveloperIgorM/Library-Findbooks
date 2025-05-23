﻿using ClosedXML.Excel;
using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewRepository.Dto;
using NewRepository.Filtros;
using NewRepository.Models;
using NewRepository.Models.NewRepository.Models;
using NewRepository.Services;
using NewRepository.Services.Livro;
using NewRepository.Services.SessaoService;
using System.Data; // Para buscar o usuário logado

namespace NewRepository.Controllers
{
    public class LivrosController : Controller
    {
        private readonly Contexto _context;
        private readonly ILivroInterface _livroInterface;
        private readonly ISessaoInterface _sessaoService;
        private readonly IExcelInterface _excelInterface;

        public LivrosController(ILivroInterface livroInterface, ISessaoInterface sessaoService, Contexto context, IExcelInterface excelInterface)
        {
            _livroInterface = livroInterface;
            _sessaoService = sessaoService;
            _context = context;
            _excelInterface = excelInterface;
        }

        
        [UsuarioLogado]
      
        public async Task<IActionResult> Index()
        {
            ViewBag.ExibirRodape = false;
            var usuarioLogado = _sessaoService.BuscarSessao();
            var administradorLogado = _sessaoService.BuscarSessaoAdm();

            List<LivroModel> livros;


            if (usuarioLogado != null)
            {
                ViewBag.NomeFantasia = usuarioLogado.NomeFantasia; // Passa o NomeFantasia para a ViewBag
                                                                   // Usuário visualiza apenas os livros cadastrados por ele
                livros = await _livroInterface.GetLivrosPorUsuario(usuarioLogado.Id);
                ViewBag.UsuarioLogado = true;

            }else if (administradorLogado != null)
            {
                ViewBag.Nome = administradorLogado.Nome;
                // Administrador visualiza todos os livros
                livros = await _livroInterface.GetLivros();
           
            }
            else
            {
                // Nenhum usuário logado
                livros = new List<LivroModel>();
                ViewBag.UsuarioLogado = false;
            }

            return View(livros);
        }


        public IActionResult Cadastrar()
        {
            var usuarioLogado = _sessaoService.BuscarSessao();
            ViewBag.UsuarioLogado = usuarioLogado != null;
            if (usuarioLogado == null)
            {
                ViewBag.UsuarioLogado = false; // Define que o usuário não está logado
                return Unauthorized(); // Caso a sessão esteja expirada ou inválida
            }

            ViewBag.UsuarioLogado = true; // Define que o usuário está logado
            return View();
        }

        public async Task<IActionResult> Detalhes(int id)
        {
            ViewBag.ExibirRodape = false;
            // Verifica se há um usuário logado na sessão
            var usuarioLogado = _sessaoService.BuscarSessao();
            ViewBag.UsuarioLogado = usuarioLogado != null; // Define true se há sessão, caso contrário false

            var administradorLogado = _sessaoService.BuscarSessaoAdm();
            ViewBag.administradorLogado = administradorLogado != null;

            // Carrega o livro com as instituições e seus respectivos usuários relacionados
            var livro = await _context.Livros
                .Include(l => l.InstituicaoLivros) // Carrega as instituições relacionadas ao livro
                .ThenInclude(il => il.Usuario)      // Carrega os dados do usuário da instituição
                .FirstOrDefaultAsync(l => l.Id == id);

            if (livro == null)
            {
                return NotFound();
            }

            return View(livro);  // Passa o livro com as instituições e quantidades para a View
        }

        [UsuarioLogado]
        public async Task<IActionResult> Editar(int id)
        {
            // Verifica se o usuário ou administrador está logado
            var usuarioLogado = _sessaoService.BuscarSessao();
           

            if (usuarioLogado == null )
            {
                return Unauthorized(); // Retorna 401 caso nenhum esteja logado
            }

            // Busca o livro pelo ID
            var livro = await _livroInterface.GetLivroPorId(id);

            if (livro == null)
            {
                return NotFound(); // Retorna 404 se o livro não for encontrado
            }

            // Verifica se o usuário logado tem permissão para editar o livro
            if (usuarioLogado != null && livro.UsuarioId != usuarioLogado.Id)
            {
                return Forbid(); // Retorna 403 se o usuário tentar editar um livro que não cadastrou
            }

 
            else
            {
                
                ViewBag.NomeFantasia = usuarioLogado.NomeFantasia;
            }

            return View(livro); // Retorna a view com os dados do livro
        }


        public async Task<IActionResult> Remover(int id)
        {
            var usuarioLogado = _sessaoService.BuscarSessao();
          

            if (usuarioLogado == null)
            {
                return Unauthorized(); // Caso a sessão esteja expirada ou inválida
            }

            await _livroInterface.RemoverLivro(id);
            return RedirectToAction("Index", "Livros");
        }

        public async Task<IActionResult> MeusLivros(string? isbn)
        {
            var usuarioLogado = _sessaoService.BuscarSessao(); // Buscar o usuário logado
           

            if (usuarioLogado == null)
            {
                return Unauthorized(); // Caso a sessão esteja expirada ou inválida
            }

            List<LivroModel> livros;

            // Se um ISBN for fornecido, buscar apenas esse livro
            if (!string.IsNullOrEmpty(isbn))
            {
                var livro = await _livroInterface.GetLivroPorIsbnEUsuario(isbn, usuarioLogado.Id);
                livros = livro != null ? new List<LivroModel> { livro } : new List<LivroModel>();
            }
            else
            {
                // Caso não tenha ISBN, listar todos os livros do usuário
                livros = await _livroInterface.GetLivrosPorUsuario(usuarioLogado.Id);
            }

            return View(livros);
        }

        public IActionResult Exportar()
        {
            // Criação de um novo DataTable apenas com a estrutura (sem dados)
            DataTable dataTable = GetModeloEstrutura();

            using (XLWorkbook workBook = new XLWorkbook())
            {
                workBook.AddWorksheet(dataTable, "Modelo Estrutura");
                using (MemoryStream ms = new MemoryStream())
                {
                    workBook.SaveAs(ms);
                    return File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Modelo_Estrutura.xlsx");
                }
            }
        }

        private DataTable GetModeloEstrutura()
        {
            DataTable dataTable = new DataTable();
            dataTable.TableName = "Modelo";

            // Adiciona as colunas sem dados
            dataTable.Columns.Add("Isbn", typeof(string));
            dataTable.Columns.Add("Titulo", typeof(string));
            dataTable.Columns.Add("Genero", typeof(string));
            dataTable.Columns.Add("Ano Publicacao", typeof(string));
            dataTable.Columns.Add("Autor", typeof(string));
            dataTable.Columns.Add("Nome Editora", typeof(string));
            dataTable.Columns.Add("Sinopse", typeof(string));
            dataTable.Columns.Add("Quantidade", typeof(int));

            return dataTable;
        }

        public IActionResult ExportLivros()
        {
            // Criação de um novo DataTable apenas com a estrutura (sem dados)
            DataTable dataTable = GetExportLivros();

            using (XLWorkbook workBook = new XLWorkbook())
            {
                workBook.AddWorksheet(dataTable, "Exportar Livros");
                using (MemoryStream ms = new MemoryStream())
                {
                    workBook.SaveAs(ms);
                    return File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Exportar Livros.xlsx");
                }
            }
        }

        private DataTable GetExportLivros()
        {
            // Recupera o usuário logado
            var usuarioLogado = _sessaoService.BuscarSessao();

            if (usuarioLogado == null)
            {
                throw new UnauthorizedAccessException("Usuário não autorizado ou sessão expirada.");
            }

            DataTable dataTable = new DataTable();
            dataTable.TableName = "ExportLivros";

            // Adiciona as colunas ao DataTable
            dataTable.Columns.Add("Isbn", typeof(string));
            dataTable.Columns.Add("Titulo", typeof(string));
            dataTable.Columns.Add("Genero", typeof(string));
            dataTable.Columns.Add("Ano Publicacao", typeof(string));
            dataTable.Columns.Add("Autor", typeof(string));
            dataTable.Columns.Add("Nome Editora", typeof(string));
            dataTable.Columns.Add("Sinopse", typeof(string));
            dataTable.Columns.Add("Quantidade", typeof(int));

            // Filtra os livros cadastrados pelo usuário logado
            var livros = _context.Livros
                .Where(livro => livro.UsuarioId == usuarioLogado.Id) // Filtra pelo ID do usuário logado
                .ToList();

            // Busca as informações de InstituicaoLivroModel associadas
            var instituicaoLivros = _context.InstituicaoLivros
                .Where(il => livros.Select(l => l.Id).Contains(il.LivroId)) // Obtém apenas os registros associados aos livros
                .ToList();

            livros.ForEach(livro =>
            {
                // Obtém a quantidade da InstituicaoLivroModel correspondente
                var quantidade = instituicaoLivros
                    .Where(il => il.LivroId == livro.Id)
                    .Sum(il => il.Quantidade); // Soma as quantidades se houver mais de uma instituição

                // Trata o campo AnoPublicacao para garantir que ele contenha apenas um ano válido
                string anoPublicacao = livro.AnoPublicacao;

                dataTable.Rows.Add(
                    livro.Isbn,
                    livro.Titulo,
                    livro.Genero,
                    anoPublicacao, // Usa o ano diretamente
                    livro.Autor,
                    livro.NomeEditatora,
                    livro.Sinopse,
                    quantidade // Adiciona a quantidade
                );
            });


            return dataTable;
        }


        [HttpPost]
        public async Task<IActionResult> Cadastrar(LivroCriacaoDto livroCriacaoDto, IFormFile foto)
        {
            var usuarioLogado = _sessaoService.BuscarSessao();
            if (usuarioLogado == null)
            {
                ViewBag.UsuarioLogado = false; // Define que o usuário não está logado
                return Unauthorized(); // Caso a sessão esteja expirada ou inválida
            }

            ViewBag.UsuarioLogado = true; // Define que o usuário está logado

            // Verifica se o ISBN já existe para essa instituição
            var livroExistente = await _livroInterface.GetLivroPorIsbnEUsuario(livroCriacaoDto.Isbn, usuarioLogado.Id);
            if (livroExistente != null)
            {
                ModelState.AddModelError("Isbn", "Um livro com esse número de ISBN já foi cadastrado por sua instituição.");
                return View(livroCriacaoDto);
            }

            if (ModelState.IsValid)
            {
                var livro = await _livroInterface.CriarLivro(livroCriacaoDto, foto, usuarioLogado.Id); // Passa o ID da biblioteca logada
                return RedirectToAction("Index", "Livros");
            }
            else
            {
                return View(livroCriacaoDto);
            }
        }

        [HttpPost]
       
        public async Task<IActionResult> Editar(LivroModel livro, IFormFile? foto)
        {
            var usuarioLogado = _sessaoService.BuscarSessao();
            
            if (usuarioLogado == null)
            {
                ViewBag.UsuarioLogado = false; // Define que o usuário não está logado
                return RedirectToAction("Login", "Usuario"); // Redireciona para a página de login caso a sessão esteja expirada
            }
         
            ViewBag.UsuarioLogado = true;
           
            if (ModelState.IsValid)
            {
                // Salva o livro editado
                await _livroInterface.EditarLivro(livro, foto);

                // Redireciona para a ação Index mantendo o status de logado
                return RedirectToAction("Index");
            }

            // Retorna a view com as informações de erro caso o ModelState seja inválido
            return View(livro);
        }

        [HttpPost]
        public IActionResult ImportExcel(IFormFile form)
        {
            // Recupera a sessão do usuário logado
            var usuarioLogado = _sessaoService.BuscarSessao();

            // Verifica se o usuário está logado, caso contrário retorna não autorizado
            if (usuarioLogado == null)
            {
                return Unauthorized(); // Sessão expirada ou inválida
            }

            // Verifica se o estado do Model está válido
            if (ModelState.IsValid)
            {
                // Lê o arquivo Excel em um stream
                var streamFile = _excelInterface.LerStream(form);

                // Lê os dados do Excel e passa o ID do usuário logado para o método LerXls
                var livros = _excelInterface.LerXls(streamFile, usuarioLogado.Id); // Corrige para usar 'Id' do usuário logado

                // Salva os dados dos livros e passa o ID do usuário
                _excelInterface.SalvarDados(livros, usuarioLogado.Id);

                return RedirectToAction("Index", "Livros"); // Redireciona para a lista de livros após a importação
            }

            return RedirectToAction("Cadastrar", "Livros");
        }
    }
}
